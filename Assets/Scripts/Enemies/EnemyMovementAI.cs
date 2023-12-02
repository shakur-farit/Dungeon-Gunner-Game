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
    private List<Vector2Int> _surroundingPositionList = new List<Vector2Int>();

    [HideInInspector] public int UpdateFrameNumber = 1; // deafult value.
                                                        // This is set by the enemy spawner.
    [HideInInspector] public float MovementSpeed;

    public int SetUpdateFrameNumber
    {
        set
        {
            UpdateFrameNumber = value;
        }
    }

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

        if(Time.frameCount % Settings.targetFrameRateToSpreadPathfidingOver != UpdateFrameNumber) 
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

        int obstacle = Mathf.Min(currentRoom.instantiatedRoom.aStarMovementPenalty
            [
                adjustedPlayerCellPosition.x,
                adjustedPlayerCellPosition.y
            ],
            currentRoom.instantiatedRoom.aStarItemObstacles
            [
                adjustedPlayerCellPosition.x,
                adjustedPlayerCellPosition.y
            ]);

        if (obstacle != 0)
            return playerCellPosition;


        _surroundingPositionList.Clear();

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (j == 0 && i == 0)
                    continue;

                _surroundingPositionList.Add(new Vector2Int(i, j));
            }
        }

        for (int l = 0; l < 8; l++)
        {
            int index = Random.Range(0, _surroundingPositionList.Count);

            try
            {
                obstacle = Mathf.Min(currentRoom.instantiatedRoom.aStarMovementPenalty
                    [
                        adjustedPlayerCellPosition.x + _surroundingPositionList[index].x,
                        adjustedPlayerCellPosition.y + _surroundingPositionList[index].y
                    ],
                    currentRoom.instantiatedRoom.aStarItemObstacles
                    [
                        adjustedPlayerCellPosition.x + _surroundingPositionList[index].x,
                        adjustedPlayerCellPosition.y + _surroundingPositionList[index].y
                    ]);

                if(obstacle != 0)
                {
                    return new Vector3Int(playerCellPosition.x + _surroundingPositionList[index].x,
                        playerCellPosition.y + _surroundingPositionList[index].y, 0);
                }
            }
            catch
            {

            }

            _surroundingPositionList.RemoveAt(index);
        }

        return (Vector3Int)currentRoom.spawnPositionArray[Random.Range(0, currentRoom.spawnPositionArray.Length)];
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(_movementDetails), _movementDetails);
    }
#endif
}
