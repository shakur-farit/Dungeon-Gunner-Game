using UnityEngine;
using TMPro;

public class ScorePrefab : MonoBehaviour
{
    public TextMeshProUGUI RankTMP; 
    public TextMeshProUGUI NameTMP; 
    public TextMeshProUGUI LevelTMP; 
    public TextMeshProUGUI ScoreTMP;

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(RankTMP), RankTMP);
        HelperUtilities.ValidateCheckNullValue(this, nameof(NameTMP), NameTMP);
        HelperUtilities.ValidateCheckNullValue(this, nameof(LevelTMP), LevelTMP);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ScoreTMP), ScoreTMP);
    }
#endif
}
