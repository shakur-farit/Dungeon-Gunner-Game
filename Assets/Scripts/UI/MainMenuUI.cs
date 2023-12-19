using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] GameObject _playButton;
    [SerializeField] private GameObject _highScoreButton;
    [SerializeField] private GameObject _returnToMainMenuButton;
    [SerializeField] private GameObject _instructionsButton;
    [SerializeField] private GameObject _quitButton;

    private bool _isHighScoresSceneLoaded = false;
    private bool _isInstructionsLoaded = false;

    private const int CHARACTER_SELECTION_SCENE_INDEX = 1;
    private const int HIGH_SCORE_SCENE_INDEX = 2;
    private const int INSTRUCTIONS_SCENE_INDEX = 3;
    private const int MAIN_GAME_SCENE_INDEX = 4;

    private void Start()
    {
        MusicManager.Instance.PlayMusic(GameResources.Instance.mainMenuMusic, 0f, 2f);

        SceneManager.LoadScene(1, LoadSceneMode.Additive);

        _returnToMainMenuButton.SetActive(false);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(MAIN_GAME_SCENE_INDEX);
    }

    public void LoadHighScore()
    {
        _playButton.SetActive(false);
        _highScoreButton.SetActive(false);
        _instructionsButton.SetActive(false);
        _quitButton.SetActive(false);
        _isHighScoresSceneLoaded = true;

        SceneManager.UnloadSceneAsync(CHARACTER_SELECTION_SCENE_INDEX);

        _returnToMainMenuButton.SetActive(true);

        SceneManager.LoadScene(HIGH_SCORE_SCENE_INDEX, LoadSceneMode.Additive);
    }

    public void LoadInstructions()
    {
        _playButton.SetActive(false);
        _highScoreButton.SetActive(false);
        _instructionsButton.SetActive(false);
        _quitButton.SetActive(false);
        _isInstructionsLoaded = true;

        SceneManager.UnloadSceneAsync(CHARACTER_SELECTION_SCENE_INDEX);

        _returnToMainMenuButton.SetActive(true);

        SceneManager.LoadScene(INSTRUCTIONS_SCENE_INDEX, LoadSceneMode.Additive);
    }

    public void LoadCharacterSelector()
    {
        _returnToMainMenuButton.SetActive(false);

        if (_isHighScoresSceneLoaded)
        {
            SceneManager.UnloadSceneAsync(HIGH_SCORE_SCENE_INDEX);
            _isHighScoresSceneLoaded = false;
        }

        if (_isInstructionsLoaded)
        {
            SceneManager.UnloadSceneAsync(INSTRUCTIONS_SCENE_INDEX);
            _isInstructionsLoaded= false;
        }

        _playButton.SetActive(true);
        _highScoreButton.SetActive(true);
        _instructionsButton.SetActive(true);
        _quitButton.SetActive(true);

        SceneManager.LoadScene(CHARACTER_SELECTION_SCENE_INDEX, LoadSceneMode.Additive);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(_playButton), _playButton);
        HelperUtilities.ValidateCheckNullValue(this, nameof(_highScoreButton), _highScoreButton);
        HelperUtilities.ValidateCheckNullValue(this, nameof(_returnToMainMenuButton), _returnToMainMenuButton);
    }
#endif
}
