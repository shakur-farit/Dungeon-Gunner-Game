using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class SoundEffectManager : SingletonMonobehaviour<SoundEffectManager>
{
    public int SoundsVolume = 8;

    private void OnDisable()
    {
        PlayerPrefs.SetInt("soundsVolume", SoundsVolume);
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("soundsVolume"))
            SoundsVolume = PlayerPrefs.GetInt("soundsVolume");

        SetSoundsVolume(SoundsVolume);
    }

    public void PlaySoundEffect(SoundEffectSO soundEffect)
    {
        SoundEffect sound = (SoundEffect)PoolManager.Instance.ReuseComponent(
            soundEffect.SoundPrefab, Vector3.zero, Quaternion.identity);
        sound.SetSound(soundEffect);
        sound.gameObject.SetActive(true);
        StartCoroutine(DisableSound(sound, soundEffect.SoundEffectClip.length));
    }

    public void IncreaseSoundsVolume()
    {
        int maxMusicVolume = 20;

        if (SoundsVolume >= maxMusicVolume)
            return;

        SoundsVolume += 1;

        SetSoundsVolume(SoundsVolume);
    }

    public void DecreaseSoundsVolume()
    {
        if (SoundsVolume <= 0)
            return;

        SoundsVolume -= 1;

        SetSoundsVolume(SoundsVolume);
    }

    private IEnumerator DisableSound(SoundEffect sound, float soundDuration)
    {
        yield return new WaitForSeconds(soundDuration);
        sound.gameObject.SetActive(false);
    }

    private void SetSoundsVolume(int soundsVolume)
    {
        float muteDecibels = -80f;

        if(SoundsVolume == 0)
        {
            GameResources.Instance.soundMasterMixerGroup.audioMixer.
                SetFloat("soundsVolume", muteDecibels);
            return;
        }

        GameResources.Instance.soundMasterMixerGroup.audioMixer
            .SetFloat("soundsVolume", HelperUtilities.LinearToDecibles(soundsVolume));
    }
}
