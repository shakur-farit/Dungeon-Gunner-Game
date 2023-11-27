using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    private TextMeshProUGUI _scoreText;

    private void Awake()
    {
        _scoreText = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        StaticEventHandler.OnScoreChanged += CallStaticEventHandler_OnScoredChanged;
    }

    private void OnDisable()
    {
        StaticEventHandler.OnScoreChanged -= CallStaticEventHandler_OnScoredChanged;
    }

    private void CallStaticEventHandler_OnScoredChanged(ScoreChangedArgs scoreChangedArgs)
    {
        _scoreText.text = "SCORE: " + scoreChangedArgs.Score.ToString("###,###0") +
            "   MULTIPLIER: x" + scoreChangedArgs.Multiplier;
    }
}
