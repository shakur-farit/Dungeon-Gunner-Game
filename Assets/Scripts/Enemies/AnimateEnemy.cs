using System;
using System.Collections;
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
    }

    private void OnDisable()
    {
        _enemy.EnemyMovementToPositionEvent.OnMovementToPosition -=
            MovementToPositionEvent_OnMovementToPosition;

        _enemy.EnemyIdleEvent.OnIdle -= IdleEvent_OnIdle;
    }

    private void MovementToPositionEvent_OnMovementToPosition(MovementToPositionEvent movementToPositionEvent,
        MovementToPositionArgs movementToPositionArgs)
    {
        if(_enemy.transform.position.x < GameManager.Instance.GetPlayer.transform.position.x)
        {
            SetAimWeaponAnimationParametry(AimDirection.Right);
        }
        else
        {
            SetAimWeaponAnimationParametry(AimDirection.Left);
        }

        SetMovementAnimationParametrs();
    }

    private void IdleEvent_OnIdle(IdleEvent idle)
    {
        SetIdleAnimationParametrs();
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
        InitialiseAnimationParametrs();

        switch (aimDirection)
        {
            case AimDirection.Up:
                _enemy.EnemyAnimator.SetBool(Settings.aimUp, true);
                break;
            case AimDirection.UpLeft:
                _enemy.EnemyAnimator.SetBool(Settings.aimUpLeft, true);
                break;
            case AimDirection.UpRight:
                _enemy.EnemyAnimator.SetBool(Settings.aimUpRight, true);
                break;
            case AimDirection.Left:
                _enemy.EnemyAnimator.SetBool(Settings.aimLeft, true);
                break;
            case AimDirection.Right:
                _enemy.EnemyAnimator.SetBool(Settings.aimRight, true);
                break;
            case AimDirection.Down:
                _enemy.EnemyAnimator.SetBool(Settings.aimDown, true);
                break;
        }
    }
}
