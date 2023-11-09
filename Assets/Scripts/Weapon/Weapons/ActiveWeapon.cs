using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(SetActiveWeaponEvent))]
[DisallowMultipleComponent]
public class ActiveWeapon : MonoBehaviour
{
    [SerializeField] private SpriteRenderer weaponSpriteRenderer;
    [SerializeField] private PolygonCollider2D weaponPolygonCollider;
    [SerializeField] private Transform weaponShootPositionTransform;
    [SerializeField] private Transform weaponEffectPositionTransform;

    private SetActiveWeaponEvent setWeaponEvent;
    private Weapon currentWeapon;

    public AmmoDetailsSO GetCurrentAmmo => currentWeapon.WeaponDetails.weaponCurrentAmmo;
    public Weapon GetCurrentWeapon => currentWeapon;
    public Vector3 GetShootPosition => weaponShootPositionTransform.position;
    public Vector3 GetShootEffectPosition => weaponEffectPositionTransform.position;

    private void Awake()
    {
        setWeaponEvent = GetComponent<SetActiveWeaponEvent>();
    }

    private void OnEnable()
    {
        setWeaponEvent.OnSetActiveWeapon += SetWeaponEvent_OnSetActiveWeapon;   
    }

    private void OnDisable()
    {
        setWeaponEvent.OnSetActiveWeapon -= SetWeaponEvent_OnSetActiveWeapon;
    }

    private void SetWeaponEvent_OnSetActiveWeapon(SetActiveWeaponEvent setActiveWeaponEvent, 
        SetActiveWeaponEventArgs setActiveWeaponEventArgs)
    {
        SetWeapon(setActiveWeaponEventArgs.Weapon);
    }

    private void SetWeapon(Weapon weapon)
    {
        currentWeapon = weapon;

        weaponSpriteRenderer.sprite = currentWeapon.WeaponDetails.weaponSprite;

        if(weaponPolygonCollider != null && weaponSpriteRenderer.sprite != null)
        {
            List<Vector2> sprutePhysicsShapePointList = new List<Vector2>();
            weaponSpriteRenderer.sprite.GetPhysicsShape(0, sprutePhysicsShapePointList);

            weaponPolygonCollider.points = sprutePhysicsShapePointList.ToArray();
        }

        weaponShootPositionTransform.localPosition = currentWeapon.WeaponDetails.weaponShootPosition;
    }

    public void RemoveCurrentWeapon()
    {
        currentWeapon = null;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponSpriteRenderer), weaponSpriteRenderer);
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponPolygonCollider), weaponPolygonCollider);
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponShootPositionTransform),
            weaponShootPositionTransform);
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponEffectPositionTransform),
            weaponEffectPositionTransform);
    }
#endif
}
