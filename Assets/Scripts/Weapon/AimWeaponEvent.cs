using System;
using UnityEngine;

[DisallowMultipleComponent]
public class AimWeaponEvent : MonoBehaviour
{
    public event Action<AimWeaponEvent, AimWeaponEventArgs> OnWeaponAim;

    public void CallAimWeaponEvent(AimDirection aimDirection, float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector)
    {
        OnWeaponAim?.Invoke(this, new AimWeaponEventArgs()
        {
            aimDirection = aimDirection,
            aimAngle = aimAngle,
            weaponAimAngle = weaponAimAngle,
            weaponDirectionVector = weaponAimDirectionVector
        });
    }
}
