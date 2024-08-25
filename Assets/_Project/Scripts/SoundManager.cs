using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField]
    private AudioSource _musicAudioSource;
    [SerializeField]
    private AudioSource[] _soundAudioSources;

    public void PlayMusic(AudioClip audioClip)
    {
        _musicAudioSource.clip = audioClip;
        _musicAudioSource.Play();
    }

    public void PlaySound(AudioClip audioClip)
    {
        for (int i = 0; i < _soundAudioSources.Length; i++)
        {
            if (_soundAudioSources[i].isPlaying)
            {
                continue;
            }
            else
            {
                _soundAudioSources[i].clip = audioClip;
            }
        }
    }
}
