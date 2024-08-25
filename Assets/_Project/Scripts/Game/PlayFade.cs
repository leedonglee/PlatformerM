using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cysharp.Threading.Tasks;

public class PlayFade : MonoBehaviour
{
    private CanvasGroup _canvasGroup;

    // alpha 1 -> 0    
    public async UniTask FadeIn()
    {
        float alpha = 1.0f;

        while (alpha > 0f)
        {
            alpha -= Time.deltaTime;
            alpha = Mathf.Clamp(alpha, 0f, 1f);
            _canvasGroup.alpha = alpha;

            await UniTask.Yield();
        }
        
        gameObject.SetActive(false);
    }

    // alpha 0 -> 1
    public async UniTask FadeOut()
    {
        gameObject.SetActive(true);

        float alpha = 0.0f;

        while (alpha < 1f)
        {
            alpha += Time.deltaTime;
            alpha = Mathf.Clamp(alpha, 0f, 1f);
            _canvasGroup.alpha = alpha;

            await UniTask.Yield();
        }
    }

}
