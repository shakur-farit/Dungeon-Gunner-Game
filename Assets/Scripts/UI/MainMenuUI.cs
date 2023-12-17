using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    private void Start()
    {
        MusicManager.Instance.PlayMusic(GameResources.Instance.mainMenuMusic, 0f, 2f);

        SceneManager.LoadScene(1, LoadSceneMode.Additive);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(2);
    }
}
