using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Room_", menuName = "Scriptable Objects/Dungeon/Room")]
public class RoomTemplateSO : ScriptableObject
{
    [HideInInspector] public string guid;

    public GameObject prefab;

    //this is used to regenarate the guid if the SO is copied adn the prefab is changed
    [HideInInspector] public GameObject previousPrefab;

    public RoomNodeTypeSO roomNodeType;
    public Vector2Int lowerBounds;
    public Vector2Int upperBounds;

    [SerializeField] public List<Doorway> doorwayList;

    public Vector2Int[] spawnPositionArray;

    public List<Doorway> GetDoorwayList => doorwayList;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if(guid == "" || previousPrefab != prefab)
        {
            guid = GUID.Generate().ToString();
            previousPrefab = prefab;
            EditorUtility.SetDirty(this);
        }

        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(doorwayList), doorwayList);

        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(spawnPositionArray), spawnPositionArray);
    }
#endif
}
