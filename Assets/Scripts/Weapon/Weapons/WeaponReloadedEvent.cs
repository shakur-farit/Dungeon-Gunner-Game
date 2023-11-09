using System;
using UnityEngine;

[DisallowMultipleComponent]
public class WeaponReloadedEvent : MonoBehaviour
{
    public event Action<WeaponReloadedEvent, WeaponFiredEventArgs> OnWeaponReloaded;

    public void CallOnWeaponReloadedEvent(Weapon weapon)
    {
        OnWeaponReloaded?.Invoke(this, new WeaponFiredEventArgs()
        {
            Weapon = weapon
        });
    }
}

public class WeaponReloadedEventArgs : EventArgs
{
    public Weapon Weapon;
}
