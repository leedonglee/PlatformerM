using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayController : BaseController
{
    #region Camera

    public override void MoveCamera(MoveType moveType)
    {
        _playCamera.MoveCamera(moveType);
    }

    #endregion

    #region Stage

    #endregion

    #region Player

    public override Transform GetPlayerTransform()
    {
        return _userPlayer.GetPlayerTransform();
    }

    public override JumpType GetJumpType()
    {
        return _userPlayer.GetJumpType();
    }

    public override void InputEvent(MoveType moveType, AttackType attackType, JumpType jumpType)
    {
        _userPlayer.InputEvent(moveType, attackType, jumpType);
    }

    #endregion

    #region UI

    #endregion

}
