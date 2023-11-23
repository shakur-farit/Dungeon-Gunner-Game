using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDetails_", menuName = "Scriptable Objects/Enemy/Enemy Details")]
public class EnemyDetailsSO : ScriptableObject
{
    [Header("Base Enemy Details")]
    public string EnemyName;
    public GameObject EnemyPrefab;
    public float ChaseDistance = 40f;

    [Header("Enemy Material")]
    public Material EnemyStandartMaterial;

    [Header("Enemy Materialize Settings")]
    public float EnemyMaterializeTime;
    public Shader EnemyMaterializeShader;
    [ColorUsage (true, true)] public Color EnemyMaterializeColor;

    [Header("Enemy Weapon Details")]
    public WeaponDetailsSO EnenmyWeapon;
    public float FiringIntervalMin = 0.1f;
    public float FiringIntervalMax = 1f;
    public float FiringDurationMin = 1f;
    public float FiringDurationMax = 2f;
    public bool FiringLineOfSightRiquired;

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(EnemyName), EnemyName);
        HelperUtilities.ValidateCheckNullValue(this, nameof(EnemyPrefab), EnemyPrefab);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(ChaseDistance), ChaseDistance, false);
        HelperUtilities.ValidateCheckNullValue(this, nameof(EnemyStandartMaterial), EnemyStandartMaterial);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(EnemyMaterializeTime), EnemyMaterializeTime, true);
        HelperUtilities.ValidateCheckNullValue(this, nameof(EnemyMaterializeShader), EnemyMaterializeShader);
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(FiringIntervalMin), FiringIntervalMin,
            nameof(FiringIntervalMax), FiringIntervalMax, false);
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(FiringDurationMin), FiringDurationMin,
            nameof(FiringDurationMax), FiringDurationMax, false);
    }
#endif
}
