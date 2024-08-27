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
    None, Up, Down, Left, Right, UpLeft, UpRight, DownLeft, DownRight
}

public enum AttackType
{
    None, A, Q, W, E, R
}

public enum JumpType
{
    None, Single, SingleJump, Double, DoubleJump
}

#endregion

#region Controller

public interface IBaseController
{
    IBaseCamera Camera { get; }
    IBaseStage  Stage  { get; }
    IBasePlayer Player { get; }
    IBaseUI UI { get; }
}

public abstract class BaseController : MonoBehaviour, IBaseController
{
    [SerializeField]
    protected BaseCamera _baseCamera;
    [SerializeField]
    protected BaseStage  _baseStage;
    [SerializeField]
    protected BasePlayer _basePlayer;
    [SerializeField]
    protected BaseUI _baseUI;

    public IBaseCamera Camera => _baseCamera;
    public IBaseStage Stage   => _baseStage;
    public IBasePlayer Player => _basePlayer;
    public IBaseUI UI => _baseUI;

    void Awake()
    {
        List<PlayBase> list = new () { _baseCamera, _baseStage, _basePlayer, _baseUI };

        for (int i = 0; i < list.Count; i++)
        {
            list[i].SetController(this);
        }
    }
}

public abstract class PlayBase : MonoBehaviour
{
    protected IBaseController _controller;

    public void SetController(IBaseController controller)
    {
        _controller = controller;
    }
}

#endregion

#region Camera

public abstract class BaseCamera : PlayBase, IBaseCamera 
{
    public abstract void MoveCamera(MoveType moveType);
}

public interface IBaseCamera
{
    // from Player
    void MoveCamera(MoveType moveType);
}

#endregion

#region Stage

public interface IBaseStage
{
    bool CanClimb(Transform playerTransform);

    // bool CanJumpDown();

    /*
    // Player
    void SetTerrain(Transform playerTransform, 점프 위치 jumpType = JumpType.Ground);
    */
}

public abstract class BaseStage : PlayBase, IBaseStage
{
    public abstract bool CanClimb(Transform playerTransform);

    // public abstract bool CanJumpDown();
}

#endregion

public interface IBasePlayer
{
    Transform Transform { get; } // <- Camera, Stage

    JumpType  JumpType  { get; } // <- UI

    void Control(MoveType moveType, AttackType attackType, JumpType jumpType); // <- UI
}

#region Player

public abstract class BasePlayer : PlayBase, IBasePlayer
{
    public abstract Transform Transform { get; }

    public abstract JumpType  JumpType  { get; }

    public abstract void Control(MoveType moveType, AttackType attackType, JumpType jumpType);
}

#endregion

#region UI

public interface IBaseUI
{
    /*
    void SetTimer(float timer);

    void SetHit(int damage);

    void SetDamage(int damage);
    */
}

public abstract class BaseUI : PlayBase, IBaseUI
{
    
}

#endregion
