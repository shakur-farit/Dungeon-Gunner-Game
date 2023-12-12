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
                GetAmmoPrecentToSpawn(ammoNumber));
            return;
        }

        chest.Initialize(true, GetHealthPercentToSpawn(healthNumber),
            GetWeaponPrecentToSpawn(weaponNumber),
            GetAmmoPrecentToSpawn(ammoNumber));
    }

    private bool RandomSpawnChest()
    {
        int chancePrecent = Random.Range(_chestSpawnChanceMin, _chestSpawnChanceMax + 1);

        foreach (RangeByLevel rangeByLevel in _chestSpawnChanceByLevelList)
        {
            if(rangeByLevel.DungeonLevel == GameManager.Instance.GetCurrentDungeonLevel)
            {
                chancePrecent = Random.Range(rangeByLevel.Min, rangeByLevel.Max + 1);
                break;
            }
        }

        int randomPrecent = Random.Range(1, 100 + 1);

        if (randomPrecent <= chancePrecent)
            return true;

        return false;
    }

    private void GetItemsToSpawn(out int ammoNumber, out int healthNumber, out int weaponNumber)
    {
        ammoNumber = 0;
        healthNumber = 0;
        weaponNumber = 0;

        int numberOfItemsToSpawn = Random.Range(_numberOfItemsToSpawnMin, _numberOfItemsToSpawnMax + 1);

        int choice;

        if(numberOfItemsToSpawn == 1)
        {
            choice = Random.Range(0, 3);

            if (choice == 0)
            {
                weaponNumber++;
                return;
            }

            if (choice == 1)
            {
                ammoNumber++;
                return;
            }

            if (choice == 2)
            {
                healthNumber++;
                return;
            }

            return;
        }
        else if (numberOfItemsToSpawn == 2)
        {
            choice = Random.Range(0, 3);

            if (choice == 0)
            {
                weaponNumber++;
                ammoNumber++;
                return;
            }

            if (choice == 1)
            {
                ammoNumber++;
                healthNumber++;
                return;
            }

            if (choice == 2)
            {
                healthNumber++;
                weaponNumber++;
                return;
            }

            return;
        }
        else if (numberOfItemsToSpawn >= 3)
        {
            weaponNumber++;
            ammoNumber++;
            healthNumber++;
            return;
        }
    }

    private WeaponDetailsSO GetWeaponPrecentToSpawn(int weaponNumber)
    {
        if (weaponNumber == 0)
            return null;

        var weaponRandom = new RandomSpawnableObject<WeaponDetailsSO>(_weaponSpawnByLevelList);

        WeaponDetailsSO weaponDetails = weaponRandom.GetItem();

        return weaponDetails;
    }

    private int GetHealthPercentToSpawn(int healthNumber)
    {
        return GetPrecentToSpawn(healthNumber, _healthSpawnByLevelList);
    }

    private int GetAmmoPrecentToSpawn(int ammoNumber)
    {
        return GetPrecentToSpawn(ammoNumber, _ammoSpawnByLevelList);
    }

    private int GetPrecentToSpawn(int number, List<RangeByLevel> rangeByLevelList)
    {
        if (number == 0)
            return default;

        foreach (RangeByLevel rangeByLevel in rangeByLevelList)
        {
            if (rangeByLevel.DungeonLevel == GameManager.Instance.GetCurrentDungeonLevel)
                return Random.Range(rangeByLevel.Min, rangeByLevel.Max);
        }

        return default;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(_chestPrefab), _chestPrefab);
        HelperUtilities.ValidateCheckPositiveRange(this, 
            nameof(_chestSpawnChanceMin), _chestSpawnChanceMin,
            nameof(_chestSpawnChanceMax), _chestSpawnChanceMax, 
            true);

        if(_chestSpawnChanceByLevelList != null && _chestSpawnChanceByLevelList.Count > 0)
        {
            HelperUtilities.ValidateCheckEnumerableValues(this, nameof(_chestSpawnChanceByLevelList),
                _chestSpawnChanceByLevelList);

            foreach (RangeByLevel rangeByLevel in _chestSpawnChanceByLevelList)
            {
                HelperUtilities.ValidateCheckNullValue(this, nameof(rangeByLevel.DungeonLevel), rangeByLevel.DungeonLevel);
                HelperUtilities.ValidateCheckPositiveRange(this,
                    nameof(rangeByLevel.Min), rangeByLevel.Min,
                    nameof(rangeByLevel.Max), rangeByLevel.Max,
                    true);
            }
        }

        HelperUtilities.ValidateCheckPositiveRange(this,
            nameof(_numberOfItemsToSpawnMin), _numberOfItemsToSpawnMin,
            nameof(_numberOfItemsToSpawnMax), _numberOfItemsToSpawnMax,
            true);

        if(_weaponSpawnByLevelList != null && _weaponSpawnByLevelList.Count > 0)
        {
            foreach (var weaponDeatails in _weaponSpawnByLevelList)
            {
                HelperUtilities.ValidateCheckNullValue(this, nameof(weaponDeatails.DungeonLevel),
                    weaponDeatails.DungeonLevel);

                foreach (var weaponRatio in weaponDeatails.SpawnableObjectRatioList)
                {
                    HelperUtilities.ValidateCheckNullValue(this, nameof(weaponRatio.DungeonObject), 
                        weaponRatio.DungeonObject);

                    HelperUtilities.ValidateCheckPositiveValue(this, nameof(weaponRatio.Ratio), weaponRatio.Ratio, true);
                }
            }
        }

        if(_healthSpawnByLevelList != null && _healthSpawnByLevelList.Count > 0)
        {
            HelperUtilities.ValidateCheckEnumerableValues(this, nameof(_healthSpawnByLevelList), _healthSpawnByLevelList);

            foreach (var rangeByLevel in _healthSpawnByLevelList)
            {
                HelperUtilities.ValidateCheckNullValue(this, nameof(rangeByLevel.DungeonLevel), rangeByLevel.DungeonLevel);
                HelperUtilities.ValidateCheckPositiveRange(this,
                    nameof(rangeByLevel.Min), rangeByLevel.Min,
                    nameof(rangeByLevel.Max), rangeByLevel.Max,
                    true);
            }
        }

        if(_ammoSpawnByLevelList != null && _ammoSpawnByLevelList.Count > 0)
        {
            HelperUtilities.ValidateCheckEnumerableValues(this,
                nameof(_ammoSpawnByLevelList), _ammoSpawnByLevelList);

            foreach (var rangeByLevel in _ammoSpawnByLevelList)
            {
                HelperUtilities.ValidateCheckNullValue(this, nameof(rangeByLevel.DungeonLevel), rangeByLevel.DungeonLevel);
                HelperUtilities.ValidateCheckPositiveRange(this,
                    nameof(rangeByLevel.Min), rangeByLevel.Min,
                    nameof(rangeByLevel.Max), rangeByLevel.Max,
                    true);
            }
        }
    }
#endif
}
