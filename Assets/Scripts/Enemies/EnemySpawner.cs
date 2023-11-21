using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class EnemySpawner : SingletonMonobehaviour<EnemySpawner>
{
    private int _enemiesToSpawn;
    private int _currentEnemyCount;
    private int _enemiesSpawnedSoFar;
    private int _enemiesMaxConcurrentSpawnNumber;
    private Room _currentRoom;
    private RoomEnemySpawnParametrs _roomEnemySpawnParametrs;

    private void OnEnable()
    {
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomCganged;
    }

    private void OnDisable()
    {
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomCganged;
    }

    private void StaticEventHandler_OnRoomCganged(RoomChangedEventArgs roomChangedEventArgs)
    {
        _enemiesSpawnedSoFar = 0;
        _currentEnemyCount = 0;

        _currentRoom = roomChangedEventArgs.Room;

        if (_currentRoom.roomNodeType.isCorridorEW ||
           _currentRoom.roomNodeType.isCorridorNS ||
           _currentRoom.roomNodeType.isEntrance)
            return;

        if (_currentRoom.isClearedOfEnemies)
            return;

        _enemiesToSpawn = _currentRoom.GetNumberOfEnemiesToSpawn(GameManager.Instance.GetCurrentDungeonLevel);

        _roomEnemySpawnParametrs = _currentRoom.GetRoomEnenySpawnParametrs(GameManager
            .Instance.GetCurrentDungeonLevel);

        if(_enemiesToSpawn == 0)
        {
            _currentRoom.isClearedOfEnemies = true;

            return;
        }

        _enemiesMaxConcurrentSpawnNumber = GetConcurrentEnemies();

        _currentRoom.instantiatedRoom.LockDoors();

        SpawnEnemies();
    }

    private void SpawnEnemies()
    {
        if(GameManager.Instance.gameState == GameState.playingLevel)
        {
            GameManager.Instance.previousGameState = GameState.playingLevel;
            GameManager.Instance.gameState = GameState.engagingEnemies;
        }

        StartCoroutine(SpawnEnemiesRoutine());
    }

    private IEnumerator SpawnEnemiesRoutine()
    {
        Grid grid = _currentRoom.instantiatedRoom.grid;

        RandomSpawnableObject<EnemyDetailsSO> randomEnemyHelperClass =
            new RandomSpawnableObject<EnemyDetailsSO>(_currentRoom.enemiesbyLevelList);

        if(_currentRoom.spawnPositionArray.Length > 0)
        {
            for (int i = 0; i < _enemiesToSpawn; i++)
            {
                while(_currentEnemyCount >= _enemiesMaxConcurrentSpawnNumber)
                {
                    yield return null;
                }

                Vector3Int cellPosition = (Vector3Int)_currentRoom.spawnPositionArray[Random
                    .Range(0, _currentRoom.spawnPositionArray.Length)];

                CreateEnemy(randomEnemyHelperClass.GetItem(), grid.CellToWorld(cellPosition));
                Debug.Log("CreateEnemy");

                yield return new WaitForSeconds(GetEnemySpawnInterval());
            }
        }
    }

    private float GetEnemySpawnInterval()
    {
        return Random.Range(_roomEnemySpawnParametrs.MinSpawnInterval,
            _roomEnemySpawnParametrs.MaxSpawnInterval);
    }

    private int GetConcurrentEnemies()
    {
        return Random.Range(_roomEnemySpawnParametrs.MinCocncurrentEnemies,
            _roomEnemySpawnParametrs.MaxCocncurrentEnemies);
    }

    private void CreateEnemy(EnemyDetailsSO enemyDetails, Vector3 position)
    {
        _enemiesSpawnedSoFar++;

        _currentEnemyCount++;

        DungeonLevelSO dungeonLevel = GameManager.Instance.GetCurrentDungeonLevel;

        GameObject enemy = Instantiate(enemyDetails.EnemyPrefab,
            position, Quaternion.identity, transform);

        enemy.GetComponent<Enemy>().EnemyInitialization(enemyDetails, _enemiesSpawnedSoFar, dungeonLevel);
    }
}
