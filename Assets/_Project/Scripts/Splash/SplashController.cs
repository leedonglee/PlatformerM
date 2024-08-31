using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Cysharp.Threading.Tasks;

public class SplashController : MonoBehaviour
{
    [Header("BGM")]
    [SerializeField]
    private AudioClip _audioClip;

    [Header("UI")]
    [SerializeField]
    private RectTransform _titleRT;
    [SerializeField]
    private CanvasGroup _noticeUI;
    [SerializeField]
    private Button _startButton;
    [SerializeField]
    private CanvasGroup _startTextUI;
    [SerializeField]
    private CanvasGroup _fadeUI;

    private bool _isStarted = false;

    void Start()
    {
        SoundManager.Instance?.PlayMusic(_audioClip);

        _startButton.onClick.AddListener(() =>
        {
            SoundManager.Instance?.PlaySound(SoundType.Button);

            _fadeUI.gameObject.SetActive(true);
            MoveScene();
        });

        StartSplash();
    }

    private async void StartSplash()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(2.0f), ignoreTimeScale: false);

        await ShowTitle();

        await ShowNotice();

        await ShowStartText();
    }

    private async UniTask ShowTitle()
    {
        Vector2 startPos = _titleRT.anchoredPosition;
        Vector2 endPos = new Vector2(startPos.x, -200f);
        float elapsedTime = 0f;

        while (elapsedTime < 0.5f)
        {
            _titleRT.anchoredPosition = Vector2.Lerp(startPos, endPos, elapsedTime / 0.5f);
            elapsedTime += Time.deltaTime;

            await UniTask.Yield();
        }

        startPos = _titleRT.anchoredPosition;
        endPos = new Vector2(startPos.x, -100f);
        elapsedTime = 0f;

        while (elapsedTime < 0.5f)
        {
            _titleRT.anchoredPosition = Vector2.Lerp(startPos, endPos, elapsedTime / 0.5f);
            elapsedTime += Time.deltaTime;

            await UniTask.Yield();
        }

        startPos = _titleRT.anchoredPosition;
        endPos = new Vector2(startPos.x, -140f);
        elapsedTime = 0f;

        while (elapsedTime < 0.5f)
        {
            _titleRT.anchoredPosition = Vector2.Lerp(startPos, endPos, elapsedTime / 0.5f);
            elapsedTime += Time.deltaTime;

            await UniTask.Yield();
        }
    }

    private async UniTask ShowNotice()
    {
        float alpha = 0f;

        while (alpha < 1f)
        {
            alpha += Time.deltaTime;
            alpha = Mathf.Clamp(alpha, 0f, 1f);
            _noticeUI.alpha = alpha;

            await UniTask.Yield();
        }

        await UniTask.Delay(TimeSpan.FromSeconds(2.0f), ignoreTimeScale: false);

        while (alpha > 0f)
        {
            alpha -= Time.deltaTime;
            alpha = Mathf.Clamp(alpha, 0f, 1f);
            _noticeUI.alpha = alpha;

            await UniTask.Yield();
        }

        _noticeUI.gameObject.SetActive(false);
    }

    private async UniTask ShowStartText()
    {
        _startButton.gameObject.SetActive(true);

        while (true)
        {
            float alpha = 0f;

            while (alpha < 1f && !_isStarted)
            {
                alpha += Time.deltaTime;
                alpha = Mathf.Clamp(alpha, 0f, 1f);
                _startTextUI.alpha = alpha;

                await UniTask.Yield();
            }

            while (alpha > 0f && !_isStarted)
            {
                alpha -= Time.deltaTime;
                alpha = Mathf.Clamp(alpha, 0f, 1f);
                _startTextUI.alpha = alpha;

                await UniTask.Yield();
            }

            if (_isStarted)
            {
                break;
            }
        }
    }

    private async void MoveScene()
    {
        float alpha = 0f;

        while (alpha < 1f)
        {
            alpha += Time.deltaTime;
            alpha = Mathf.Clamp(alpha, 0f, 1f);
            _fadeUI.alpha = alpha;

            await UniTask.Yield();
        }

        _isStarted = true;

        await UniTask.Yield();

        SceneManager.LoadScene(2);
    }

}
