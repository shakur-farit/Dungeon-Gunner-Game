using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(EnemyWeaponAI))]
[RequireComponent(typeof(AimWeaponEvent))]
[RequireComponent(typeof(AimWeapon))]
[RequireComponent(typeof(FireWeaponEvent))]
[RequireComponent(typeof(FireWeapon))]
[RequireComponent(typeof(SetActiveWeaponEvent))]
[RequireComponent(typeof(ActiveWeapon))]
[RequireComponent(typeof(WeaponFiredEvent))]
[RequireComponent(typeof(ReloadWeaponEvent))]
[RequireComponent(typeof(ReloadWeapon))]
[RequireComponent(typeof(WeaponReloadedEvent))]
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
[RequireComponent(typeof(HealthEvent))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(DestroyedEvent))]
[RequireComponent(typeof(Destroyed))]
[RequireComponent(typeof(DealContactDamage))]
[DisallowMultipleComponent]
public class Enemy : MonoBehaviour
{
    [HideInInspector] public EnemyDetailsSO EnemyDetails;
    [HideInInspector] public AimWeaponEvent EnemyAimWeaponEvent;
    [HideInInspector] public FireWeaponEvent EnemyFireWeaponEvent;
    [HideInInspector] public SpriteRenderer[] SpriteRendererArray;
    [HideInInspector] public Animator EnemyAnimator;
    [HideInInspector] public MovementToPositionEvent EnemyMovementToPositionEvent;
    [HideInInspector] public IdleEvent EnemyIdleEvent;

    private FireWeapon _fireWeapon;
    private SetActiveWeaponEvent _setActiveWeaponEvent;
    private MaterializeEffect _materializeEffect;
    private CircleCollider2D _circleCollider2D;
    private PolygonCollider2D _polygonCollider2D;
    private EnemyMovementAI _enemyMovementAI;
    private HealthEvent _healthEvent;
    private Health _health;

    private void Awake()
    {
        _fireWeapon = GetComponent<FireWeapon>();
        _setActiveWeaponEvent = GetComponent<SetActiveWeaponEvent>();
        _materializeEffect = GetComponent<MaterializeEffect>();
        _circleCollider2D = GetComponent<CircleCollider2D>();
        _polygonCollider2D = GetComponent<PolygonCollider2D>();
        _enemyMovementAI = GetComponent<EnemyMovementAI>();
        _healthEvent = GetComponent<HealthEvent>();
        _health = GetComponent<Health>();
        SpriteRendererArray = GetComponentsInChildren<SpriteRenderer>();
        EnemyAnimator = GetComponent<Animator>();
        EnemyMovementToPositionEvent = GetComponent<MovementToPositionEvent>();
        EnemyIdleEvent = GetComponent<IdleEvent>();
        EnemyAimWeaponEvent = GetComponent<AimWeaponEvent>();
        EnemyFireWeaponEvent = GetComponent<FireWeaponEvent>();
    }

    private void OnEnable()
    {
        _healthEvent.OnHealthChanged += HealthEvent_OnHealthLost;
    }

    private void OnDisable()
    { 
        _healthEvent.OnHealthChanged -= HealthEvent_OnHealthLost;
    }

    private void HealthEvent_OnHealthLost(HealthEvent healthEvent, HealthEventArgs healthEventArgs)
    {
        if (healthEventArgs.HealthAmount <= 0)
        {
            EnemyDestoryed();
        }
    }

    private void EnemyDestoryed()
    {
        DestroyedEvent destroyedEvent = GetComponent<DestroyedEvent>();
        destroyedEvent.CallDestroyedEvent(false, _health.GetStartingHealth);
    }

    public void EnemyInitialization(EnemyDetailsSO enemyDetails, int enemySpawnNumber,
        DungeonLevelSO dungeonLevel)
    {
        EnemyDetails = enemyDetails;

        SetEnemyMovementUpdateFrame(enemySpawnNumber);

        SetEnemyStartingHealth(dungeonLevel);

        SetEnemyStartingWeapon();
            
        SetEnemyAnimationSpeed();

        StartCoroutine(MaterializeEnemy());
    }

    private void SetEnemyMovementUpdateFrame(int enemySpawnNumber)
    {
        _enemyMovementAI.SetUpdateFrameNumber = 
            enemySpawnNumber % Settings.targetFrameRateToSpreadPathfidingOver;
    }

    private void SetEnemyStartingHealth(DungeonLevelSO dungeonLevel)
    {
        foreach (EnemyHealthDetails enemyHealthDetails in EnemyDetails.EnemyHealthDetailsArray)
        {
            if(enemyHealthDetails.DungeonLevel == dungeonLevel)
            {
                _health.SetStartingHealth = enemyHealthDetails.EnemyHealthAmount;
                return;
            }
        }

        _health.SetStartingHealth = Settings.defaultEnemyHealth;
    }

    private void SetEnemyStartingWeapon()
    {
        if(EnemyDetails.EnenmyWeapon != null)
        {
            Weapon weapon = new Weapon()
            {
                WeaponDetails = EnemyDetails.EnenmyWeapon,
                WeaponReloadTimer = 0f,
                WeaponClipRemainingAmmo = EnemyDetails.EnenmyWeapon.weaponClipAmmoCapacity,
                WeaponRemainingAmmo = EnemyDetails.EnenmyWeapon.weaponAmmoCapacity,
                IsWeaponReloading = false
            };

            _setActiveWeaponEvent.CallSetActiveWeaponEvent(weapon);
        }
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

        _fireWeapon.enabled = isEnabled;
    }
}
