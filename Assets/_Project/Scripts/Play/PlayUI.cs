using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayUI : BaseUI
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

/*

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

*/


/*
    public abstract class IUIPad : MonoBehaviour
    {
        public enum UIPadButtonType
        {
            None, Attack, Jump
        }

        public abstract class IUIPadStick : MonoBehaviour, IDragHandler 
        {
            public event Action<StickState> OnStickEvent;

            public abstract void OnDrag(PointerEventData eventData);
        }

        public abstract class IUIPadButton : MonoBehaviour, IPointerDownHandler
        {
            protected UIPadButtonType _buttonType;

            public event Action<bool> OnButtonEvent;

            public abstract void OnPointerDown(PointerEventData eventData);
        }

        public event Action<StickState, bool, bool> OnPadEvent;
    }

    [SerializeField]
    protected IUIPad _pad;

*/