using System.Collections;
using UnityEngine;

public class MusicManager : SingletonMonobehaviour<MusicManager>
{
    public int MusicVolume = 10;

    private AudioSource _musicAudioSource = null;
    private AudioClip _currentAudioClip = null;
    private Coroutine _fadeOutMusicCoroutine;
    private Coroutine _fadeInMusicCoroutine;

    private void OnDisable()
    {
        PlayerPrefs.SetInt("musicVolume", MusicVolume);
    }

    protected override void Awake()
    {
        base.Awake();

        _musicAudioSource = GetComponent<AudioSource>();

        GameResources.Instance.musicOffSnapshot.TransitionTo(0f);
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("musicVolume"))
            MusicVolume = PlayerPrefs.GetInt("musicVolume");

        SetMusicVolume(MusicVolume);
    }

    public void IncreaseMusicVolume()
    {
        int maxMusicVolume = 20;

        if (MusicVolume >= maxMusicVolume)
            return;

        MusicVolume += 1;

        SetMusicVolume(MusicVolume);
    }

    public void DecreaseMusicVolume()
    {
        if (MusicVolume <= 0)
            return;

        MusicVolume -= 1;

        SetMusicVolume(MusicVolume);
    }

    public void SetMusicVolume(int musicVolume)
    {
        float muteDecibels = -80f;

        if(musicVolume == 0)
        {
            GameResources.Instance.musicMasterMixerGroup.audioMixer.SetFloat("musicVolume", muteDecibels);
            return;
        }

        GameResources.Instance.musicMasterMixerGroup.audioMixer.SetFloat("musicVolume",
            HelperUtilities.LinearToDecibles(musicVolume));
    }

    public void PlayMusic(MusicTrackSO musicTrack, float fadeOutTime = Settings.musicFadeOutTime,
        float fadeInTime = Settings.musicFadeInTime)
    {
        StartCoroutine(PlayMusicRoutine(musicTrack, fadeOutTime, fadeInTime));
    }

    private IEnumerator PlayMusicRoutine(MusicTrackSO musicTrack, float fadeOutTime, float fadeInTime)
    {
        if(_fadeOutMusicCoroutine != null)
        {
            StopCoroutine(_fadeOutMusicCoroutine);
        }

        if(_fadeInMusicCoroutine != null)
        {
            StopCoroutine(_fadeInMusicCoroutine);
        }

        if(musicTrack._trackClip != _currentAudioClip)
        {
            _currentAudioClip = musicTrack._trackClip;

            yield return _fadeOutMusicCoroutine = StartCoroutine(FadeOutMusic(fadeOutTime));

            yield return _fadeInMusicCoroutine = StartCoroutine(FadeInMusic(musicTrack, fadeInTime));
        }

        yield return null;
    }

    private IEnumerator FadeOutMusic(float fadeOutTime)
    {
        GameResources.Instance.musicLowSnapshot.TransitionTo(fadeOutTime);

        yield return new WaitForSeconds(fadeOutTime);
    }

    private IEnumerator FadeInMusic(MusicTrackSO musicTrack, float fadeInTime)
    {
        _musicAudioSource.clip = musicTrack._trackClip;
        _musicAudioSource.volume = musicTrack._trackVolume;
        _musicAudioSource.Play();

        GameResources.Instance.musicOnFullSnapshot.TransitionTo(fadeInTime);

        yield return new WaitForSeconds(fadeInTime);
    }
}
