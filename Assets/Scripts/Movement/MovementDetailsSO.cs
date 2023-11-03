using UnityEngine;

[CreateAssetMenu(fileName = "MovementDetails_", menuName = "Scriptable Objects/Movement/Movement Details")]
public class MovementDetailsSO : ScriptableObject
{
    public float minMoveSpeed = 8f;
    public float maxMoveSpeed = 8f;
    public float rollSpeed;
    public float rollDistance;
    public float rollCooldownTime;

    public float GetMoveSpeed()
    {
        if (minMoveSpeed == maxMoveSpeed)
            return minMoveSpeed;
        else
            return Random.Range(minMoveSpeed, maxMoveSpeed);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(minMoveSpeed), minMoveSpeed,
            nameof(maxMoveSpeed), maxMoveSpeed, false);

        if (rollSpeed != 0f || rollDistance != 0 || rollCooldownTime != 0)
        {
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(rollDistance), rollDistance, false);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(rollSpeed), rollSpeed, false);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(rollCooldownTime), rollCooldownTime, false);
        }
    }
#endif
}
