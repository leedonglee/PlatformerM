using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IJoystick
{
    MoveType StickMoveType { get; }
}

public interface IPadButton
{
    PadButtonType ButtonType { get; }
    bool IsDown { get; }
    bool IsPressed { get; }
}

public class UIMobilePad : MonoBehaviour, IMobilePad
{
    [SerializeField]
    private UIPadJoystick _joystick;

    [Header("Buttons")]
    [SerializeField]
    private UIPadButton[] _padButtons;

    public MoveType GetMoveType()
    {
        return _joystick.StickMoveType;
    }

    public PadButtonState GetPadButtonState(PadButtonType padButtonType)
    {
        for (int i = 0; i < _padButtons.Length; i++)
        {
            UIPadButton padButton = _padButtons[i];

            if (padButton.ButtonType == padButtonType)
            {
                return new PadButtonState(padButton.IsDown, padButton.IsPressed);
            }
        }

        return new PadButtonState(false, false);
    }

}
