using System.Collections.Generic;
using UnityEngine;

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

        StaticEventHandler.OnPointScored += CallStaticEventHandler_OnPointScored;

        StaticEventHandler.OnMultiplier += CallStaticEventHandler_OnMultiplier;
    }

    private void OnDisable()
    {
        StaticEventHandler.OnRoomChanged -= CallStaticEventHandler_OnRoomChanged;

        StaticEventHandler.OnPointScored -= CallStaticEventHandler_OnPointScored;

        StaticEventHandler.OnMultiplier -= CallStaticEventHandler_OnMultiplier;
    }

    private void CallStaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
    {
        SetCurrentRoom = roomChangedEventArgs.Room;
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

        if(gameState == GameState.gameStarted)
        {
            PlayDungeonLevel(currentDungeonLevelListIndex);
            gameState = GameState.playingLevel;
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

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(dungeonLevelList), dungeonLevelList);
    }
#endif
}
