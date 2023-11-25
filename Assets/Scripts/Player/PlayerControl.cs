using System.Collections;
using System.Collections.Generic;
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
    private float playerRollColldownTimer = 0f;

    [HideInInspector] public bool IsPlayerRolling = false;

    private void Awake()
    {
        player = GetComponent<Player>();

        moveSpeed = movementDetails.GetMovementSpeed;
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

        foreach (Weapon weapon in player.WeaponList)
        {
            if(weapon.WeaponDetails == player.PlayerDetails.startingWeapon)
            {
                SetWeaponByIndex(index);
                break;
            }
            index++;
        }
    }

    private void SetPlayerAnimationSpeed()
    {
        player.PlayerAnimator.speed = moveSpeed / Settings.baseSpeedForPlayerAnimations;
    }

    private void Update()
    {
        if (IsPlayerRolling)
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
            if (!rightMouseButton)
            {
                player.PlayerMovementByVelocityEvent.CallMovementbyVelocityEvent(direction, moveSpeed);
                return;
            }

            if (playerRollColldownTimer <= 0f)
            {
                PlayerRoll((Vector3)direction);
                return;
            }
        }
        
        player.PlayerIdleEvent.CallIdleEvent();
    }

    private void PlayerRoll(Vector3 direction)
    {
        playerRollCoroutine = StartCoroutine(PlayerRollRoutine(direction));
    }

    private IEnumerator PlayerRollRoutine(Vector3 direction)
    {
        float minDistance = 0.2f;

        IsPlayerRolling = true;

        Vector3 targetPosition = player.transform.position + 
            (Vector3)direction * movementDetails.rollDistance;

        while(Vector3.Distance(player.transform.position, targetPosition) > minDistance)
        {
            player.PlayerMovementToPositionEvent.CallMovementToPositionEvent(targetPosition, 
                player.transform.position, movementDetails.rollSpeed, direction, IsPlayerRolling);

            yield return waitForFixedUpdate;
        }

        IsPlayerRolling = false;

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

        SwitchWeaponInput();

        ReloadWeaponInput();
    }

    private void AimWeaponInput(out Vector3 weaponDirection, out float weaponAngleDegress,
        out float playerAngleDegress, out AimDirection playerAimDirection)
    {
        Vector3 mouseWorldPosition = HelperUtilities.GetMouseWorldPosition();

        weaponDirection = (mouseWorldPosition - player.PlayerActiveWeapon.GetShootPosition);

        Vector3 playerDirection = (mouseWorldPosition - transform.position);

        weaponAngleDegress = HelperUtilities.GetAngelFromVector(weaponDirection);
        playerAngleDegress = HelperUtilities.GetAngelFromVector(playerDirection);

        playerAimDirection = HelperUtilities.GetAimDerection(playerAngleDegress);

        player.PlayerAimWeaponEvent.CallAimWeaponEvent(playerAimDirection, playerAngleDegress, 
            weaponAngleDegress, weaponDirection);
    }

    private void FireWeaponInput(Vector3 weaponDirection, float weaponAngleDegress, 
        float playerAngleDegress, AimDirection playerAimDirection)
    {
        if (Input.GetMouseButton(0))
        {
            player.PlayerFireWeaponEvent.CallOnFireWeaponEvent(true, leftMouseDownPreviousFrame, playerAimDirection, 
                playerAngleDegress, weaponAngleDegress, weaponDirection);
            leftMouseDownPreviousFrame = true;
            return;
        }

        leftMouseDownPreviousFrame = false;
    }

    private void SwitchWeaponInput()
    {
        if(Input.mouseScrollDelta.y < 0f)
        {
            PreviousWeapon();
        }

        if(Input.mouseScrollDelta.y > 0f)
        {
            NextWeapon();
        }

        for (int i = 1; i < 10; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0 + i))
            {
                SetWeaponByIndex(i);
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            SetWeaponByIndex(10);
        }

        if (Input.GetKeyDown(KeyCode.Minus))
        {
            SetCurrentWeaponToFirstInTheList();
        }
    }

    private void SetWeaponByIndex(int weaponIndex)
    {
        int index = weaponIndex - 1;

        if(index < player.WeaponList.Count)
        {
            currentWeaponIndex = weaponIndex;
            player.PlayerSetActiveWeaponEvent.CallSetActiveWeaponEvent(player.WeaponList[index]);
        }
    }

    private void NextWeapon()
    {
        currentWeaponIndex++;

        if(currentWeaponIndex > player.WeaponList.Count)
        {
            currentWeaponIndex = 1;
        }

        SetWeaponByIndex(currentWeaponIndex);
    }

    private void PreviousWeapon()
    {
        currentWeaponIndex--;

        if (currentWeaponIndex < 1)
        {
            currentWeaponIndex = player.WeaponList.Count;
        }

        SetWeaponByIndex(currentWeaponIndex);
    }

    private void ReloadWeaponInput()
    {
        Weapon currentWeapon = player.PlayerActiveWeapon.GetCurrentWeapon;

        if (currentWeapon.IsWeaponReloading)
            return;

        if (currentWeapon.WeaponRemainingAmmo < currentWeapon.WeaponDetails.weaponClipAmmoCapacity &&
            !currentWeapon.WeaponDetails.hasInfiniteAmmo)
            return;

        if (currentWeapon.WeaponClipRemainingAmmo == currentWeapon.WeaponDetails.weaponClipAmmoCapacity)
            return;

        if (Input.GetKeyDown(KeyCode.R))
        {
            player.PlayerReloadWeaponEvent.CallOnRealoadWeaponEvent(player.PlayerActiveWeapon.GetCurrentWeapon, 0);
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

            IsPlayerRolling = false;
        }
    }

    private void SetCurrentWeaponToFirstInTheList()
    {
        List<Weapon> tempWeaponList = new List<Weapon>();

        Weapon currentWeapon = player.WeaponList[currentWeaponIndex - 1];
        currentWeapon.WeaponListPosition = 1;
        tempWeaponList.Add(currentWeapon);

        int index = 2;

        foreach (Weapon weapon in player.WeaponList)
        {
            if (weapon == currentWeapon)
                continue;

            tempWeaponList.Add(weapon);
            weapon.WeaponListPosition = index;
            index++;
        }

        player.WeaponList = tempWeaponList;

        currentWeaponIndex = 1;

        SetWeaponByIndex(currentWeaponIndex);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(movementDetails), movementDetails);
    }
#endif
}
