using System;
using UnityEngine;

[RequireComponent (typeof(Rigidbody2D))]
[RequireComponent(typeof(MovementToPositionEvent))]
[DisallowMultipleComponent]
public class MovementToPosition : MonoBehaviour
{
    private Rigidbody2D rb;
    private MovementToPositionEvent movementToPositionEvent;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        movementToPositionEvent = GetComponent<MovementToPositionEvent>();
    }

    private void OnEnable()
    {
        movementToPositionEvent.OnMovementToPosition += MovementRoPositionEvent_OnMovementToPosition;
    }

    private void OnDisable()
    {
        movementToPositionEvent.OnMovementToPosition -= MovementRoPositionEvent_OnMovementToPosition;
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

        rb.MovePosition(rb.position + (unitVector * moveSpeed * Time.fixedDeltaTime));
    }
}
