using UnityEngine;

[CreateAssetMenu(fileName = "WeaponDetails_", menuName = "Scriptable Objects/Weapons/Weapon Details")]
public class WeaponDetailsSO : ScriptableObject
{
    public string weaponName;
    public Sprite weaponSprite;
    public Vector3 weaponShootPosition;
    public AmmoDetailsSO weaponCurrentAmmo;
    public bool hasInfiniteAmmo = false;
    public bool hasInfinityClipCapacity = false;
    public int weaponClipAmmoCapacity = 6;
    public int weaponAmmoCapacity;
    public float weaponFireRate = 0.2f;
    public float weaponPrechargeTime = 0f;
    public float weaponRealoadTime = 0f;

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(weaponName), weaponName);
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponCurrentAmmo), weaponCurrentAmmo);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(weaponFireRate), weaponFireRate,false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(weaponPrechargeTime), weaponPrechargeTime, true);

        if (!hasInfiniteAmmo)
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(weaponAmmoCapacity), weaponAmmoCapacity, false);

        if (!hasInfinityClipCapacity)
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(weaponClipAmmoCapacity), weaponClipAmmoCapacity, false);
    }
#endif
}
