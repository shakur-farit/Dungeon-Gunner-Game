using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDetails_", menuName = "Scriptable Objects/Enemy/Enemy Details")]
public class EnemyDetailsSO : ScriptableObject
{
    [Header("Base Enemy Details")]
    public string EnemyName;
    public GameObject EnemyPrefab;
    public float ChaseDistance = 40f;

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(EnemyName), EnemyName);
        HelperUtilities.ValidateCheckNullValue(this, nameof(EnemyPrefab), EnemyPrefab);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(ChaseDistance), ChaseDistance, false);
    }
#endif
}
