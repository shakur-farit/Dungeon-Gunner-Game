using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Player))]
[DisallowMultipleComponent]
public class PlayerControl : MonoBehaviour
{
    [SerializeField] private MovementDetailsSO movementDetails;

    private Player player;
    private bool leftMouseDownPreviousFrame = false;
    private int currentWeaponIndex = 1;
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

        SetStartingWeapon();

        SetPlayerAnimationSpeed();
    }

    private void SetStartingWeapon()
    {
        int index = 1;

        foreach (Weapon weapon in player.weaponList)
        {
            if(weapon.WeaponDetails == player.playerDetails.startingWeapon)
            {
                SetWeaponByIndex(index);
                break;
            }
            index++;
        }
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

        Vector3 targetPosition = player.transform.position + 
            (Vector3)direction * movementDetails.rollDistance;

        while(Vector3.Distance(player.transform.position, targetPosition) > minDistance)
        {
            player.movementToPositionEvent.CallMovementToPositionEvent(targetPosition, 
                player.transform.position, movementDetails.rollSpeed, direction, isPlayerRolling);

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

        AimWeaponInput(out weaponDirection, out weaponAngleDegress, 
            out playerAngleDegress, out playerAimDirection);

        FireWeaponInput(weaponDirection, weaponAngleDegress, playerAngleDegress, playerAimDirection);

        ReloadWeaponInput();
    }

    private void AimWeaponInput(out Vector3 weaponDirection, out float weaponAngleDegress,
        out float playerAngleDegress, out AimDirection playerAimDirection)
    {
        Vector3 mouseWorldPosition = HelperUtilities.GetMouseWorldPosition();

        weaponDirection = (mouseWorldPosition - player.activeWeapon.GetShootPosition);

        Vector3 playerDirection = (mouseWorldPosition - transform.position);

        weaponAngleDegress = HelperUtilities.GetAngelFromVector(weaponDirection);
        playerAngleDegress = HelperUtilities.GetAngelFromVector(playerDirection);

        playerAimDirection = HelperUtilities.GetAimDerection(playerAngleDegress);

        player.aimWeaponEvent.CallAimWeaponEvent(playerAimDirection, playerAngleDegress, 
            weaponAngleDegress, weaponDirection);
    }

    private void FireWeaponInput(Vector3 weaponDirection, float weaponAngleDegress, 
        float playerAngleDegress, AimDirection playerAimDirection)
    {
        if (Input.GetMouseButton(0))
        {
            player.fireWeaponEvent.CallOnFireWeaponEvent(true, leftMouseDownPreviousFrame, playerAimDirection, 
                playerAngleDegress, weaponAngleDegress, weaponDirection);
            leftMouseDownPreviousFrame = true;
            return;
        }

        leftMouseDownPreviousFrame = false;
    }

    private void SetWeaponByIndex(int weaponIndex)
    {
        int index = weaponIndex - 1;

        if(index < player.weaponList.Count)
        {
            currentWeaponIndex = weaponIndex;
            player.setActiveWeaponEvent.CallSetActiveWeaponEvent(player.weaponList[index]);
        }
    }

    private void ReloadWeaponInput()
    {
        Weapon currentWeapon = player.activeWeapon.GetCurrentWeapon;

        if (currentWeapon.IsWeaponReloading)
            return;

        if (currentWeapon.WeaponRemainingAmmo < currentWeapon.WeaponDetails.weaponClipAmmoCapacity &&
            !currentWeapon.WeaponDetails.hasInfiniteAmmo)
            return;

        if (currentWeapon.WeaponClipRemainingAmmo == currentWeapon.WeaponDetails.weaponClipAmmoCapacity)
            return;

        if (Input.GetKeyDown(KeyCode.R))
        {
            player.reloadWeaponEvent.CallOnRealoadWeaponEvent(player.activeWeapon.GetCurrentWeapon, 0);
        }
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
