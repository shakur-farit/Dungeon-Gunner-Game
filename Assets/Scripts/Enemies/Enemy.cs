using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(SortingGroup))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(AnimateEnemy))]
[RequireComponent(typeof(EnemyMovementAI))]
[RequireComponent(typeof(MaterializeEffect))]
[RequireComponent(typeof(MovementToPositionEvent))]
[RequireComponent(typeof(MovementToPosition))]
[RequireComponent(typeof(IdleEvent))]
[RequireComponent(typeof(Idle))]
[DisallowMultipleComponent]
public class Enemy : MonoBehaviour
{
    [HideInInspector] public EnemyDetailsSO EnemyDetails;
    [HideInInspector] public SpriteRenderer[] SpriteRendererArray;
    [HideInInspector] public Animator EnemyAnimator;
    [HideInInspector] public MovementToPositionEvent EnemyMovementToPositionEvent;
    [HideInInspector] public IdleEvent EnemyIdleEvent;

    private MaterializeEffect _materializeEffect;
    private CircleCollider2D _circleCollider2D;
    private PolygonCollider2D _polygonCollider2D;
    private EnemyMovementAI _enemyMovementAI;

    private void Awake()
    {
        _materializeEffect = GetComponent<MaterializeEffect>();
        _circleCollider2D = GetComponent<CircleCollider2D>();
        _polygonCollider2D = GetComponent<PolygonCollider2D>();
        _enemyMovementAI = GetComponent<EnemyMovementAI>();
        SpriteRendererArray = GetComponentsInChildren<SpriteRenderer>();
        EnemyAnimator = GetComponent<Animator>();
        EnemyMovementToPositionEvent = GetComponent<MovementToPositionEvent>();
        EnemyIdleEvent = GetComponent<IdleEvent>();
    }

    public void EnemyInitialization(EnemyDetailsSO enemyDetails, int enemySpawnNumber,
        DungeonLevelSO dungeonLevel)
    {
        EnemyDetails = enemyDetails;

        SetEnemyMovementUpdateFrame(enemySpawnNumber);
            
        SetEnemyAnimationSpeed();

        StartCoroutine(MaterializeEnemy());
    }

    private void SetEnemyMovementUpdateFrame(int enemySpawnNumber)
    {
        _enemyMovementAI.SetUpdateFrameNumber = 
            enemySpawnNumber % Settings.targetFrameRateToSpreadPathfidingOver;
    }

    private void SetEnemyAnimationSpeed()
    {
        EnemyAnimator.speed = _enemyMovementAI.MovementSpeed / Settings.baseSpeedForEnemyAnimations;
    }

    private IEnumerator MaterializeEnemy()
    {
        EnemyEnable(false);

        yield return StartCoroutine(_materializeEffect.MaterializeRoutine(EnemyDetails.EnemyMaterializeShader,
            EnemyDetails.EnemyMaterializeColor, EnemyDetails.EnemyMaterializeTime, SpriteRendererArray,
            EnemyDetails.EnemyStandartMaterial));

        EnemyEnable(true);
    }

    private void EnemyEnable(bool isEnabled)
    {
        _circleCollider2D.enabled = isEnabled;
        _polygonCollider2D.enabled = isEnabled;

        _enemyMovementAI.enabled = isEnabled;
    }
}
