using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System;

[DisallowMultipleComponent]
public class GameManager : SingletonMonobehaviour<GameManager>
{
    [Header("Game Object References")]
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Dungeon Levels")]
    [SerializeField] private List<DungeonLevelSO> dungeonLevelList;
    [SerializeField] private int currentDungeonLevelListIndex = 0;

    private Room currentRoom;
    private Room previousRoom;
    private PlayerDetailsSO playerDetails;
    private Player player;
    private long gameScore;
    private int scoreMultiplier;
    private InstantiatedRoom bossRoom;
    private bool isFading = false;

    [HideInInspector] public GameState gameState;
    [HideInInspector] public GameState previousGameState;

    public Room GetCurrentRoom => currentRoom;
    public Player GetPlayer => player;
    public Sprite GetPlayerMinimaoIcon => playerDetails.playerMiniMapIcon;
    public DungeonLevelSO GetCurrentDungeonLevel => dungeonLevelList[currentDungeonLevelListIndex];

    public Room SetCurrentRoom
    {
        set
        {
            previousRoom = currentRoom;
            currentRoom = value;
        }
    }

    protected override void Awake()
    {
        base.Awake();

        playerDetails = GameResources.Instance.currentPlayer.playerDetails;

        InstantiatePlayer();
    }

    private void InstantiatePlayer()
    {
        GameObject playerGameObject = Instantiate(playerDetails.playerPrefab);

        player = playerGameObject.GetComponent<Player>();

        player.Initialize(playerDetails);
    }

    private void OnEnable()
    {
        StaticEventHandler.OnRoomChanged += CallStaticEventHandler_OnRoomChanged;

        StaticEventHandler.OnRoomEnemiesDefeated += CallStaticEventHandler_OnRoomEnemiesDefeated;

        StaticEventHandler.OnPointScored += CallStaticEventHandler_OnPointScored;

        StaticEventHandler.OnMultiplier += CallStaticEventHandler_OnMultiplier;

        player.PlayerDestroyedEvent.OnDestoryed += CallPlayer_OnDestroyed;
    }

    private void OnDisable()
    {
        StaticEventHandler.OnRoomChanged -= CallStaticEventHandler_OnRoomChanged;

        StaticEventHandler.OnRoomEnemiesDefeated -= CallStaticEventHandler_OnRoomEnemiesDefeated;

        StaticEventHandler.OnPointScored -= CallStaticEventHandler_OnPointScored;

        StaticEventHandler.OnMultiplier -= CallStaticEventHandler_OnMultiplier;

        player.PlayerDestroyedEvent.OnDestoryed -= CallPlayer_OnDestroyed;
    }

    private void CallStaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
    {
        SetCurrentRoom = roomChangedEventArgs.Room;
    }

    private void CallStaticEventHandler_OnRoomEnemiesDefeated(RoomEnemiesDefeatedArgs roomEnemiesDefeatedArgs)
    {
        RoomEnemiesDefeated();
    }

    private void CallStaticEventHandler_OnPointScored(PointScoreArgs pointScoreArgs)
    {
        gameScore += pointScoreArgs.Points * scoreMultiplier;

        StaticEventHandler.CallScoreChangedEvent(gameScore, scoreMultiplier);
    }

    private void CallStaticEventHandler_OnMultiplier(MultiplierArgs multiplierArgs)
    {
        scoreMultiplier = multiplierArgs.Multiplier
            ? scoreMultiplier + 1
            : scoreMultiplier - 1;

        scoreMultiplier = Mathf.Clamp(scoreMultiplier, 1, 30);

        StaticEventHandler.CallScoreChangedEvent(gameScore, scoreMultiplier);
    }

    private void CallPlayer_OnDestroyed(DestroyedEvent destroyedEvent, DestoryedEventArgs destoryedEventArgs)
    {
        previousGameState = gameState;
        gameState = GameState.gameLost;
    }

    private void Start()
    {
        previousGameState = GameState.gameStarted;
        gameState = GameState.gameStarted;

        gameScore = 0;

        scoreMultiplier = 1;

        StartCoroutine(Fade(0f, 1f, 0f, Color.black));
    }

    private void Update()
    {
        HandleGameState();

        if (Input.GetKeyDown(KeyCode.N))
        {
            gameState = GameState.gameStarted;
        }
    }

