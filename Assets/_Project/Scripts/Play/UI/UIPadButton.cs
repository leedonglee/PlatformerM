using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIPadButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPadButton
{
    [SerializeField]
    private PadButtonType _buttonType;

    private bool _isDown = false;
    private bool _isPressed = false;

    public PadButtonType ButtonType => _buttonType;

    public bool IsDown 
    {
        get
        {
            if (_isDown)
            {
                _isDown = false;
                return true;
            }
            
            return false;
        }
    }

    public bool IsPressed => _isPressed;

    public void OnPointerDown(PointerEventData eventData)
    {
        _isDown = true;
        _isPressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isPressed = false;
    }

}
