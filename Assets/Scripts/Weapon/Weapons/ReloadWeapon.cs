using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ReloadWeaponEvent))]
[RequireComponent(typeof(WeaponReloadedEvent))]
[RequireComponent(typeof(SetActiveWeaponEvent))]
[DisallowMultipleComponent]
public class ReloadWeapon : MonoBehaviour
{
    private ReloadWeaponEvent _reloadWeaponEvent;
    private WeaponReloadedEvent _weaponReloadedEvent;
    private SetActiveWeaponEvent _setActiveWeaponEvent;
    private Coroutine _reloadWeaponCoroutine;

    private void Awake()
    {
        _reloadWeaponEvent = GetComponent<ReloadWeaponEvent>();
        _weaponReloadedEvent = GetComponent<WeaponReloadedEvent>();
        _setActiveWeaponEvent = GetComponent<SetActiveWeaponEvent>();
    }

    private void OnEnable()
    {
        _reloadWeaponEvent.OnReloadWeapon += ReloadedWeaponEvent_OnReloadWeapon;

        _setActiveWeaponEvent.OnSetActiveWeapon += SetActiveWeaponEvent_OnSetActiveWeapon;
    }

    private void OnDisable()
    {
        _reloadWeaponEvent.OnReloadWeapon -= ReloadedWeaponEvent_OnReloadWeapon;

        _setActiveWeaponEvent.OnSetActiveWeapon -= SetActiveWeaponEvent_OnSetActiveWeapon;
    }

    private void ReloadedWeaponEvent_OnReloadWeapon(ReloadWeaponEvent reloadWeaponEvent,
        ReloadWeaponEventArgs reloadWeaponEventArgs)
    {
        StartReloadWeapon(reloadWeaponEventArgs);
    }

    private void StartReloadWeapon(ReloadWeaponEventArgs reloadWeaponEventArgs)
    {
        if(_reloadWeaponCoroutine != null)
        {
            StopCoroutine(_reloadWeaponCoroutine);
        }

        _reloadWeaponCoroutine = StartCoroutine(ReloadWeaponRoutine(reloadWeaponEventArgs.Weapon,
            reloadWeaponEventArgs.TopUpAmmoPercent));
    }

    private IEnumerator ReloadWeaponRoutine(Weapon weapon, int topUpAmmoPercent)
    {
        weapon.IsWeaponReloading = true;

        while(weapon.WeaponReloadTimer < weapon.WeaponDetails.weaponRealoadTime)
        {
            weapon.WeaponReloadTimer += Time.deltaTime;
            yield return null;
        }

        if(topUpAmmoPercent != 0)
        {
            int ammoIncrease = Mathf.RoundToInt((weapon.WeaponDetails.weaponAmmoCapacity * topUpAmmoPercent) / 100f);

            int totalAmmo = weapon.WeaponRemainingAmmo + ammoIncrease;

            if(totalAmmo > weapon.WeaponDetails.weaponAmmoCapacity)
            {
                weapon.WeaponRemainingAmmo = weapon.WeaponDetails.weaponAmmoCapacity;
            }
            else
            {
                weapon.WeaponRemainingAmmo = totalAmmo;
            }
        }

        if (weapon.WeaponDetails.hasInfiniteAmmo)
        {
            weapon.WeaponClipRemainingAmmo = weapon.WeaponDetails.weaponClipAmmoCapacity;
        }
        else if(weapon.WeaponRemainingAmmo >= weapon.WeaponDetails.weaponClipAmmoCapacity)
        {
            weapon.WeaponClipRemainingAmmo = weapon.WeaponDetails.weaponClipAmmoCapacity;
        }
        else
        {
            weapon.WeaponClipRemainingAmmo = weapon.WeaponRemainingAmmo;
        }

        weapon.WeaponReloadTimer = 0f;
        weapon.IsWeaponReloading = false;

        _weaponReloadedEvent.CallOnWeaponReloadedEvent(weapon);
    }

    private void SetActiveWeaponEvent_OnSetActiveWeapon(SetActiveWeaponEvent setActiveWeaponEvent,
        SetActiveWeaponEventArgs setActiveWeaponEventArgs)
    {
        if (setActiveWeaponEventArgs.Weapon.IsWeaponReloading)
        {
            if(_reloadWeaponCoroutine != null)
            {
                StopCoroutine(_reloadWeaponCoroutine);
            }

            _reloadWeaponCoroutine = StartCoroutine(ReloadWeaponRoutine(setActiveWeaponEventArgs.Weapon, 0));
        }
    }
}
