using UnityEngine;

[RequireComponent(typeof(ActiveWeapon))]
[RequireComponent(typeof(FireWeaponEvent))]
[RequireComponent(typeof(WeaponFiredEvent))]
[DisallowMultipleComponent]
public class FireWeapon : MonoBehaviour
{
    private float _fireRateCoolDownTimer = 0f;
    private ActiveWeapon _activeWeapon;
    private FireWeaponEvent _fireWeaponEvent;
    private WeaponFiredEvent _weaponFiredEvent;

    private void Awake()
    {
        _activeWeapon = GetComponent<ActiveWeapon>();
        _fireWeaponEvent = GetComponent<FireWeaponEvent>();
        _weaponFiredEvent = GetComponent<WeaponFiredEvent>();
    }

    private void OnEnable()
    {
        _fireWeaponEvent.OnFireWeapon += FireWeaponEvent_OnFireWeapon;
    }

    private void OnDisable()
    {
        _fireWeaponEvent.OnFireWeapon -= FireWeaponEvent_OnFireWeapon;
    }

    private void Update()
    {
        _fireRateCoolDownTimer -= Time.deltaTime;
    }

    private void FireWeaponEvent_OnFireWeapon(FireWeaponEvent fireWeaponEvent, FireWeaponEventArgs fireWeaponEventArgs)
    {
        WeaponFire(fireWeaponEventArgs);
    }

    private void WeaponFire(FireWeaponEventArgs fireWeaponEventArgs)
    {
        if (fireWeaponEventArgs.Fire)
        {
            if (IsWeaponReadyToFire())
            {
                FireAmmo(fireWeaponEventArgs.AimAngle,
                    fireWeaponEventArgs.WeaponAimAngle, fireWeaponEventArgs.WeaponAimDirectionVector);

                ResetColdownTimer();
            }
        }
    }

    private void FireAmmo(float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector)
    {
        AmmoDetailsSO currentAmmo = _activeWeapon.GetCurrentAmmo;

        if (currentAmmo == null)
            return;

        GameObject ammoPrefab = currentAmmo.ammoPrefabArray[Random.Range(0, currentAmmo.ammoPrefabArray.Length)];

        float ammoSpeed = Random.Range(currentAmmo.ammoSpeedMin, currentAmmo.ammoSpeedMax);

        IFireable ammo = (IFireable)PoolManager.Instance.ReuseComponent(ammoPrefab, _activeWeapon.GetShootPosition, Quaternion.identity);

        ammo.InitialiseAmmo(currentAmmo, aimAngle, weaponAimAngle, ammoSpeed, weaponAimDirectionVector);

        if (!_activeWeapon.GetCurrentWeapon.WeaponDetails.hasInfinityClipCapacity)
        {
            _activeWeapon.GetCurrentWeapon.WeaponClipRemainingAmmo--;
            _activeWeapon.GetCurrentWeapon.WeaponRemainingAmmo--;
        }

        _weaponFiredEvent.CallOnWeaponFiredEvent(_activeWeapon.GetCurrentWeapon);
    }

    private void ResetColdownTimer()
    {
        _fireRateCoolDownTimer = _activeWeapon.GetCurrentWeapon.WeaponDetails.weaponFireRate;
    }

    private bool IsWeaponReadyToFire()
    {
        if (_activeWeapon.GetCurrentWeapon.WeaponRemainingAmmo <= 0 &&
            !_activeWeapon.GetCurrentWeapon.WeaponDetails.hasInfiniteAmmo)
            return false;

        if (_activeWeapon.GetCurrentWeapon.IsWeaponReloading)
            return false;

        if (_fireRateCoolDownTimer > 0f)
            return false;

        if (!_activeWeapon.GetCurrentWeapon.WeaponDetails.hasInfinityClipCapacity &&
            _activeWeapon.GetCurrentWeapon.WeaponClipRemainingAmmo <= 0)
            return false;

        return true;
    }
}
