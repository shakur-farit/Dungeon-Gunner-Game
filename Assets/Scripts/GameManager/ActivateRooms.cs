using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class ActivateRooms : MonoBehaviour
{
    [Header("Populate With Minimap Camera")]
    [SerializeField] private Camera _miniMapCamaera;

    private void Start()
    {
        InvokeRepeating("EnableRooms", 0.5f, 0.75f);
    }

    private void EnableRooms()
    {
        foreach (KeyValuePair<string, Room> keyValuePair in
            DungeonBuilder.Instance.dungeonBuilderRoomDictionary)
        {
            Room room = keyValuePair.Value;

            HelperUtilities
                .CameraWorldPositionBounds(out Vector2Int miniMapCameraWorldPositionLowerBounds,
                out Vector2Int miniMapCameraWorldPositionUpperBounds, _miniMapCamaera);

            if((room.lowerBounds.x <= miniMapCameraWorldPositionUpperBounds.x &&
                room.lowerBounds.y <= miniMapCameraWorldPositionUpperBounds.y) &&
                (room.upperBounds.x >= miniMapCameraWorldPositionLowerBounds.x &&
                room.upperBounds.y >= miniMapCameraWorldPositionLowerBounds.y))
            {
                room.instantiatedRoom.gameObject.SetActive(true);
                continue;
            }

            room.instantiatedRoom.gameObject.SetActive(false);
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(_miniMapCamaera), _miniMapCamaera);
    }
#endif
}
