using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Room_", menuName = "Scriptable Objects/Dungeon/Room")]
public class RoomTemplateSO : ScriptableObject
{
    [HideInInspector] public string guid;

    [Header("Room Prefab")]
    public GameObject prefab;

    [HideInInspector] public GameObject previousPrefab; // this is used to regenerate the
                                                        // guid if the so is copied and the prefab is changed

    [Header("Room Music")]
    public MusicTrackSO battleMusic;
    public MusicTrackSO ambientMusic;

    [Header("Room Configuration")]
    public RoomNodeTypeSO roomNodeType;
    public Vector2Int lowerBounds;
    public Vector2Int upperBounds;

    [SerializeField] public List<Doorway> doorwayList;

    public Vector2Int[] spawnPositionArray;

    [Header("Enemy Deatils")]
    public List<SpawnableObjectsByLevel<EnemyDetailsSO>> enemiesByLevelList;
    public List<RoomEnemySpawnParametrs> roomEnemySpawnParametrsList;

    public List<Doorway> GetDoorwayList => doorwayList;

#if UNITY_EDITOR

    private void OnValidate()
    {
        if (guid == "" || previousPrefab != prefab)
        {
            guid = GUID.Generate().ToString();
            previousPrefab = prefab;
            EditorUtility.SetDirty(this);
        }

        HelperUtilities.ValidateCheckNullValue(this, nameof(prefab), prefab);
        HelperUtilities.ValidateCheckNullValue(this, nameof(battleMusic), battleMusic);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ambientMusic), ambientMusic);
        HelperUtilities.ValidateCheckNullValue(this, nameof(roomNodeType), roomNodeType);

        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(doorwayList), doorwayList);

        if(enemiesByLevelList.Count > 0 || roomEnemySpawnParametrsList.Count > 0)
        {
            HelperUtilities.ValidateCheckEnumerableValues(this, nameof(enemiesByLevelList), enemiesByLevelList);
            HelperUtilities.ValidateCheckEnumerableValues(this, nameof(roomEnemySpawnParametrsList), 
                roomEnemySpawnParametrsList);

            foreach (RoomEnemySpawnParametrs roomEnemySpawnParametrs in roomEnemySpawnParametrsList)
            {
                HelperUtilities.ValidateCheckNullValue(this, nameof(roomEnemySpawnParametrs.DungeonLevel),
                    roomEnemySpawnParametrs.DungeonLevel);
                HelperUtilities.ValidateCheckPositiveRange(this, nameof(roomEnemySpawnParametrs.MinTotalEnemiesToSpawn),
                    roomEnemySpawnParametrs.MinTotalEnemiesToSpawn, nameof(roomEnemySpawnParametrs.MaxTotalEnemiesToSpawn),
                    roomEnemySpawnParametrs.MaxTotalEnemiesToSpawn, false);
                HelperUtilities.ValidateCheckPositiveRange(this, nameof(roomEnemySpawnParametrs.MinCocncurrentEnemies),
                     roomEnemySpawnParametrs.MinCocncurrentEnemies, nameof(roomEnemySpawnParametrs.MaxCocncurrentEnemies),
                    roomEnemySpawnParametrs.MaxCocncurrentEnemies, false);
                HelperUtilities.ValidateCheckPositiveRange(this, nameof(roomEnemySpawnParametrs.MinSpawnInterval),
                    roomEnemySpawnParametrs.MinSpawnInterval, nameof(roomEnemySpawnParametrs.MaxSpawnInterval),
                    roomEnemySpawnParametrs.MaxSpawnInterval, false);

                bool isEnemyTypeListForDungeonLevel = false;
                foreach (SpawnableObjectsByLevel<EnemyDetailsSO> dungeonObjectByLevel in enemiesByLevelList)
                {
                    if (dungeonObjectByLevel.DungeonLevel == roomEnemySpawnParametrs.DungeonLevel &&
                        dungeonObjectByLevel.SpawnableObjectRatioList.Count > 0)
                        isEnemyTypeListForDungeonLevel = true;

                    HelperUtilities.ValidateCheckNullValue(this, nameof(dungeonObjectByLevel.DungeonLevel),
                        dungeonObjectByLevel.DungeonLevel);

                    foreach (SpawnableObjectRatio<EnemyDetailsSO> dungeonObjectRatio in dungeonObjectByLevel
                        .SpawnableObjectRatioList)
                    {
                        HelperUtilities.ValidateCheckNullValue(this, nameof(dungeonObjectRatio.DungeonObject),
                            dungeonObjectRatio.DungeonObject);

                        HelperUtilities.ValidateCheckPositiveValue(this, nameof(dungeonObjectRatio.Ratio),
                            dungeonObjectRatio.Ratio, false);
                    }
                }

                if(!isEnemyTypeListForDungeonLevel && roomEnemySpawnParametrs.DungeonLevel != null)
                {
                    Debug.Log("No enemy types specified in dor dungeon level " +
                        roomEnemySpawnParametrs.DungeonLevel.levelName + " in gameobjecy" + this.name.ToString());
                }
            }
        }

        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(spawnPositionArray), spawnPositionArray);
    }

#endif
}