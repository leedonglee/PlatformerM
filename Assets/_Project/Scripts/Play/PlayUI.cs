using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Cysharp.Threading.Tasks;

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
    [SerializeField]
    private Button _quitButton;
    [SerializeField]
    private Button _modeButton;

    [Header("Quit Popup")]
    [SerializeField]
    private GameObject _quitPopup;
    [SerializeField]
    private Button _cancelButton;
    [SerializeField]
    private Button _okButton;

    [Header("Mode")]
    [SerializeField]
    private TextMeshProUGUI _modeText;

    [Header("Fade")]
    [SerializeField]
    private CanvasGroup _fadeUI;

    private bool _mobileMode = false;

    void Start()
    {
        _quitButton.onClick.AddListener(OnClickQuit);
        _modeButton.onClick.AddListener(OnClickMode);
        _cancelButton.onClick.AddListener(OnClickCancel);
        _okButton.onClick.AddListener(OnClickOK);

        if (!_mobileMode)
        {
            _mobilePad.gameObject.SetActive(false);

            if (!_mobileMode)
            {
                _modeText.text = "PC";
            }
            else
            {
                _modeText.text = "모바일";
            }
        }
    }

    void Update()
    {
        PlayerControl();
    }

    public override async void FadeIn()
    {
        float alpha = 1f;

        while (alpha > 0f)
        {
            alpha -= Time.deltaTime;
            _fadeUI.alpha = alpha;

            await UniTask.Yield();
        }

        _fadeUI.gameObject.SetActive(false);
    }

    private void PlayerControl()
    {
        MoveType moveType;

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
        JumpType jumpType;

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

    private void OnClickQuit()
    {
        SoundManager.Instance?.PlaySound(SoundType.Button);

        _quitPopup.SetActive(true);
    }

    private void OnClickMode()
    {
        SoundManager.Instance?.PlaySound(SoundType.Button);

        _mobileMode = !_mobileMode;
        _mobilePad.gameObject.SetActive(_mobileMode);

        if (!_mobileMode)
        {
            _modeText.text = "PC";
        }
        else
        {
            _modeText.text = "모바일";
        }
    }

    private void OnClickCancel()
    {
        SoundManager.Instance?.PlaySound(SoundType.Button);

        _quitPopup.SetActive(false);
    }

    private void OnClickOK()
    {
        SoundManager.Instance?.PlaySound(SoundType.Button);

        _fadeUI.alpha = 0f;
        _fadeUI.gameObject.SetActive(true);
        _quitPopup.SetActive(false);
        FadeOut();
    }

    private async void FadeOut()
    {
        float alpha = 0f;

        while (alpha < 1f)
        {
            alpha += Time.deltaTime;
            _fadeUI.alpha = alpha;

            await UniTask.Yield();
        }

        _controller.QuitGame();
    }

}
