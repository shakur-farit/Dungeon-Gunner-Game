using UnityEngine;

[CreateAssetMenu(fileName = "AmmoHitEffect_", menuName = "Scriptable Objects/Weapons/Ammo Hit Effect")]
public class AmmoHitEffectSO : ScriptableObject
{
    [Header("Ammo Hit Effect Deatails")]
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
    public GameObject AmmoHitEffectPrefab;

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(Duration), Duration, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(StartParticleSize), StartParticleSize, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(StartParticleSpeed), StartParticleSpeed, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(StartLifetime), StartLifetime, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(MaxParticleNumber), MaxParticleNumber, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(EmissionRate), EmissionRate, true);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(BurstParticleNumber), BurstParticleNumber, false);
        HelperUtilities.ValidateCheckNullValue(this, nameof(AmmoHitEffectPrefab), AmmoHitEffectPrefab);
    }
#endif
}
