using System;
using UnityEngine;

[DisallowMultipleComponent]
public class AimWeaponEvents : MonoBehaviour
{
    public event Action<AimWeaponEvents, AimWeaponEventArgs> OnWeaponAim;
}
