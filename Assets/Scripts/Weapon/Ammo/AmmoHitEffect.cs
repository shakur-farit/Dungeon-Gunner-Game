using UnityEngine;

[DisallowMultipleComponent]
public class AmmoHitEffect : MonoBehaviour
{
    private ParticleSystem _ammoHitEffectParticleSystem;

    private void Awake()
    {
        _ammoHitEffectParticleSystem = GetComponent<ParticleSystem>();
    }

    public void SetHitEffect(AmmoHitEffectSO ammoHitEffect)
    {
        SetShootEffectColorGradient(ammoHitEffect.ColorGradient);

        SetShootEffectParticleStartingValues(ammoHitEffect.Duration, ammoHitEffect.StartParticleSize,
            ammoHitEffect.StartParticleSpeed, ammoHitEffect.StartLifetime, ammoHitEffect.EffectGravity,
            ammoHitEffect.MaxParticleNumber);

        SetShooEffectParticaleEmission(ammoHitEffect.EmissionRate, ammoHitEffect.BurstParticleNumber);

        SetShootEffecPaticaleSprite(ammoHitEffect.EffectSprite);

        SetShootEffectVelocityOverLifetime(ammoHitEffect.VelocityOverLifetimeMin, ammoHitEffect.VelocityOverLifetimeMax);
    }

    private void SetShootEffectColorGradient(Gradient colorGradient)
    {
        ParticleSystem.ColorOverLifetimeModule colorOverLifetimeModule = _ammoHitEffectParticleSystem.colorOverLifetime;
        colorOverLifetimeModule.color = colorGradient;
    }

    private void SetShootEffectParticleStartingValues(float duration, float startParticleSize,
        float startParticleSpeed, float startLifetime, float effectGravity, int maxParticleNumber)
    {
        ParticleSystem.MainModule mainModule = _ammoHitEffectParticleSystem.main;
        mainModule.duration = duration;
        mainModule.startSize = startParticleSize;
        mainModule.startSpeed = startParticleSpeed;
        mainModule.startLifetime = startLifetime;
        mainModule.gravityModifier = effectGravity;
        mainModule.maxParticles = maxParticleNumber;
    }

    private void SetShooEffectParticaleEmission(int emissionRate, int burstParticleNumber)
    {
        ParticleSystem.EmissionModule emissionModule = _ammoHitEffectParticleSystem.emission;

        ParticleSystem.Burst burst = new ParticleSystem.Burst(0f, burstParticleNumber);
        emissionModule.SetBurst(0, burst);

        emissionModule.rateOverTime = emissionRate;
    }

    private void SetShootEffecPaticaleSprite(Sprite effectSprite)
    {
        ParticleSystem.TextureSheetAnimationModule textureSheetAnimationModule =
            _ammoHitEffectParticleSystem.textureSheetAnimation;

        textureSheetAnimationModule.SetSprite(0, effectSprite);
    }

    private void SetShootEffectVelocityOverLifetime(Vector3 velocityOverLifetimeMin, Vector3 velocityOverLifetimeMax)
    {
        ParticleSystem.VelocityOverLifetimeModule velocityOverLifetimeModule =
            _ammoHitEffectParticleSystem.velocityOverLifetime;

        ParticleSystem.MinMaxCurve minMaxCurveX = new ParticleSystem.MinMaxCurve();
        minMaxCurveX.mode = ParticleSystemCurveMode.TwoConstants;
        minMaxCurveX.constantMin = velocityOverLifetimeMin.x;
        minMaxCurveX.constantMax = velocityOverLifetimeMax.x;
        velocityOverLifetimeModule.x = minMaxCurveX;

        ParticleSystem.MinMaxCurve minMaxCurveY = new ParticleSystem.MinMaxCurve();
        minMaxCurveY.mode = ParticleSystemCurveMode.TwoConstants;
        minMaxCurveY.constantMin = velocityOverLifetimeMin.y;
        minMaxCurveY.constantMax = velocityOverLifetimeMax.y;
        velocityOverLifetimeModule.y = minMaxCurveY;

        ParticleSystem.MinMaxCurve minMaxCurveZ = new ParticleSystem.MinMaxCurve();
        minMaxCurveZ.mode = ParticleSystemCurveMode.TwoConstants;
        minMaxCurveZ.constantMin = velocityOverLifetimeMin.z;
        minMaxCurveZ.constantMax = velocityOverLifetimeMax.z;
        velocityOverLifetimeModule.z = minMaxCurveZ;
    }
}
