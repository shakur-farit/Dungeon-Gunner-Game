using UnityEngine;

[CreateAssetMenu(fileName = "SoundsEffect_", menuName = "Scriptable Objects/Sounds/Sound Effect")]
public class SoundEffectSO : ScriptableObject
{
    [Header("Sound Effect Details")]
    public string SoundEffectName;
    public GameObject SoundPrefab;
    public AudioClip SoundEffectClip;
    [Range(0.1f, 1.5f)]
    public float SoundEffectPitchRandomVariantionMin = 0.8f;
    [Range(0.1f, 1.5f)]
    public float SoundEffectPitchRandomVariantionMax = 1.2f;
    [Range(0f, 1f)]
    public float SoundEffectVolume = 1f;

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(SoundEffectName), SoundEffectName);
        HelperUtilities.ValidateCheckNullValue(this, nameof(SoundPrefab), SoundPrefab);
        HelperUtilities.ValidateCheckNullValue(this, nameof(SoundEffectClip), SoundEffectClip);
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(SoundEffectPitchRandomVariantionMin), SoundEffectPitchRandomVariantionMin,
            nameof(SoundEffectPitchRandomVariantionMax), SoundEffectPitchRandomVariantionMax, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(SoundEffectVolume), SoundEffectVolume, true);
    }
#endif
}
