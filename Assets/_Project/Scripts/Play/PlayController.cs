using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Cysharp.Threading.Tasks;

public class PlayController : BaseController
{
    [Header("BGM")]
    [SerializeField]
    private AudioClip _audioClip;

    void Start()
    {
        SoundManager.Instance?.PlayMusic(_audioClip);

        StartGame();
    }

    private async void StartGame()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f), ignoreTimeScale: false);

        UI.FadeIn();
    }

    public override void QuitGame()
    {
        SceneManager.LoadScene(1);
    }

}
