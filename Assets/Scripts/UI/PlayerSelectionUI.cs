using UnityEngine;

public class PlayerSelectionUI : MonoBehaviour
{
    public SpriteRenderer PlayerHandSpriteRenderer;
    public SpriteRenderer PlayerHandNoWeaponSpriteRenderer;
    public SpriteRenderer PlayerWeaponSpriteRenderer;
    public Animator PlayerAnimator;

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(PlayerHandSpriteRenderer), PlayerHandSpriteRenderer);
        HelperUtilities.ValidateCheckNullValue(this, nameof(PlayerHandNoWeaponSpriteRenderer), PlayerHandNoWeaponSpriteRenderer);
        HelperUtilities.ValidateCheckNullValue(this, nameof(PlayerWeaponSpriteRenderer), PlayerWeaponSpriteRenderer);
        HelperUtilities.ValidateCheckNullValue(this, nameof(PlayerAnimator), PlayerAnimator);
    }
#endif
}
