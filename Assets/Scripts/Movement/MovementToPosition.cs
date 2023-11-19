using UnityEngine;

[RequireComponent (typeof(Rigidbody2D))]
[RequireComponent(typeof(MovementToPositionEvent))]
[DisallowMultipleComponent]
public class MovementToPosition : MonoBehaviour
{
    private Rigidbody2D _rigidbody;
    private MovementToPositionEvent _movementToPositionEvent;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _movementToPositionEvent = GetComponent<MovementToPositionEvent>();
    }

    private void OnEnable()
    {
        _movementToPositionEvent.OnMovementToPosition += MovementRoPositionEvent_OnMovementToPosition;
    }

    private void OnDisable()
    {
        _movementToPositionEvent.OnMovementToPosition -= MovementRoPositionEvent_OnMovementToPosition;
    }

    private void MovementRoPositionEvent_OnMovementToPosition(MovementToPositionEvent movementToPositionEvent,
        MovementToPositionArgs movementToPositionArgs)
    {
        MoveRigidbody(movementToPositionArgs.movePosition, movementToPositionArgs.currentPosition,
            movementToPositionArgs.moveSpeed);
    }

    private void MoveRigidbody(Vector3 movePosition, Vector3 currentPosition, float moveSpeed)
    {
        Vector2 unitVector = Vector3.Normalize(movePosition - currentPosition);

        _rigidbody.MovePosition(_rigidbody.position + (unitVector * moveSpeed * Time.fixedDeltaTime));
    }
}
