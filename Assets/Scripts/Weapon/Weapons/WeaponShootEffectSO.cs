using UnityEngine;

[CreateAssetMenu(fileName = "WeaponShootEffect_", menuName = "Scriptable Objects/Weapons/Weapon Shoot Effect")]
public class WeaponShootEffectSO : ScriptableObject
{
    [Header("Wepaon Shoot Effect Deatails")]
    public Gradient ColorGradient;
    public float Duration = 0.5f;
    public float StartParticleSize = 0.25f;
    public float StartParticleSpeed = 3f;
    public float StartLifetime = 0.5f;
    public int MaxParticleNumber = 100;
    public int EmissionRate = 100;
    public int BurstParticleNumber = 20;
    public float EffectGravity = -0.01f;
    public Sprite EffectSprite;
    public Vector3 VelocityOverLifetimeMin;
    public Vector3 VelocityOverLifetimeMax;
    public GameObject WeaponShootEffectPrefab;

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(Duration), Duration, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(StartParticleSize), StartParticleSize,false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(StartParticleSpeed), StartParticleSpeed,false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(StartLifetime), StartLifetime,false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(MaxParticleNumber), MaxParticleNumber,false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(EmissionRate), EmissionRate,false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(BurstParticleNumber), BurstParticleNumber,false);
        HelperUtilities.ValidateCheckNullValue(this, nameof(WeaponShootEffectPrefab), WeaponShootEffectPrefab);
    }
#endif
}
