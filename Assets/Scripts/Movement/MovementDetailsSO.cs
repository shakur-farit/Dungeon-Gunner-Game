using UnityEngine;

[CreateAssetMenu(fileName = "MovementDetails_", menuName = "Scriptable Objects/Movement/Movement Details")]
public class MovementDetailsSO : ScriptableObject
{
    public float minMovementSpeed = 8f;
    public float maxMovementSpeed = 8f;
    public float rollSpeed;
    public float rollDistance;
    public float rollCooldownTime;

    public float GetMovementSpeed
    {
        get 
        {
            if (minMovementSpeed == maxMovementSpeed)
                return minMovementSpeed;
            
            return Random.Range(minMovementSpeed, maxMovementSpeed);
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(minMovementSpeed), minMovementSpeed,
            nameof(maxMovementSpeed), maxMovementSpeed, false);

        if (rollSpeed != 0f || rollDistance != 0 || rollCooldownTime != 0)
        {
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(rollDistance), rollDistance, false);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(rollSpeed), rollSpeed, false);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(rollCooldownTime), rollCooldownTime, false);
        }
    }
#endif
}
