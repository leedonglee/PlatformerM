using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundType
{
    Button, Jump
}

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField]
    private AudioSource _musicAudioSource;
    [SerializeField]
    private AudioSource[] _soundAudioSources;

    [Header("Sound")]
    [SerializeField]
    private AudioClip _buttonSound;
    [SerializeField]
    private AudioClip _jumpSound;

    public void PlayMusic(AudioClip audioClip)
    {
        _musicAudioSource.clip = audioClip;
        _musicAudioSource.Play();
    }

    public void PlaySound(SoundType soundType)
    {
        AudioClip audioClip = null;

        switch (soundType)
        {
            case SoundType.Button:
                audioClip = _buttonSound;
                break;
            case SoundType.Jump:
                audioClip = _jumpSound;
                break;
        }

        for (int i = 0; i < _soundAudioSources.Length; i++)
        {
            if (_soundAudioSources[i].isPlaying)
            {
                continue;
            }
            else
            {
                _soundAudioSources[i].clip = audioClip;

                if (_soundAudioSources[i].clip != null)
                {
                    _soundAudioSources[i].Play();
                }
            }
        }
    }
}
