using Cinemachine;
using System;
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
