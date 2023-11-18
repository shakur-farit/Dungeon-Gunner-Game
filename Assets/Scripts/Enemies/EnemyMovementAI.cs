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
        if (!_chasePlayer && distanceToPlayer < _enemy.EnemyDetails.ChaseDistance)
            _chasePlayer = true;

        if (!_chasePlayer)
            return;

        float distanceToRebuildPath =
            (_playerReferencePosition - GameManager.Instance.GetPlayer.GetPlayerPosition).magnitude;
        if (_currentEnemyPathRebuildCooldown <= 0f || distanceToRebuildPath > Settings.playerMoveDistanceToRebuildPath)
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

            _moveEnemyRoutine = StartCoroutine(MoveEnemyRoutine(_movementSteps));
        }
    }

    private IEnumerator MoveEnemyRoutine(Stack<Vector3> movementSteps)
    {
        Debug.Log("Move Steps = " + movementSteps.Count);

        while (movementSteps.Count > 0)
        {
            Vector3 nextPosition = movementSteps.Pop();

            while ((nextPosition - transform.position).magnitude > 0.2f)
            {
                _enemy.EnemyMovementToPositionEvent
                    .CallMovementToPositionEvent(nextPosition,
                    transform.position, MovementSpeed,
                    (nextPosition - transform.position).normalized, false);

                yield return _waitForFixedUpdate;
            }

            yield return _waitForFixedUpdate;
        }

        _enemy.EnemyIdleEvent.CallIdleEvent();
    }

    private void CreatePath()
    {
        Room currentRoom = GameManager.Instance.GetCurrentRoom;

        Grid grid = currentRoom.instantiatedRoom.grid;

        Vector3Int playerGridPosition = GetNearestNonObstaclePlayerPosition(currentRoom);

        Vector3Int enemyGridPosition = grid.WorldToCell(transform.position);

        _movementSteps = AStar.BuildPath(currentRoom, enemyGridPosition, playerGridPosition);

        if (_movementSteps != null)
        {
            _movementSteps.Pop();
            return;
        }

        _enemy.EnemyIdleEvent.CallIdleEvent();
    }

    private Vector3Int GetNearestNonObstaclePlayerPosition(Room currentRoom)
    {
        Vector3 playerPosition = GameManager.Instance.GetPlayer.GetPlayerPosition;

        Vector3Int playerCellPosition =
            currentRoom.instantiatedRoom.grid.WorldToCell(playerPosition);

        Vector2Int adjustedPlayerCellPosition =
            new Vector2Int(playerCellPosition.x - currentRoom.templateLowerBounds.x,
            playerCellPosition.y - currentRoom.templateLowerBounds.y);

        int obstacle = currentRoom.instantiatedRoom
            .aStarMovementPenalty
            [
                adjustedPlayerCellPosition.x,
                adjustedPlayerCellPosition.y
            ];

        if (obstacle != 0)
            return playerCellPosition;

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (j == 0 && i == 0)
                    continue;

                try
                {
                    obstacle = currentRoom.instantiatedRoom
                        .aStarMovementPenalty
                        [
                            adjustedPlayerCellPosition.x + i,
                            adjustedPlayerCellPosition.y + j
                        ];

                    if (obstacle != 0)
                        return new Vector3Int(playerCellPosition.x + i,
                            playerCellPosition.y + j, 0);
                }
                catch
                {
                    continue;
                }
            }
        }

        return playerCellPosition;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(_movementDetails), _movementDetails);
    }
#endif
}
