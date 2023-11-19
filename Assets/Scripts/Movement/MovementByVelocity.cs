using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent (typeof(MovementByVelocityEvent))]
[DisallowMultipleComponent]
public class MovementByVelocity : MonoBehaviour
{
    private Rigidbody2D _rigidbody;
    private MovementByVelocityEvent _movementByVelocityEvent;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _movementByVelocityEvent = GetComponent<MovementByVelocityEvent>();
    }

    private void OnEnable()
    {
        _movementByVelocityEvent.OnMovementByVelocity += MovementByVelocityEvent_OnMovementByVelocity;
    }

    private void OnDisable()
    {
        _movementByVelocityEvent.OnMovementByVelocity -= MovementByVelocityEvent_OnMovementByVelocity;
    }

    private void MovementByVelocityEvent_OnMovementByVelocity(
        MovementByVelocityEvent movementByVelocityEvent, MovementByVelocityArgs movementByVelocityArgs)
    {
        MoveRigidbody(movementByVelocityArgs.moveDirection, movementByVelocityArgs.moveSpeed);
    }

    private void MoveRigidbody(Vector2 moveDirection, float moveSpeed)
    {
        _rigidbody.velocity = moveDirection * moveSpeed;
    }
}
