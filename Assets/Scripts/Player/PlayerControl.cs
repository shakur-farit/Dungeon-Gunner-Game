using System;
using System.Collections;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] private MovementDetailsSO movementDetails;
    [SerializeField] private Transform weaponShootPosition;

    private Player player;
    private float moveSpeed;
    private Coroutine playerRollCoroutine;
    private WaitForFixedUpdate waitForFixedUpdate;
    private bool isPlayerRolling = false;
    private float playerRollColldownTimer = 0f;

    private void Awake()
    {
        player = GetComponent<Player>();

        moveSpeed = movementDetails.GetMoveSpeed();
    }

    private void Start()
    {
        waitForFixedUpdate = new WaitForFixedUpdate();

        SetPlayerAnimationSpeed();
    }

    private void SetPlayerAnimationSpeed()
    {
        player.animator.speed = moveSpeed / Settings.baseSpeedForPlayerAnimations;
    }

    private void Update()
    {
        if (isPlayerRolling)
            return;

        MovementInput();

        WeaponInput();

        PlayerRollCooldownTimer();
    }

    private void MovementInput()
    {
        float horizontalMovment = Input.GetAxisRaw("Horizontal");
        float verticalMovement = Input.GetAxisRaw("Vertical");
        bool rightMouseButton = Input.GetMouseButtonDown(1);

        Vector2 direction = new Vector2(horizontalMovment, verticalMovement);

        if(horizontalMovment != 0 && verticalMovement != 0)
            direction *= .7f;

        if (direction != Vector2.zero)
        {
            if(!rightMouseButton)
                player.movementByVelocityEvent.CallMovementbyVelocityEvent(direction, moveSpeed);
            else if(playerRollColldownTimer <= 0f)
                PlayerRoll((Vector3)direction);
        }
        else
            player.idleEvent.CallIdleEvent();
    }

    private void PlayerRoll(Vector3 direction)
    {
        playerRollCoroutine = StartCoroutine(PlayerRollRoutine(direction));
    }

    private IEnumerator PlayerRollRoutine(Vector3 direction)
    {
        float minDistance = 0.2f;

        isPlayerRolling = true;

        Vector3 targetPosition = player.transform.position + (Vector3)direction * movementDetails.rollDistance;

        while(Vector3.Distance(player.transform.position, targetPosition) > minDistance)
        {
            player.movementToPositionEvent.CallMovementToPositionEvent(targetPosition, player.transform.position,
                movementDetails.rollSpeed, direction, isPlayerRolling);

            yield return waitForFixedUpdate;
        }

        isPlayerRolling = false;

        playerRollColldownTimer = movementDetails.rollCooldownTime;

        player.transform.position = targetPosition;
    }

    private void PlayerRollCooldownTimer()
    {
        if(playerRollColldownTimer >= 0f)
            playerRollColldownTimer -= Time.deltaTime;
    }

    private void WeaponInput()
    {
        Vector3 weaponDirection;
        float weaponAngleDegress, playerAngleDegress;
        AimDirection playerAimDirection;

        AimWeaponInput(out weaponDirection, out weaponAngleDegress, out playerAngleDegress, out playerAimDirection);
    }

    private void AimWeaponInput(out Vector3 weaponDirection, out float weaponAngleDegress,
        out float playerAngleDegress, out AimDirection playerAimDirection)
    {
        Vector3 mouseWorldPosition = HelperUtilities.GetMouseWorldPosition();

        weaponDirection = (mouseWorldPosition - weaponShootPosition.position);

        Vector3 playerDirection = (mouseWorldPosition - transform.position);

        weaponAngleDegress = HelperUtilities.GetAngelFromVector(weaponDirection);
        playerAngleDegress = HelperUtilities.GetAngelFromVector(playerDirection);

        playerAimDirection = HelperUtilities.GetAimDerection(playerAngleDegress);

        player.aimWeaponEvent.CallAimWeaponEvent(playerAimDirection, playerAngleDegress, weaponAngleDegress, weaponDirection);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        StopPlayerRollRoutine();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        StopPlayerRollRoutine();
    }

    private void StopPlayerRollRoutine()
    {
        if(playerRollCoroutine != null)
        {
            StopCoroutine(playerRollCoroutine);

            isPlayerRolling = false;
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(movementDetails), movementDetails);
    }
#endif
}