    private void HandleGameState()
    {
        Dictionary<GameState, Action> stateActions = new Dictionary<GameState, Action>();

        stateActions[GameState.gameStarted] = () =>
        {
            PlayDungeonLevel(currentDungeonLevelListIndex);
            gameState = GameState.playingLevel;
            RoomEnemiesDefeated();
        };

        stateActions[GameState.playingLevel] = () =>
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                DisplayDungeonOverViewMap();
            }
        };

        stateActions[GameState.dungeonOverviewMap] = () =>
        {
            if (Input.GetKeyUp(KeyCode.Tab))
            {
                DungeonMap.Instance.ClearDungeonOverViewMap();
            }
        };

        stateActions[GameState.bossStage] = () =>
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                DisplayDungeonOverViewMap();
            }
        };

        stateActions[GameState.levelCompleted] = () =>
        {
            StartCoroutine(LevelCompleted());
        };

        stateActions[GameState.gameWon] = () =>
        {
            if (previousGameState != GameState.gameWon)
                StartCoroutine(GameWon());
        };

        stateActions[GameState.gameLost] = () =>
        {
            if (previousGameState != GameState.gameLost)
            {
                StopAllCoroutines();
                StartCoroutine(GameLost());
            }
        };

        stateActions[GameState.restartGame] = () =>
        {
            RestartGame();
        };

        if (stateActions.TryGetValue(gameState, out var action))
        {
            action.Invoke();
        }
    }

    private void RoomEnemiesDefeated()
    {
        bool isDungeonClearOfRegularEnemies = true;
        bossRoom = null;

        foreach (KeyValuePair<string, Room> keyValuePair in DungeonBuilder.Instance.dungeonBuilderRoomDictionary)
        {
            if (keyValuePair.Value.roomNodeType.isBossRoom)
            {
                bossRoom = keyValuePair.Value.instantiatedRoom;
                continue;
            }

            if (!keyValuePair.Value.isClearedOfEnemies)
            {
                isDungeonClearOfRegularEnemies = false;
            }
        }

        bool isDungeonClearOfRegularEnemiesAndBossCleared =
            isDungeonClearOfRegularEnemies && (bossRoom == null || bossRoom.room.isClearedOfEnemies);

        gameState = isDungeonClearOfRegularEnemiesAndBossCleared
            ? (currentDungeonLevelListIndex < dungeonLevelList.Count - 1 
                ? GameState.levelCompleted 
                : GameState.gameWon)
            : (isDungeonClearOfRegularEnemies 
                ? GameState.bossStage 
                : gameState);

        if (gameState == GameState.bossStage)
        {
            StartCoroutine(BossStage());
        }
    }

    private void DisplayDungeonOverViewMap()
    {
        if (isFading)
            return;

        DungeonMap.Instance.DisplayDungeonOverViewMap();
    }

    private void PlayDungeonLevel(int dungeonLevelListIndex)
    {
        bool dungeonBuildSuccessfully = DungeonBuilder.Instance.GenerateDungeon(dungeonLevelList[dungeonLevelListIndex]);

        if (!dungeonBuildSuccessfully)
            Debug.Log("Couldn't build dungeon from specified rooms and node graphs");

        StaticEventHandler.CallRoomChangedEvent(currentRoom);

        player.gameObject.transform.position =
            new Vector3((currentRoom.lowerBounds.x + currentRoom.upperBounds.x) / 2f,
            (currentRoom.lowerBounds.y + currentRoom.upperBounds.y) / 2f, 0f);

        player.gameObject.transform.position = 
            HelperUtilities.GetSpawnPositionNearestToPlayer(player.gameObject.transform.position);

        StartCoroutine(DisplayDungeonLevelText());
    }

    private IEnumerator DisplayDungeonLevelText()
    {
        StartCoroutine(Fade(0f, 1f, 0f, Color.black));

        GetPlayer.PlayerControlReference.DisablePlayerMovement();

        string messageText = "LEVEL " + (currentDungeonLevelListIndex + 1).ToString() +
            "\n\n" + dungeonLevelList[currentDungeonLevelListIndex].levelName.ToUpper();

        yield return StartCoroutine(DisplayMessagerRoutine(messageText, Color.white, 3f));

        GetPlayer.PlayerControlReference.EnablePlayerMovement();

        yield return StartCoroutine(Fade(1f, 0f, 0.5f, Color.black));
    }

    private IEnumerator DisplayMessagerRoutine(string text, Color textColor, float displaySeconds)
    {
        messageText.SetText(text);
        messageText.color = textColor;

        float timer = displaySeconds > 0f ? displaySeconds : float.MaxValue;

        while (timer > 0f && !Input.GetKeyDown(KeyCode.Return))
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        yield return null;

        messageText.SetText(string.Empty);
    }

    private IEnumerator BossStage()
    {
        bossRoom.gameObject.SetActive(true);

        bossRoom.UnlockDoors(0f);

        yield return new WaitForSeconds(2f);

        yield return StartCoroutine(Fade(0f, 1f, 0.5f, new Color(0f,0f,0f, 0.4f)));

        string text = "WELL DONE " + GameResources.Instance.currentPlayer.playerName.ToUpper() +
            " YOU'VE SURVIVED... SO FAR.\n\nNOW FIND AND DEFEAT THE BOSS. DON'T DISAPPOINT ME!";

        yield return StartCoroutine(DisplayMessagerRoutine(text, Color.white, 5f));

        yield return StartCoroutine(Fade(1f, 0f, 0.5f, new Color(0f,0f,0f,0.4f)));
    }

    private IEnumerator LevelCompleted()
    {
        gameState = GameState.playingLevel;

        yield return new WaitForSeconds(2f);

        yield return StartCoroutine(Fade(0f, 1f, 0.5f, new Color(0f, 0f, 0f, 0.4f)));

        string text = "WELL DONE " + GameResources.Instance.currentPlayer.playerName.ToUpper() +
            "! \n\nYOU'VE SURVIVED THIS DUNGEON... BUT THIS ISN'T END YET.";

        yield return StartCoroutine(DisplayMessagerRoutine(text, Color.white, 5f));

        text = "COLLECT ANY LOOT... THEN PRESS \"ENTER\"\n\n TO DESCEND FURTHER INTO THE DUNGEON";

        yield return StartCoroutine(DisplayMessagerRoutine(text, Color.white, 5f));

        yield return StartCoroutine(Fade(1f, 0f, 0.5f, new Color(0f, 0f, 0f, 0.4f)));

        while (!Input.GetKeyDown(KeyCode.Return))
        {
            yield return null;
        }

        yield return null;

        currentDungeonLevelListIndex++;

        PlayDungeonLevel(currentDungeonLevelListIndex);
    }

    private IEnumerator GameWon()
    {
        previousGameState = GameState.gameWon;

        GetPlayer.PlayerControlReference.DisablePlayerMovement();

        yield return StartCoroutine(Fade(0f, 1f, 0.5f, Color.black));

        string text = "WELL DONE " + GameResources.Instance.currentPlayer.playerName.ToUpper() +
            "!\n\n YOU'VE SURVIVED IN AAAAALL MY DUNGEONS... I'M PROUD OF YOU!";

        yield return StartCoroutine(DisplayMessagerRoutine(text, Color.white, 5f));

        text = "YOU SCORED " + gameScore.ToString("###,###0");

        yield return StartCoroutine(DisplayMessagerRoutine(text, Color.white, 3f));

        text = "PRESS \"ENTER\" TO RESTART THE GAME";

        yield return StartCoroutine(DisplayMessagerRoutine(text, Color.white, 0f));

        gameState = GameState.restartGame;
    }

    private IEnumerator GameLost()
    {
        previousGameState = GameState.gameLost;

        GetPlayer.PlayerControlReference.DisablePlayerMovement();

        yield return StartCoroutine(Fade(0f, 1f, 0.5f, Color.black));

        Enemy[] enemyArray = GameObject.FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in enemyArray)
        {
            enemy.gameObject.SetActive(false);
        }

        string text = "BAD LUCK " + GameResources.Instance.currentPlayer.playerName.ToUpper() +
            "\n\n I SEE THAT YOU ARE NOT AS GOOD AS YOU THOUGHT.\n\n MAYBE NEXT TIME";

        yield return StartCoroutine(DisplayMessagerRoutine(text, Color.white, 5f));

        text = "YOU SCORED " + gameScore.ToString("###,###0");

        yield return StartCoroutine(DisplayMessagerRoutine(text, Color.white, 3f));

        text = "PRESS \"ENTER\" TO RESTART THE GAME";

        yield return StartCoroutine(DisplayMessagerRoutine(text, Color.white, 0f));

        gameState = GameState.restartGame;
    }

    private void RestartGame()
    {
        SceneManager.LoadScene(0);
    }

    public IEnumerator Fade(float startFadeAlpha, float targetFadeAlphs, float fadeSeconds, Color backgroundColor)
    {
        isFading = true;

        Image image = canvasGroup.GetComponent<Image>();
        image.color = backgroundColor;

        float time = 0f;

        while (time <= fadeSeconds)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startFadeAlpha, targetFadeAlphs, time / fadeSeconds);
            yield return null;
        }

        isFading = false;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(messageText), messageText);
        HelperUtilities.ValidateCheckNullValue(this, nameof(canvasGroup), canvasGroup);

        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(dungeonLevelList), dungeonLevelList);
    }
#endif
}
