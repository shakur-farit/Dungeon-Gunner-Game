using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(HealthEvent))]
[RequireComponent(typeof(DealContactDamage))]
[RequireComponent(typeof(ReceiveContactDamage))]
[RequireComponent(typeof(DestroyedEvent))]
[RequireComponent(typeof(Destroyed))]
[RequireComponent(typeof(PlayerControl))]
[RequireComponent(typeof(MovementToPosition))]
[RequireComponent(typeof(MovementToPositionEvent))]
[RequireComponent(typeof(MovementByVelocity))]
[RequireComponent(typeof(MovementByVelocityEvent))]
[RequireComponent(typeof(Idle))]
[RequireComponent(typeof(IdleEvent))]
[RequireComponent(typeof(AimWeapon))]
[RequireComponent(typeof(AimWeaponEvent))]
[RequireComponent(typeof(SetActiveWeaponEvent))]
[RequireComponent(typeof(FireWeaponEvent))]
[RequireComponent(typeof(FireWeapon))]
[RequireComponent(typeof(WeaponFiredEvent))]
[RequireComponent(typeof(ReloadWeaponEvent))]
[RequireComponent(typeof(WeaponReloadedEvent))]
[RequireComponent(typeof(ReloadWeapon))]
[RequireComponent(typeof(ActiveWeapon))]
[RequireComponent(typeof(AnimatePlayer))]
[RequireComponent(typeof(SortingGroup))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[DisallowMultipleComponent]
public class Player : MonoBehaviour
{
    [HideInInspector] public PlayerDetailsSO PlayerDetails;
    [HideInInspector] public Health PlayerHealth;
    [HideInInspector] public HealthEvent PlayerHealthEvent;
    [HideInInspector] public DestroyedEvent PlayerDestroyedEvent;
    [HideInInspector] public PlayerControl PlayerControlReference;
    [HideInInspector] public MovementToPositionEvent PlayerMovementToPositionEvent;
    [HideInInspector] public MovementByVelocityEvent PlayerMovementByVelocityEvent;
    [HideInInspector] public IdleEvent PlayerIdleEvent;
    [HideInInspector] public AimWeaponEvent PlayerAimWeaponEvent;
    [HideInInspector] public SetActiveWeaponEvent PlayerSetActiveWeaponEvent;
    [HideInInspector] public FireWeaponEvent PlayerFireWeaponEvent;
    [HideInInspector] public FireWeapon PlayerFireWeapon;
    [HideInInspector] public ReloadWeaponEvent PlayerReloadWeaponEvent;
    [HideInInspector] public WeaponReloadedEvent PlayerWeaponReloadedEvent;
    [HideInInspector] public ReloadWeapon PlayerReloadWeapon;
    [HideInInspector] public WeaponFiredEvent PlayerWeaponFiredEvent;
    [HideInInspector] public ActiveWeapon PlayerActiveWeapon;
    [HideInInspector] public SpriteRenderer PlayerSpriteRenderer;
    [HideInInspector] public Animator PlayerAnimator;

    public List<Weapon> WeaponList = new List<Weapon>();

    public Vector3 GetPlayerPosition => transform.position;

    private void Awake()
    {
        PlayerHealth = GetComponent<Health>();
        PlayerHealthEvent = GetComponent<HealthEvent>();
        PlayerDestroyedEvent = GetComponent<DestroyedEvent>();
        PlayerControlReference = GetComponent<PlayerControl>();
        PlayerMovementToPositionEvent = GetComponent<MovementToPositionEvent>();
        PlayerMovementByVelocityEvent = GetComponent<MovementByVelocityEvent>();
        PlayerIdleEvent = GetComponent<IdleEvent>();
        PlayerAimWeaponEvent = GetComponent<AimWeaponEvent>();
        PlayerSetActiveWeaponEvent = GetComponent<SetActiveWeaponEvent>();
        PlayerFireWeaponEvent = GetComponent<FireWeaponEvent>();
        PlayerFireWeapon = GetComponent<FireWeapon>();
        PlayerWeaponFiredEvent = GetComponent<WeaponFiredEvent>();
        PlayerReloadWeaponEvent = GetComponent<ReloadWeaponEvent>();
        PlayerWeaponReloadedEvent = GetComponent<WeaponReloadedEvent>();
        PlayerReloadWeapon = GetComponent<ReloadWeapon>();
        PlayerActiveWeapon = GetComponent<ActiveWeapon>();
        PlayerSpriteRenderer = GetComponent<SpriteRenderer>();
        PlayerAnimator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        PlayerHealthEvent.OnHealthChanged += HealthEvent_OnHealthChanged;  
    }

    private void OnDisable()
    {
        PlayerHealthEvent.OnHealthChanged -= HealthEvent_OnHealthChanged;
    }

    private void HealthEvent_OnHealthChanged(HealthEvent healthEvent, HealthEventArgs healthEventArgs)
    {
        if(healthEventArgs.HealthAmount <= 0f)
        {
            PlayerDestroyedEvent.CallDestroyedEvent(true, 0);
        }
    }

    public void Initialize(PlayerDetailsSO playerDetails)
    {
        PlayerDetails = playerDetails;

        CreatePlayerStartingWeapons();

        SetPlayerHealth();
    }

    private void CreatePlayerStartingWeapons()
    {
        WeaponList.Clear();

        foreach (WeaponDetailsSO weaponDetails in PlayerDetails.startingWeaponList)
        {
            AddWeaponToPlayer(weaponDetails);
        }
    }

    private Weapon AddWeaponToPlayer(WeaponDetailsSO weaponDetails)
    {
        Weapon weapon = new Weapon()
        {
            WeaponDetails = weaponDetails,
            WeaponReloadTimer = 0f,
            WeaponClipRemainingAmmo = weaponDetails.weaponClipAmmoCapacity,
            WeaponRemainingAmmo = weaponDetails.weaponAmmoCapacity,
            IsWeaponReloading = false
        };

        WeaponList.Add(weapon);

        weapon.WeaponListPosition = WeaponList.Count;

        PlayerSetActiveWeaponEvent.CallSetActiveWeaponEvent(weapon);

        return weapon;
    }

    private void SetPlayerHealth()
    {
        PlayerHealth.SetStartingHealth = PlayerDetails.playerHealthAmount;
    }
}
