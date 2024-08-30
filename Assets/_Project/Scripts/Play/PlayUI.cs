using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PadButtonType
{
    Jump
}

public struct PadButtonState
{
    public PadButtonState(bool isDown, bool isPressed)
    {
        this.isDown = isDown;
        this.isPressed = isPressed;
    }

    public bool isDown;
    public bool isPressed;
}

interface IMobilePad
{
    MoveType GetMoveType();

    PadButtonState GetPadButtonState(PadButtonType padButtonType);
}

public class PlayUI : BaseUI
{
    [SerializeField]
    private UIMobilePad _mobilePad;

    private bool _mobileMode = false;

    void Update()
    {
        PlayerControl();
    }

    void PlayerControl()
    {
        MoveType moveType = MoveType.None;

        if (!_mobileMode)
        {
            moveType = GetPCMoveType();
        }
        else
        {
            moveType = GetMobileMoveType();
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

        if (!_mobileMode)
        {
            jumpType = GetPCJumpType();
        }
        else
        {
            jumpType = GetMobileJumpType();
        }

        _controller.Player.Control(moveType, attackType, jumpType);
    }

#region PC

    private MoveType GetPCMoveType()
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

        return moveType;
    }

    private JumpType GetPCJumpType()
    {
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

        return jumpType;
    }

#endregion

#region Mobile

    private MoveType GetMobileMoveType()
    {
        return _mobilePad.GetMoveType();
    }

    private JumpType GetMobileJumpType()
    {
        JumpType jumpType = JumpType.None;

        PadButtonState padButtonState = _mobilePad.GetPadButtonState(PadButtonType.Jump);
        
        if (padButtonState.isPressed)
        {
            if (_controller.Player.JumpType == JumpType.None)
            {
                jumpType = JumpType.Single;
            }
        }

        if (padButtonState.isDown)
        {
            if (_controller.Player.JumpType == JumpType.SingleJump)
            {
                jumpType = JumpType.Double;
            }            
        }

        return jumpType;
    }

#endregion

}
