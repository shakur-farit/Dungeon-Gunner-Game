using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
[DisallowMultipleComponent]
public class AnimateEnemy : MonoBehaviour
{
    private Enemy _enemy;

    private void Awake()
    {
        _enemy = GetComponent<Enemy>();
    }

    private void OnEnable()
    {
        _enemy.EnemyMovementToPositionEvent.OnMovementToPosition +=
            MovementToPositionEvent_OnMovementToPosition;

        _enemy.EnemyIdleEvent.OnIdle += IdleEvent_OnIdle;

        _enemy.EnemyAimWeaponEvent.OnWeaponAim += AimWeaponEvent_OnWeaponAim;
    }

    private void OnDisable()
    {
        _enemy.EnemyMovementToPositionEvent.OnMovementToPosition -=
            MovementToPositionEvent_OnMovementToPosition;

        _enemy.EnemyIdleEvent.OnIdle -= IdleEvent_OnIdle;

        _enemy.EnemyAimWeaponEvent.OnWeaponAim -= AimWeaponEvent_OnWeaponAim;
    }

    private void MovementToPositionEvent_OnMovementToPosition(MovementToPositionEvent movementToPositionEvent,
        MovementToPositionArgs movementToPositionArgs)
    {     
        SetMovementAnimationParametrs();
    }

    private void IdleEvent_OnIdle(IdleEvent idle)
    {
        SetIdleAnimationParametrs();
    }

    private void AimWeaponEvent_OnWeaponAim(AimWeaponEvent weaponEvent, AimWeaponEventArgs aimWeaponEventArgs)
    {
        InitialiseAnimationParametrs();
        SetAimWeaponAnimationParametry(aimWeaponEventArgs.aimDirection);
    }

    private void InitialiseAnimationParametrs()
    {
        _enemy.EnemyAnimator.SetBool(Settings.aimUp, false);
        _enemy.EnemyAnimator.SetBool(Settings.aimUpRight, false);
        _enemy.EnemyAnimator.SetBool(Settings.aimUpLeft, false);
        _enemy.EnemyAnimator.SetBool(Settings.aimRight, false);
        _enemy.EnemyAnimator.SetBool(Settings.aimLeft, false);
        _enemy.EnemyAnimator.SetBool(Settings.aimDown, false);
    }

    private void SetMovementAnimationParametrs()
    {
        _enemy.EnemyAnimator.SetBool(Settings.isIdle, false);
        _enemy.EnemyAnimator.SetBool(Settings.isMoving, true);
    }

    private void SetIdleAnimationParametrs()
    {
        _enemy.EnemyAnimator.SetBool(Settings.isIdle, true);
        _enemy.EnemyAnimator.SetBool(Settings.isMoving, false);
    }

    private void SetAimWeaponAnimationParametry(AimDirection aimDirection)
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

        //int aimParam;
        if (aimMapping.TryGetValue(aimDirection, out int aimParam))
        {
            _enemy.EnemyAnimator.SetBool(aimParam, true);
        }
    }
}
