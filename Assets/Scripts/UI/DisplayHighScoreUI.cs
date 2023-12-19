using UnityEngine;

public class DisplayHighScoreUI : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private Transform _contentAnchorTransform;

    private void Start()
    {
        DisplayScores();
    }

    private void DisplayScores()
    {
        HighScores highScores = HighScoreManager.Instance.GetHighScore();
        GameObject scoreGameObject;

        int rank = 0;
        foreach (var score in highScores.ScoresList)
        {
            rank++;

            scoreGameObject = Instantiate(GameResources.Instance.scorePrefab, _contentAnchorTransform);

            ScorePrefab scorePrefab = scoreGameObject.GetComponent<ScorePrefab>();

            scorePrefab.RankTMP.text = rank.ToString();
            scorePrefab.NameTMP.text = score.PlayerName;
            scorePrefab.LevelTMP.text = score.LevelDescritpion;
            scorePrefab.ScoreTMP.text = score.PlayerScore.ToString("###,###0");
        }

        scoreGameObject = Instantiate(GameResources.Instance.scorePrefab, _contentAnchorTransform);
    }
}
