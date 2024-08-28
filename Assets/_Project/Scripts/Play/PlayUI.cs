using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayUI : BaseUI
{
    void Update()
    {
        PlayerControl();
    }

    void PlayerControl()
    {
        MoveType moveType = MoveType.None;

        // Move
        if (Input.GetKey(KeyCode.UpArrow))
        {
            moveType = MoveType.Up;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            moveType = MoveType.Down;
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            if (moveType == MoveType.Up)
            {
                moveType = MoveType.UpLeft;
            }
            else if (moveType == MoveType.Down)
            {
                moveType = MoveType.DownLeft;
            }
            else
            {
                moveType = MoveType.Left;
            }
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            if (moveType == MoveType.Up)
            {
                moveType = MoveType.UpRight;
            }
            else if (moveType == MoveType.Down)
            {
                moveType = MoveType.DownRight;
            }
            else
            {
                moveType = MoveType.Right;
            }
        }
        
        
        // Attack
        AttackType attackType = AttackType.None;

        /*
        if (Input.GetKey(KeyCode.A))
        {
            attackType = AttackType.A;
        }
        */

        // Jump
        JumpType jumpType = JumpType.None;

        if (Input.GetKey(KeyCode.LeftAlt))
        {
            if (_controller.Player.JumpType == JumpType.None)
            {
                jumpType = JumpType.Single;
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            if (_controller.Player.JumpType == JumpType.SingleJump)
            {
                jumpType = JumpType.Double;
            }
        }

        _controller.Player.Control(moveType, attackType, jumpType);
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