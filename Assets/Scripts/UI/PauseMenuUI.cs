using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _musicLevelText;
    [SerializeField] private TextMeshProUGUI _soundsLevelText;

    private void OnEnable()
    {
        Time.timeScale = 0f;

        StartCoroutine(InitializeUI());
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void IncreaseMusicVolume()
    {
        MusicManager.Instance.IncreaseMusicVolume();
        _musicLevelText.SetText(MusicManager.Instance.MusicVolume.ToString());
    }

    public void DecreaseMusicVolume()
    {
        MusicManager.Instance.DecreaseMusicVolume();
        _musicLevelText.SetText(MusicManager.Instance.MusicVolume.ToString());
    }

    public void IncreaseSoundsVolume()
    {
        SoundEffectManager.Instance.IncreaseSoundsVolume();
        _soundsLevelText.SetText(SoundEffectManager.Instance.SoundsVolume.ToString());
    }

    public void DecreaseSoundsVolume()
    {
        SoundEffectManager.Instance.DecreaseSoundsVolume();
        _soundsLevelText.SetText(SoundEffectManager.Instance.SoundsVolume.ToString());
    }

    private IEnumerator InitializeUI()
    {
        yield return null;

        _soundsLevelText.SetText(SoundEffectManager.Instance.SoundsVolume.ToString());
        _musicLevelText.SetText(MusicManager.Instance.MusicVolume.ToString());
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(_musicLevelText), _musicLevelText);
        HelperUtilities.ValidateCheckNullValue(this, nameof(_soundsLevelText), _soundsLevelText);
    }
#endif
}
