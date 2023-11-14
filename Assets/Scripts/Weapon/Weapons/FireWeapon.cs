using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ActiveWeapon))]
[RequireComponent(typeof(FireWeaponEvent))]
[RequireComponent(typeof(ReloadWeaponEvent))]
[RequireComponent(typeof(WeaponFiredEvent))]
[DisallowMultipleComponent]
public class FireWeapon : MonoBehaviour
{
    private float _firePrechargeTimer = 0f;
    private float _fireRateCoolDownTimer = 0f;
    private ActiveWeapon _activeWeapon;
    private FireWeaponEvent _fireWeaponEvent;
    private ReloadWeaponEvent _reloadWeaponEvent;
    private WeaponFiredEvent _weaponFiredEvent;

    private void Awake()
    {
        _activeWeapon = GetComponent<ActiveWeapon>();
        _fireWeaponEvent = GetComponent<FireWeaponEvent>();
        _reloadWeaponEvent = GetComponent<ReloadWeaponEvent>();
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
        WeaponPrecharge(fireWeaponEventArgs);

        if (fireWeaponEventArgs.Fire)
        {
            if (IsWeaponReadyToFire())
            {
                FireAmmo(fireWeaponEventArgs.AimAngle,
                    fireWeaponEventArgs.WeaponAimAngle, fireWeaponEventArgs.WeaponAimDirectionVector);

                ResetColdownTimer();

                ResetPrechargeTimer();
            }
        }
    }

    private void WeaponPrecharge(FireWeaponEventArgs fireWeaponEventArgs)
    {
        if (fireWeaponEventArgs.FirePreviousFrame)
        {
            _firePrechargeTimer -= Time.deltaTime;
            return;
        }

        ResetPrechargeTimer();
    }

    private void FireAmmo(float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector)
    {
        AmmoDetailsSO currentAmmo = _activeWeapon.GetCurrentAmmo;

        if (currentAmmo != null)
            StartCoroutine(FireAmmoRoutine(currentAmmo, aimAngle, weaponAimAngle, weaponAimDirectionVector));
    }

    private IEnumerator FireAmmoRoutine(AmmoDetailsSO currentAmmo, float aimAngle,
        float weaponAimAngle, Vector3 weaponAimDirectionVector)
    {
        int ammoCounter = 0;

        int ammoPerShot = Random.Range(currentAmmo.ammoSpawnAmountMin, currentAmmo.ammoSpawnAmountMax + 1);

        float ammoSpawnInterval;

        if(ammoPerShot > 1)
        {
            ammoSpawnInterval = Random.Range(currentAmmo.ammoSpawnIntervalMin, currentAmmo.ammoSpawnIntervalMax);
        }
        else
        {
            ammoSpawnInterval = 0f;
        }

        while(ammoCounter < ammoPerShot)
        {
            ammoCounter++;

            GameObject ammoPrefab = currentAmmo.ammoPrefabArray[Random.Range(0, currentAmmo.ammoPrefabArray.Length)];

            float ammoSpeed = Random.Range(currentAmmo.ammoSpeedMin, currentAmmo.ammoSpeedMax);

            IFireable ammo = (IFireable)PoolManager.Instance
                .ReuseComponent(ammoPrefab, _activeWeapon.GetShootPosition, Quaternion.identity);

            ammo.InitialiseAmmo(currentAmmo, aimAngle, weaponAimAngle, ammoSpeed, weaponAimDirectionVector);

            yield return new WaitForSeconds(ammoSpawnInterval);
        }

        if (!_activeWeapon.GetCurrentWeapon.WeaponDetails.hasInfinityClipCapacity)
        {
            _activeWeapon.GetCurrentWeapon.WeaponClipRemainingAmmo--;
            _activeWeapon.GetCurrentWeapon.WeaponRemainingAmmo--;
        }

        _weaponFiredEvent.CallOnWeaponFiredEvent(_activeWeapon.GetCurrentWeapon);

        PlayWeaponShootEffect(aimAngle);

        PlayWeaponSoundEffect();
    }

    private bool IsWeaponReadyToFire()
    {
        if (_activeWeapon.GetCurrentWeapon.WeaponRemainingAmmo <= 0 &&
            !_activeWeapon.GetCurrentWeapon.WeaponDetails.hasInfiniteAmmo)
            return false;

        if (_activeWeapon.GetCurrentWeapon.IsWeaponReloading)
            return false;

        if (_firePrechargeTimer > 0f || _fireRateCoolDownTimer > 0f)
            return false;

        if (!_activeWeapon.GetCurrentWeapon.WeaponDetails.hasInfinityClipCapacity &&
            _activeWeapon.GetCurrentWeapon.WeaponClipRemainingAmmo <= 0)
        {
            _reloadWeaponEvent.CallOnRealoadWeaponEvent(_activeWeapon.GetCurrentWeapon, 0);

            return false;
        }

        return true;
    }

    private void ResetColdownTimer()
    {
        _fireRateCoolDownTimer = _activeWeapon.GetCurrentWeapon.WeaponDetails.weaponFireRate;
    }

    private void ResetPrechargeTimer()
    {
        _firePrechargeTimer = _activeWeapon.GetCurrentWeapon.WeaponDetails.weaponPrechargeTime;
    }

    private void PlayWeaponShootEffect(float aimAngle)
    {
        if(_activeWeapon.GetCurrentWeapon.WeaponDetails.weaponShootEffect != null &&
            _activeWeapon.GetCurrentWeapon.WeaponDetails.weaponShootEffect.WeaponShootEffectPrefab != null)
        {
            WeaponShootEffect weaponShootEffect = (WeaponShootEffect)PoolManager.Instance.ReuseComponent(
                _activeWeapon.GetCurrentWeapon.WeaponDetails.weaponShootEffect.WeaponShootEffectPrefab,
                _activeWeapon.GetShootEffectPosition, Quaternion.identity);

            weaponShootEffect.SetShootEffect(_activeWeapon.GetCurrentWeapon.WeaponDetails.weaponShootEffect, aimAngle);

            weaponShootEffect.gameObject.SetActive(true);
        }
    }

    private void PlayWeaponSoundEffect()
    {
        if(_activeWeapon.GetCurrentWeapon.WeaponDetails.weaponFiringSoundEffect != null)
        {
            SoundEffectManager.Instance.PlaySoundEffect(_activeWeapon
                .GetCurrentWeapon.WeaponDetails.weaponFiringSoundEffect);
        }
    }
}
