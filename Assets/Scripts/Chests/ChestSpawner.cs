using System.Collections.Generic;
using UnityEngine;

public class ChestSpawner : MonoBehaviour
{
    [System.Serializable]
    private struct RangeByLevel
    {
        public DungeonLevelSO DungeonLevel;
        [Range(0, 100)] public int Min;
        [Range(0, 100)] public int Max;
    }

    [Header("Chest Prefab")]
    [SerializeField] private GameObject _chestPrefab;

    [Header("Chest Spawn Chance")]
    [SerializeField][Range(0, 100)] private int _chestSpawnChanceMin; 
    [SerializeField][Range(0, 100)] private int _chestSpawnChanceMax;
    [SerializeField] private List<RangeByLevel> _chestSpawnChanceByLevelList;

    [Header("Chest Spawn Deatails")]
    [SerializeField] private ChestSpawnEvent _chestSpawnEvent;
    [SerializeField] private ChestSpawnPosition _chestSpawnPosition;
    [SerializeField][Range(0, 3)] private int _numberOfItemsToSpawnMin;
    [SerializeField][Range(0, 3)] private int _numberOfItemsToSpawnMax;

    [Header("Chest Content Details")]
    [SerializeField] private List<SpawnableObjectsByLevel<WeaponDetailsSO>> _weaponSpawnByLevelList;
    [SerializeField] private List<RangeByLevel> _healthSpawnByLevelList;
    [SerializeField] private List<RangeByLevel> _ammoSpawnByLevelList;

    private bool _chestSpawned = false;
    private Room _chestRoom;

    private void OnEnable()
    {
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;

        StaticEventHandler.OnRoomEnemiesDefeated += StaticEventHandler_OnRoomEnemiesDefated;
    }

    private void OnDisable()
    {   
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
     
        StaticEventHandler.OnRoomEnemiesDefeated -= StaticEventHandler_OnRoomEnemiesDefated;
    }

    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
    {
        if (_chestRoom == null)
            _chestRoom = GetComponentInParent<InstantiatedRoom>().room;

        if(!_chestSpawned && 
            _chestSpawnEvent == ChestSpawnEvent.onRoomEntry && 
            _chestRoom == roomChangedEventArgs.Room)
        {
            SpawnChest();
        }
    }

    private void StaticEventHandler_OnRoomEnemiesDefated(RoomEnemiesDefeatedArgs roomEnemiesDefeatedArgs)
    {
        if (_chestRoom == null)
            _chestRoom = GetComponentInParent<InstantiatedRoom>().room;

        if (!_chestSpawned &&
            _chestSpawnEvent == ChestSpawnEvent.onEnemiesDefeated &&
            _chestRoom == roomEnemiesDefeatedArgs.Room)
        {
            SpawnChest();
        }
    }

    private void SpawnChest()
    {
        _chestSpawned = true;

        if (!RandomSpawnChest())
            return;

        GetItemsToSpawn(out int ammoNumber, out int healthNumber, out int weaponNumber);

        GameObject chestGameObject = Instantiate(_chestPrefab, transform);

        if(_chestSpawnPosition == ChestSpawnPosition.atSpawnerPosition)
        {
            chestGameObject.transform.position = transform.position;
        }
        else if(_chestSpawnPosition == ChestSpawnPosition.atPlayerPosition)
        {
            Vector3 spawnPosition = HelperUtilities.GetSpawnPositionNearestToPlayer(GameManager
                .Instance.GetPlayer.transform.position);

            Vector3 variation = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);

            chestGameObject.transform.position = spawnPosition + variation;
        }

        Chest chest = chestGameObject.GetComponent<Chest>();

        if(_chestSpawnEvent == ChestSpawnEvent.onRoomEntry)
        {
            chest.Initialize(false, GetHealthPercentToSpawn(healthNumber),
                GetWeaponPrecentToSpawn(weaponNumber),
                GetAamoPrecentToSpawn(ammoNumber));
            return;
        }

        chest.Initialize(true, GetHealthPercentToSpawn(healthNumber),
            GetWeaponPrecentToSpawn(weaponNumber),
            GetAamoPrecentToSpawn(ammoNumber));
    }

    private bool RandomSpawnChest()
    {
        return false;
    }

    private void GetItemsToSpawn(out int ammoNumber, out int healthNumber, out int weaponNumber)
    {
        throw new System.NotImplementedException();
    }

    private int GetHealthPercentToSpawn(int healthNumber)
    {
        throw new System.NotImplementedException();
    }

    private WeaponDetailsSO GetWeaponPrecentToSpawn(int weaponNumber)
    {
        throw new System.NotImplementedException();
    }

    private int GetAamoPrecentToSpawn(int ammoNumber)
    {
        throw new System.NotImplementedException();
    }
}
