using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AimWeaponEvent))]
[DisallowMultipleComponent]
public class AimWeapon : MonoBehaviour
{
    [SerializeField] private Transform weaponRotationPointTransform;

    private AimWeaponEvent aimWeaponEvent;

    private void Awake()
    {
        aimWeaponEvent = GetComponent<AimWeaponEvent>();
    }

    private void OnEnable()
    {
        aimWeaponEvent.OnWeaponAim += AimWeaponEvent_OnWeaponAim;
    }

    private void OnDisable()
    {
        aimWeaponEvent.OnWeaponAim -= AimWeaponEvent_OnWeaponAim;
    }

    private void AimWeaponEvent_OnWeaponAim(AimWeaponEvent aimWeaponEvent, AimWeaponEventArgs aimWeaponEventArgs)
    {
        Aim(aimWeaponEventArgs.aimDirection, aimWeaponEventArgs.aimAngle);
    }

    private void Aim(AimDirection aimDirection, float aimAngle)
    {
        Dictionary<AimDirection, Vector3> AimDirectionScales = new Dictionary<AimDirection, Vector3>
        {
            { AimDirection.Left, new Vector3(1f, -1f, 0f) },
            { AimDirection.UpLeft, new Vector3(1f, -1f, 0f) },
            { AimDirection.Up, new Vector3(1f, 1f, 0f) },
            { AimDirection.UpRight, new Vector3(1f, 1f, 0f) },
            { AimDirection.Right, new Vector3(1f, 1f, 0f) },
            { AimDirection.Down, new Vector3(1f, 1f, 0f) }
        };

        weaponRotationPointTransform.eulerAngles = new Vector3(0f, 0f, aimAngle);

        if (AimDirectionScales.TryGetValue(aimDirection, out Vector3 scale))
        {
            weaponRotationPointTransform.localScale = scale;
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponRotationPointTransform), weaponRotationPointTransform);
    }
#endif
}
