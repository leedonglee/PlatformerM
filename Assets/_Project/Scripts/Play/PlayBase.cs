using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using Cysharp.Threading.Tasks;

/*

- BaseController

- BaseCamera : 카메라
- BaseStage : 스테이지 관리(게임 시간, 몬스터 생성, 아이템 등), 스테이지 지형지물 관리
- BasePlayer : 플레이어
- BaseUI : 타이머, 데미지 표시 등

*/

#region Enum

public enum MoveType
{
    None, Top, Bottom, Left, Right
}

public enum JumpType
{
    Ground, Ladder
}

#endregion

#region Interface

public interface IBaseController : IBaseCamera, IBaseStage, IBasePlayer, IBaseUI
{
    
}

public interface IBaseCamera
{
    // Top, Bottom은 캐릭터 중앙, Left, Right는 캐릭터 좌우측 이동
    void MoveCamera(MoveType moveType);
}

public interface IBaseStage
{
    /*
    void SetTerrain(Transform playerTransform, JumpType jumpType = JumpType.Ground);

    bool CanClimb();

    bool CanJumpDown();    
    */
}

public interface IBasePlayer
{
    Transform GetPlayerTransform();
}

public interface IBaseUI
{
    /*
    void InputEvent(MoveType moveDirection, bool attackState, bool jumpState);

    void SetTimer(float timer);

    void SetHit(int damage);

    void SetDamage(int damage);
    */
}

#endregion

#region Class

public abstract class BaseController : MonoBehaviour, IBaseController
{
    [SerializeField]
    protected BaseCamera _playCamera;
    [SerializeField]
    protected BaseStage _playStage;
    [SerializeField]
    protected BasePlayer _userPlayer;
    [SerializeField]
    protected BaseUI _playUI;

    void Awake()
    {
        List<PlayBase> list = new () { _playCamera, _playStage, _userPlayer, _playUI };

        for (int i = 0; i < list.Count; i++)
        {
            list[i].SetController(this);
        }
    }

    #region Camera

    public void MoveCamera(MoveType moveType)
    {
        _playCamera.MoveCamera(moveType);
    }

    #endregion

    #region 

    public Transform GetPlayerTransform()
    {
        return _userPlayer.GetPlayerTransform();
    }

    #endregion
}

public abstract class PlayBase : MonoBehaviour
{
    protected IBaseController _controller;

    public void SetController(IBaseController controller)
    {
        _controller = controller;
    }
}

public abstract class BaseCamera : PlayBase, IBaseCamera 
{
    public abstract void MoveCamera(MoveType moveType);
}

public abstract class BaseStage : PlayBase, IBaseStage
{

}

public abstract class BasePlayer : PlayBase, IBasePlayer
{
    public abstract Transform GetPlayerTransform();
}

public abstract class BaseUI : PlayBase, IBaseUI
{
    
}

#endregion
