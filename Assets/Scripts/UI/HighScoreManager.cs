using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class HighScoreManager : SingletonMonobehaviour<HighScoreManager>
{
    private HighScores _highScores = new HighScores();

    protected override void Awake()
    {
        base.Awake();

        LoadScores();
    }

    private void LoadScores()
    {
        BinaryFormatter bf = new BinaryFormatter();

        if(File.Exists(Application.persistentDataPath + "/DungeonGunnerHighScore.dat"))
        {
            CleareScoreList();

            FileStream file = File.OpenRead(Application.persistentDataPath + "/DungeonGunnerHighScore.dat");

            _highScores = (HighScores)bf.Deserialize(file);

            file.Close();
        }
    }

    private void CleareScoreList()
    {
        _highScores.ScoresList.Clear();
    }

    public void AddScore(Score score, int rank)
    {
        _highScores.ScoresList.Insert(rank - 1, score);

        if(_highScores.ScoresList.Count > Settings.numberOfHighScoresToSave)
        {
            _highScores.ScoresList.RemoveAt(Settings.numberOfHighScoresToSave);
        }

        SaveScore();
    }

    private void SaveScore()
    {
        BinaryFormatter bf = new BinaryFormatter();

        FileStream file = File.Create(Application.persistentDataPath + "/DungeonGunnerHighScore.dat");

        bf.Serialize(file, _highScores);

        file.Close();
    }

    public HighScores GetHighScore() => _highScores;

    public int GetRank(long playerScore)
    {
        if (_highScores.ScoresList.Count == 0)
            return 1;

        int index = 0;

        for (int i = 0; i < _highScores.ScoresList.Count; i++)
        {
            index++;

            if (playerScore >= _highScores.ScoresList[i].PlayerScore)
                return index;
        }

        if (_highScores.ScoresList.Count < Settings.numberOfHighScoresToSave)
            return (index + 1);

        return 0;
    }
}
