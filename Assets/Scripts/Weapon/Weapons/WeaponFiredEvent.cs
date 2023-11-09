using System;
using UnityEngine;

[DisallowMultipleComponent]
public class WeaponFiredEvent : MonoBehaviour
{
    public event Action<WeaponFiredEvent, WeaponFiredEventArgs> OnWeaponFired;

    public void CallOnWeaponFiredEvent(Weapon weapon)
    {
        OnWeaponFired?.Invoke(this, new WeaponFiredEventArgs()
        {
            Weapon = weapon
        });
    }
}

public class WeaponFiredEventArgs : EventArgs
{
    public Weapon Weapon;
}
