using System;
using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class SoundEffectManager : SingletonMonobehaviour<SoundEffectManager>
{
    public int SoundsVolume = 8;

    private void Start()
    {
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
        }
        else
        {
            GameResources.Instance.soundMasterMixerGroup.audioMixer.
                SetFloat("soundsVolume",
                HelperUtilities.LinearToDecibles(soundsVolume));
        }
    }
}
