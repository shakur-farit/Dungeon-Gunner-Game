using System;
using UnityEngine;

[DisallowMultipleComponent]
public class ReloadWeaponEvent : MonoBehaviour
{
    public event Action<ReloadWeaponEvent, ReloadWeaponEventArgs> OnReloadWeapon;

    public void CallOnRealoadWeaponEvent(Weapon weapon, int topUpAmmountPercent)
    {
        OnReloadWeapon?.Invoke(this, new ReloadWeaponEventArgs()
        {
            Weapon = weapon,
            TopUpAmmoPercent = topUpAmmountPercent
        });
    }
}

public class ReloadWeaponEventArgs : EventArgs
{
    public Weapon Weapon;
    public int TopUpAmmoPercent;
}
