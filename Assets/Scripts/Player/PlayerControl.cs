using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] private MovementDetailsSO movementDetails;
    [SerializeField] private Transform weaponShootPosition;

    private Player player;
    private float moveSpeed;

    private void Awake()
    {
        player = GetComponent<Player>();

        moveSpeed = movementDetails.GetMoveSpeed();
    }

    private void Update()
    {
        MovementInput();

        WeaponInput();
    }

    private void MovementInput()
    {
        float horizontalMovment = Input.GetAxisRaw("Horizontal");
        float verticalMovement = Input.GetAxisRaw("Vertical");

        Vector2 direction = new Vector2(horizontalMovment, verticalMovement);

        if(horizontalMovment != 0 && verticalMovement != 0)
            direction *= .7f;

        if (direction != Vector2.zero)
            player.movementByVelocityEvent.CallMovementbyVelocityEvent(direction, moveSpeed);
        else
            player.idleEvent.CallIdleEvent();
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

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(movementDetails), movementDetails);
    }
#endif
}
