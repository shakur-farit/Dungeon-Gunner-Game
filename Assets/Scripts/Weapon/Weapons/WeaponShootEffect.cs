using System;
using UnityEngine;

[DisallowMultipleComponent]
public class WeaponShootEffect : MonoBehaviour
{
    private ParticleSystem _shootEffectParticleSystem;

    private void Awake()
    {
        _shootEffectParticleSystem = GetComponent<ParticleSystem>();
    }

    public void SetShootEffect(WeaponShootEffectSO shootEffect, float aimAngle)
    {
        SetShootEffectColorGradient(shootEffect.ColorGradient);

        SetShootEffectParticleStartingValues(shootEffect.Duration, shootEffect.StartParticleSize,
            shootEffect.StartParticleSpeed, shootEffect.StartLifetime, shootEffect.EffectGravity,
            shootEffect.MaxParticleNumber);

        SetShooEffectParticaleEmission(shootEffect.EmissionRate, shootEffect.BurstParticleNumber);

        SetEmissionRotation(aimAngle);

        SetShootEffecPaticaleSprite(shootEffect.EffectSprite);

        SetShootEffectVelocityOverLifetime(shootEffect.VelocityOverLifetimeMin, shootEffect.VelocityOverLifetimeMax);
    }

    private void SetShootEffectColorGradient(Gradient colorGradient)
    {
        ParticleSystem.ColorOverLifetimeModule colorOverLifetimeModule = _shootEffectParticleSystem.colorOverLifetime;
        colorOverLifetimeModule.color = colorGradient;
    }

    private void SetShootEffectParticleStartingValues(float duration, float startParticleSize, 
        float startParticleSpeed, float startLifetime, float effectGravity, int maxParticleNumber)
    {
        ParticleSystem.MainModule  mainModule = _shootEffectParticleSystem.main;
        mainModule.duration = duration;
        mainModule.startSize = startParticleSize;
        mainModule.startSpeed = startParticleSize;
        mainModule.startLifetime = startLifetime;
        mainModule.gravityModifier = effectGravity;
        mainModule.maxParticles = maxParticleNumber;
    }

    private void SetShooEffectParticaleEmission(int emissionRate, int burstParticleNumber)
    {
        ParticleSystem.EmissionModule emissionModule = _shootEffectParticleSystem.emission;

        ParticleSystem.Burst burst = new ParticleSystem.Burst(0f, burstParticleNumber);
        emissionModule.SetBurst(0, burst);

        emissionModule.rateOverTime = emissionRate;
    }

    private void SetEmissionRotation(float aimAngle)
    {
        transform.eulerAngles = new Vector3(0f, 0f, aimAngle);
    }

    private void SetShootEffecPaticaleSprite(Sprite effectSprite)
    {
        ParticleSystem.TextureSheetAnimationModule textureSheetAnimationModule = 
            _shootEffectParticleSystem.textureSheetAnimation;

        textureSheetAnimationModule.SetSprite(0, effectSprite);
    }

    private void SetShootEffectVelocityOverLifetime(Vector3 velocityOverLifetimeMin, Vector3 velocityOverLifetimeMax)
    {
        ParticleSystem.VelocityOverLifetimeModule velocityOverLifetimeModule = 
            _shootEffectParticleSystem.velocityOverLifetime;

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
