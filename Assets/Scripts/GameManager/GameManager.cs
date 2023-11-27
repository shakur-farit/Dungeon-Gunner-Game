using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class GameManager : SingletonMonobehaviour<GameManager>
{
    [SerializeField] private List<DungeonLevelSO> dungeonLevelList;
    [SerializeField] private int currentDungeonLevelListIndex = 0;

    private Room currentRoom;
    private Room previousRoom;
    private PlayerDetailsSO playerDetails;
    private Player player;
    private long gameScore;
    private int scoreMultiplier;
    private InstantiatedRoom bossRoom;

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
        //switch (gameState)
        //{
        //    case GameState.gameStarted:
        //        PlayDungeonLevel(currentDungeonLevelListIndex);
        //        gameState = GameState.playingLevel;
        //        break;
        //}

        if (gameState == GameState.gameStarted)
        {
            PlayDungeonLevel(currentDungeonLevelListIndex);
            gameState = GameState.playingLevel;
            RoomEnemiesDefeated();
            return;
        }

        if(gameState == GameState.levelCompleted)
        {
            StartCoroutine(LevelCompleted());
            return;
        }

        if(gameState == GameState.gameWon)
        {
            if (previousGameState != GameState.gameWon)
                StartCoroutine(GameWon());
            return;
        }

        if(gameState == GameState.gameLost)
        {
            if(previousGameState != GameState.gameLost)
            {

                StopAllCoroutines();
                StartCoroutine(GameLost());
            }

            return;
        }

        if(gameState == GameState.restartGame)
        {
            RestartGame();
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
                continue;
            }
        }

        if ((isDungeonClearOfRegularEnemies && bossRoom == null) ||
            (isDungeonClearOfRegularEnemies && bossRoom.room.isClearedOfEnemies))
        {
            gameState = currentDungeonLevelListIndex < dungeonLevelList.Count - 1
                ? GameState.levelCompleted
                : GameState.gameWon;
            return;
        }

        if (isDungeonClearOfRegularEnemies)
        {
            gameState = GameState.bossStage;

            StartCoroutine(BossStage());
        }
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
    }

    private IEnumerator BossStage()
    {
        bossRoom.gameObject.SetActive(true);

        bossRoom.UnlockDoors(0f);

        yield return new WaitForSeconds(2f);

        Debug.Log("Boss stage - find and destroy the boss");
    }

    private IEnumerator LevelCompleted()
    {
        gameState = GameState.playingLevel;

        yield return new WaitForSeconds(2f);

        Debug.Log("Level Complete - Press return To progress to the next level");

        while (!Input.GetKeyDown(KeyCode.Return))
        {
            yield return null;
        }

        yield return null;

        currentDungeonLevelListIndex++;

        Debug.Log(currentDungeonLevelListIndex);

        PlayDungeonLevel(currentDungeonLevelListIndex);
    }

    private IEnumerator GameWon()
    {
        previousGameState = GameState.gameWon;

        Debug.Log("Game won! - All levels complieted and bosses defaeted. " +
            "Game will restart in 10 seconds");

        yield return new WaitForSeconds(10f);

        gameState = GameState.restartGame;
    }

    private IEnumerator GameLost()
    {
        previousGameState = GameState.gameLost;

        Debug.Log("Game lost - Bad luck! Game will restart in 10 seconds");

        yield return new WaitForSeconds(10f);

        gameState = GameState.restartGame;
    }

    private void RestartGame()
    {
        SceneManager.LoadScene(0);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(dungeonLevelList), dungeonLevelList);
    }
#endif
}
