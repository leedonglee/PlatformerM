using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/*

- IGameController -> IGamePlayer, IGameUI
- IGamePlayer
- IGameUI
- IGameCamera -> IGamePlayer
- IGameStage  -> IGamePlayer

*/

public enum NotificationType
{
    None
}

public interface IControllable
{
    void Notify(NotificationType notificationType);
}

public abstract class GameBase : MonoBehaviour
{
    protected IControllable _controller;

    public IControllable Controller { set { _controller = value; } }
}

public enum StickState
{
    None, Top, Right, Bottom, Left
}

public abstract class IGamePlayer : GameBase
{
    protected int _killCount = 0;

    public int KillCount { get { return _killCount; } }
}

public abstract class IGameUI : GameBase
{
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

    protected virtual void Update()
    {
        
    }

    void SendEvent()
    {

    }
}



public abstract class IGameController : MonoBehaviour, IControllable
{
    [SerializeField]
    protected IGamePlayer _gamePlayer;
    [SerializeField]
    protected IGameUI _gameUI;

    void Awake()
    {
        _gamePlayer.Controller = this;
        _gameUI.Controller = this;
    }

    public void Notify(NotificationType notificationType)
    {
        
    }

}



public abstract class IGameCamera : MonoBehaviour
{
    [SerializeField]
    protected IGamePlayer _gamePlayer;
}

public abstract class IGameStage : MonoBehaviour
{
    [SerializeField]
    protected IGamePlayer _gamePlayer;


}

