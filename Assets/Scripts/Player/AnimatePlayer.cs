using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
[DisallowMultipleComponent]
public class AnimatePlayer : MonoBehaviour
{
    private Player player;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void OnEnable()
    {
        player.idleEvent.OnIdle += IdleEvent_OnIdle;

        player.aimWeaponEvent.OnWeaponAim += AimWeaponEvent_OnWeaponAim;

        player.movementByVelocityEvent.OnMovementByVelocity += MovementByVelocityEvent_OnMovementByVelocity;

        player.movementToPositionEvent.OnMovementToPosition += MovementToPositionEvent_OnMovementToPosition;
    }

    private void OnDisable()
    {
        player.idleEvent.OnIdle -= IdleEvent_OnIdle;

        player.aimWeaponEvent.OnWeaponAim -= AimWeaponEvent_OnWeaponAim;

        player.movementByVelocityEvent.OnMovementByVelocity -= MovementByVelocityEvent_OnMovementByVelocity;

        player.movementToPositionEvent.OnMovementToPosition -= MovementToPositionEvent_OnMovementToPosition;
    }

    private void IdleEvent_OnIdle(IdleEvent idleEvent)
    {
        InitializeRollAnimationParametrs();
        SetIdolAnimationParameters();
    }

    private void AimWeaponEvent_OnWeaponAim(AimWeaponEvent aimWeaponEvent, AimWeaponEventArgs aimWeaponEventArgs)
    {
        InitializeAnimationParametrs();
        InitializeRollAnimationParametrs();
        SetAimWeaponAnimationParametrs(aimWeaponEventArgs.aimDirection);
    }

    private void MovementByVelocityEvent_OnMovementByVelocity(MovementByVelocityEvent movementByVelocityEvent,
        MovementByVelocityArgs movementByVelocityArgs)
    {
        InitializeRollAnimationParametrs();
        SetMovementAnimationParametrs();
    }

    private void MovementToPositionEvent_OnMovementToPosition(MovementToPositionEvent movementToPositionEvent,
        MovementToPositionArgs movementToPositionArgs)
    {
        InitializeAnimationParametrs();
        InitializeRollAnimationParametrs();
        SetMovementToPositionAnimationParametrs(movementToPositionArgs);
    }

    private void SetIdolAnimationParameters()
    {
        player.animator.SetBool(Settings.isMoving, false);
        player.animator.SetBool(Settings.isIdle, true);
    }

    private void SetAimWeaponAnimationParametrs(AimDirection aimDirection)
    {
        Dictionary<AimDirection, int> aimMapping = new Dictionary<AimDirection, int>
        {
            { AimDirection.Up, Settings.aimUp },
            { AimDirection.UpLeft, Settings.aimUpLeft },
            { AimDirection.UpRight, Settings.aimUpRight },
            { AimDirection.Left, Settings.aimLeft },
            { AimDirection.Right, Settings.aimRight },
            { AimDirection.Down, Settings.aimDown }
        };

        int aimParam;
        if (aimMapping.TryGetValue(aimDirection, out aimParam))
        {
            player.animator.SetBool(aimParam, true);
        }
    }

    private void SetMovementAnimationParametrs()
    {
        player.animator.SetBool(Settings.isMoving, true);
        player.animator.SetBool(Settings.isIdle, false);
    }

    private void SetMovementToPositionAnimationParametrs(MovementToPositionArgs movementToPositionArgs)
    {
        if (movementToPositionArgs.isRolling)
        {
            if (movementToPositionArgs.moveDirection.x > 0)
            {
                player.animator.SetBool(Settings.rollRight, true);
                return;
            }

            if (movementToPositionArgs.moveDirection.x < 0)
            {
                player.animator.SetBool(Settings.rollLeft, true);
                return;
            }

            if (movementToPositionArgs.moveDirection.y > 0)
            {
                player.animator.SetBool(Settings.rollUp, true);
                return;
            }

            if (movementToPositionArgs.moveDirection.y < 0)
            {
                player.animator.SetBool(Settings.rollDown, true);
            }
        }
    }

    private void InitializeAnimationParametrs()
    {
        player.animator.SetBool(Settings.aimUp, false);
        player.animator.SetBool(Settings.aimUpLeft, false);
        player.animator.SetBool(Settings.aimUpRight, false);
        player.animator.SetBool(Settings.aimLeft, false);
        player.animator.SetBool(Settings.aimRight, false);
        player.animator.SetBool(Settings.aimDown, false);
    }

    private void InitializeRollAnimationParametrs()
    {
        player.animator.SetBool(Settings.rollUp, false);
        player.animator.SetBool(Settings.rollRight, false);
        player.animator.SetBool(Settings.rollLeft, false);
        player.animator.SetBool(Settings.rollDown, false);
    }
}
