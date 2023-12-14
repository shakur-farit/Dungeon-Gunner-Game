using Cinemachine;
using System.Collections;
using UnityEngine;

public class DungeonMap : SingletonMonobehaviour<DungeonMap>
{
    [Header("Game Object References")]
    [SerializeField] private GameObject _minimapUI;

    private Camera _dungeonMapCamera;
    private Camera _mainCamera;

    private void Start()
    {
        _mainCamera = Camera.main;

        Transform playerTransform = GameManager.Instance.GetPlayer.transform;

        CinemachineVirtualCamera cinemachineVirtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        cinemachineVirtualCamera.Follow = playerTransform;

        _dungeonMapCamera = GetComponentInChildren<Camera>();
        _dungeonMapCamera.gameObject.SetActive(false);
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0) && GameManager.Instance.gameState == GameState.dungeonOverviewMap)
        {
            GetRoomClicked();
        }
    }

    private void GetRoomClicked()
    {
        Vector3 worldPosition = _dungeonMapCamera.ScreenToWorldPoint(Input.mousePosition);
        worldPosition = new Vector3(worldPosition.x, worldPosition.y, 0);

        Collider2D[] collidersArray = Physics2D.OverlapCircleAll(new Vector2(worldPosition.x, worldPosition.y), 1f);

        foreach (var collider in collidersArray)
        {
            if(collider.GetComponent<InstantiatedRoom>() != null)
            {
                InstantiatedRoom instantiatedRoom = collider.GetComponent<InstantiatedRoom>();

                if(instantiatedRoom.room.isClearedOfEnemies && instantiatedRoom.room.isPreviouslyVisited)
                {
                    StartCoroutine(MovePlayerToRoom(worldPosition, instantiatedRoom.room));
                }
            }
        }
    }

    private IEnumerator MovePlayerToRoom(Vector3 worldPosition, Room room)
    {
        StaticEventHandler.CallRoomChangedEvent(room);

        yield return StartCoroutine(GameManager.Instance.Fade(0f,1f,0f, Color.black));

        ClearDungeonOverViewMap();

        GameManager.Instance.GetPlayer.PlayerControlReference.DisablePlayerMovement();

        Vector3 spawnPosition = HelperUtilities.GetSpawnPositionNearestToPlayer(worldPosition);

        GameManager.Instance.GetPlayer.transform.position = spawnPosition;

        yield return StartCoroutine(GameManager.Instance.Fade(1f, 0f, 1f, Color.black));

        GameManager.Instance.GetPlayer.PlayerControlReference.EnablePlayerMovement();
    }

    public void DisplayDungeonOverViewMap()
    {
        GameManager.Instance.previousGameState = GameManager.Instance.gameState;
        GameManager.Instance.gameState = GameState.dungeonOverviewMap;

        GameManager.Instance.GetPlayer.PlayerControlReference.DisablePlayerMovement();

        _mainCamera.gameObject.SetActive(false);
        _dungeonMapCamera.gameObject.SetActive(true);

        ActivateRoomsForDisplay();

        _minimapUI.SetActive(false);
    }

    public void ClearDungeonOverViewMap()
    {
        GameManager.Instance.gameState = GameManager.Instance.previousGameState;
        GameManager.Instance.previousGameState = GameState.dungeonOverviewMap;

        GameManager.Instance.GetPlayer.PlayerControlReference.EnablePlayerMovement();

        _mainCamera.gameObject.SetActive(true);
        _dungeonMapCamera.gameObject.SetActive(false);

        _minimapUI.SetActive(true);
    }

    private void ActivateRoomsForDisplay()
    {
        foreach (var keyValuePair in DungeonBuilder.Instance.dungeonBuilderRoomDictionary)
        {
            Room room = keyValuePair.Value;

            room.instantiatedRoom.gameObject.SetActive(true);
        }
    }
}
