using UnityEngine;

public class Environment : MonoBehaviour
{
    [Header("References")]
    public SpriteRenderer EnvironmentSpriteRenderer;

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(EnvironmentSpriteRenderer), EnvironmentSpriteRenderer);
    }
#endif
}
