using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
[DisallowMultipleComponent]
public class EnemyMovementAI : MonoBehaviour
{
    [SerializeField] private MovementDetailsSO _movementDetails;

    private Enemy _enemy;
    private Stack<Vector3> _movementSteps = new Stack<Vector3>();
    private Vector3 _playerReferencePosition;
    private Coroutine _moveEnemyRoutine;
    private float _currentEnemyPathRebuildCooldown;
    private WaitForFixedUpdate _waitForFixedUpdate;
    private bool _chasePlayer = false;

    [HideInInspector] public float MovementSpeed;

    private void Awake()
    {
        _enemy = GetComponent<Enemy>();

        MovementSpeed = _movementDetails.GetMovementSpeed;
    }

    private void Start()
    {
        _waitForFixedUpdate = new WaitForFixedUpdate();

        _playerReferencePosition = GameManager.Instance.GetPlayer.GetPlayerPosition;
    }

    private void Update()
    {
        MoveEnemy();
    }

    private void MoveEnemy()
    {
        _currentEnemyPathRebuildCooldown -= Time.deltaTime;

        float distanceToPlayer = (transform.position - GameManager.Instance.GetPlayer.GetPlayerPosition).magnitude;
        if(!_chasePlayer && distanceToPlayer < _enemy.EnemyDetails.ChaseDistance)
            _chasePlayer = true;

        if (!_chasePlayer)
            return;

        float distanceToRebuildPath = 
            (_playerReferencePosition - GameManager.Instance.GetPlayer.GetPlayerPosition).magnitude;
        if(_currentEnemyPathRebuildCooldown <= 0f || distanceToRebuildPath > Settings.playerMoveDistanceToRebuildPath)
        {
            _currentEnemyPathRebuildCooldown = Settings.enemyPathRebuildCooldown;

            _playerReferencePosition = GameManager.Instance.GetPlayer.GetPlayerPosition;

            CreatePath();

            if (_movementSteps == null)
                return;

            if (_moveEnemyRoutine != null)
            {
                _enemy.EnemyIdleEvent.CallIdleEvent();
                StopCoroutine(_moveEnemyRoutine);
            }

            //_moveEnemyRoutine = StartCoroutine(_moveEnemyRoutine(_movementSteps));          
        }
    }

    private void CreatePath()
    {
        Room currentRoom = GameManager.Instance.GetCurrentRoom;

        Grid grid = currentRoom.instantiatedRoom.grid;

        Vector3Int playerGridPosition = GetNearestNonObstaclePlayerPosition(currentRoom);

        Vector3Int enemyGridPosition = grid.WorldToCell(transform.position);

        _movementSteps = AStar.BuildPath(currentRoom, enemyGridPosition, playerGridPosition);

        if( _moveEnemyRoutine != null )
        {
            _movementSteps.Pop();
            return;
        }

        _enemy.EnemyIdleEvent.CallIdleEvent();
    }

    private Vector3Int GetNearestNonObstaclePlayerPosition(Room currentRoom)
    {
        return new Vector3Int(0, 0, 0);
    }
}
