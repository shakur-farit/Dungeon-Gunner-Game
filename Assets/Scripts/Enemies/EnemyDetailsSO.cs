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

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(EnemyName), EnemyName);
        HelperUtilities.ValidateCheckNullValue(this, nameof(EnemyPrefab), EnemyPrefab);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(ChaseDistance), ChaseDistance, false);
        HelperUtilities.ValidateCheckNullValue(this, nameof(EnemyStandartMaterial), EnemyStandartMaterial);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(EnemyMaterializeTime), EnemyMaterializeTime, true);
        HelperUtilities.ValidateCheckNullValue(this, nameof(EnemyMaterializeShader), EnemyMaterializeShader);
    }
#endif
}
