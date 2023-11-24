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
        player.PlayerIdleEvent.OnIdle += IdleEvent_OnIdle;

        player.PlayerAimWeaponEvent.OnWeaponAim += AimWeaponEvent_OnWeaponAim;

        player.PlayerMovementByVelocityEvent.OnMovementByVelocity += MovementByVelocityEvent_OnMovementByVelocity;

        player.PlayerMovementToPositionEvent.OnMovementToPosition += MovementToPositionEvent_OnMovementToPosition;
    }

    private void OnDisable()
    {
        player.PlayerIdleEvent.OnIdle -= IdleEvent_OnIdle;

        player.PlayerAimWeaponEvent.OnWeaponAim -= AimWeaponEvent_OnWeaponAim;

        player.PlayerMovementByVelocityEvent.OnMovementByVelocity -= MovementByVelocityEvent_OnMovementByVelocity;

        player.PlayerMovementToPositionEvent.OnMovementToPosition -= MovementToPositionEvent_OnMovementToPosition;
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
        player.PlayerAnimator.SetBool(Settings.isMoving, false);
        player.PlayerAnimator.SetBool(Settings.isIdle, true);
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
            player.PlayerAnimator.SetBool(aimParam, true);
        }
    }

    private void SetMovementAnimationParametrs()
    {
        player.PlayerAnimator.SetBool(Settings.isMoving, true);
        player.PlayerAnimator.SetBool(Settings.isIdle, false);
    }

    private void SetMovementToPositionAnimationParametrs(MovementToPositionArgs movementToPositionArgs)
    {
        if (movementToPositionArgs.isRolling)
        {
            if (movementToPositionArgs.moveDirection.x > 0)
            {
                player.PlayerAnimator.SetBool(Settings.rollRight, true);
                return;
            }

            if (movementToPositionArgs.moveDirection.x < 0)
            {
                player.PlayerAnimator.SetBool(Settings.rollLeft, true);
                return;
            }

            if (movementToPositionArgs.moveDirection.y > 0)
            {
                player.PlayerAnimator.SetBool(Settings.rollUp, true);
                return;
            }

            if (movementToPositionArgs.moveDirection.y < 0)
            {
                player.PlayerAnimator.SetBool(Settings.rollDown, true);
            }
        }
    }

    private void InitializeAnimationParametrs()
    {
        player.PlayerAnimator.SetBool(Settings.aimUp, false);
        player.PlayerAnimator.SetBool(Settings.aimUpLeft, false);
        player.PlayerAnimator.SetBool(Settings.aimUpRight, false);
        player.PlayerAnimator.SetBool(Settings.aimLeft, false);
        player.PlayerAnimator.SetBool(Settings.aimRight, false);
        player.PlayerAnimator.SetBool(Settings.aimDown, false);
    }

    private void InitializeRollAnimationParametrs()
    {
        player.PlayerAnimator.SetBool(Settings.rollUp, false);
        player.PlayerAnimator.SetBool(Settings.rollRight, false);
        player.PlayerAnimator.SetBool(Settings.rollLeft, false);
        player.PlayerAnimator.SetBool(Settings.rollDown, false);
    }
}
