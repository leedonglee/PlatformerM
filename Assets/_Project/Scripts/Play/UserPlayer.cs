using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserPlayer : BasePlayer
{
    private enum PlayerState
    {
        None, Climbing, Attacking, /* MovableAttacking */ // None : Standing, Walking, Jumping
    }

    [SerializeField]
    private Transform _playerFoot;

    private const float PLAYER_MAX_GRAVITY = -10f;
    private const float PLAYER_MOVE_SPEED = 3f;

    private Rigidbody2D _rigidbody;

    // Sprite
    private SpriteRenderer _spriteRenderer;

    // State
    private PlayerState _playerState = PlayerState.None;
    private MoveType _moveType = MoveType.None;
    private JumpType _jumpType = JumpType.None;

    public override Transform Transform => transform;

    public override JumpType  JumpType  => _jumpType;

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    /*
    public enum MoveType
    {
        None, Up, Down, Left, Right, UpLeft, UpRight, DownLeft, DownRight
    }
    */

    void FixedUpdate()
    {
        if (_playerState == PlayerState.Climbing)
        {
            _rigidbody.velocity = Vector2.zero;
            return;
        }

        // 점프 없이 낙하(공중에서 점프 금지)
        if (_rigidbody.velocity.y < -1f && _jumpType == JumpType.None)
        {
            _jumpType = JumpType.Falling;
        }

        // 최대 중력 고정
        if (_rigidbody.velocity.y < PLAYER_MAX_GRAVITY)
        {
            // TODO : 낙하 데미지 true
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, PLAYER_MAX_GRAVITY);
        }

        // Move
        if (_moveType != MoveType.None && _jumpType != JumpType.DoubleJump)
        {
            if (_moveType == MoveType.Left)
            {
                _rigidbody.velocity = new Vector2(-PLAYER_MOVE_SPEED, _rigidbody.velocity.y);
            }
            else if (_moveType == MoveType.Right)
            {
                _rigidbody.velocity = new Vector2(PLAYER_MOVE_SPEED, _rigidbody.velocity.y);
            }
        }

        // Jump
        if (_jumpType == JumpType.Single)
        {
            Debug.Log("Single Jump");

            _jumpType = JumpType.SingleJump;
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, 9f);
        }
        else if (_jumpType == JumpType.Double)
        {
            Debug.Log("Double Jump");

            _jumpType = JumpType.DoubleJump;

            float dashX = _spriteRenderer.flipX ? -10f : 10f;
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x + dashX, _rigidbody.velocity.y + 2f); // TODO : velocity.x 수정
        }
    }

    public override void Control(MoveType moveType, AttackType attackType, JumpType jumpType)
    {
        // PlayerState 우선순위 클라이밍 == 공격 >= 이동

        _moveType = moveType;
        _jumpType = jumpType;

        if (_moveType != MoveType.None)
        {
            if (_playerState == PlayerState.None)
            {
                // Climbing
                MoveType climbingType = MoveType.None;

                switch (_moveType)
                {
                    case MoveType.Up:
                    case MoveType.UpLeft:
                    case MoveType.UpRight:
                        climbingType = MoveType.Up;
                        break;
                    case MoveType.Down:
                    case MoveType.DownLeft:
                    case MoveType.DownRight:
                        climbingType = MoveType.Down;
                        break;
                }

                if (climbingType == MoveType.Up || climbingType == MoveType.Down)
                {
                    bool canClimb = _controller.Stage.CanClimb(_playerFoot, climbingType == MoveType.Up);

                    if (canClimb)
                    {
                        _playerState = PlayerState.Climbing;
                        gameObject.layer = LayerMask.NameToLayer(GameLayer.PLAYER_CLIMBING);

                        // 중력 비활성화
                        _rigidbody.gravityScale = 0f;
                        return;
                    }
                }

                // Move
                if (_playerState == PlayerState.None)
                {
                    switch (_moveType)
                    {
                        case MoveType.Left:
                        case MoveType.UpLeft:
                        case MoveType.DownLeft:
                            _moveType = MoveType.Left;
                            break;
                        case MoveType.Right:
                        case MoveType.UpRight:
                        case MoveType.DownRight:
                            _moveType = MoveType.Right;
                            break;
                    }
                    
                    if (_moveType == MoveType.Left || _moveType == MoveType.Right)
                    {
                        if (_moveType == MoveType.Left)
                        {
                            _spriteRenderer.flipX = true;
                            _controller.Camera.MoveCamera(MoveType.Left);
                        }
                        else
                        {
                            _spriteRenderer.flipX = false;
                            _controller.Camera.MoveCamera(MoveType.Right);                    
                        }
                    }
                }
            }
            else if (_playerState == PlayerState.Climbing)
            {
                // Jump
                if (_moveType != MoveType.Up && _moveType != MoveType.Down && _jumpType == JumpType.Single)
                {
                    _playerState = PlayerState.None;
                    gameObject.layer = LayerMask.NameToLayer(GameLayer.PLAYER);

                    // 중력 활성화
                    _rigidbody.gravityScale = 1f;

                    // Set Direction
                    if (_spriteRenderer.flipX)
                    {
                        _controller.Camera.MoveCamera(MoveType.Left);
                    }
                    else
                    {
                        _controller.Camera.MoveCamera(MoveType.Right);
                    }

                    ILadder ladder = _controller.Stage.GetPlayerLadder();

                    // 점프 후 같은 사다리 오르기 방지
                    ladder.CanClimb = false;
                    return;
                }
                else
                {
                    _jumpType = JumpType.None;
                }

                // Climbing
                MoveType climbingType = MoveType.None;

                switch (_moveType)
                {
                    case MoveType.Up:
                    case MoveType.UpLeft:
                    case MoveType.UpRight:
                        climbingType = MoveType.Up;
                        break;
                    case MoveType.Down:
                    case MoveType.DownLeft:
                    case MoveType.DownRight:
                        climbingType = MoveType.Down;
                        break;
                }

                if (climbingType == MoveType.Up || climbingType == MoveType.Down)
                {
                    bool canClimb = _controller.Stage.CanClimb(_playerFoot, climbingType == MoveType.Up);

                    if (canClimb)
                    {                        
                        if (climbingType == MoveType.Up)
                        {
                            transform.position = new Vector2(transform.position.x, transform.position.y + PLAYER_MOVE_SPEED * Time.deltaTime);
                            _controller.Camera.MoveCamera(MoveType.Up);
                        }
                        else
                        {
                            transform.position = new Vector2(transform.position.x, transform.position.y - PLAYER_MOVE_SPEED * Time.deltaTime);
                            _controller.Camera.MoveCamera(MoveType.Down);
                        }
                    }
                    else
                    {
                        _playerState = PlayerState.None;
                        gameObject.layer = LayerMask.NameToLayer(GameLayer.PLAYER);

                        // 중력 활성화
                        _rigidbody.gravityScale = 1f;

                        // Set Direction
                        if (_spriteRenderer.flipX)
                        {
                            _controller.Camera.MoveCamera(MoveType.Left);
                        }
                        else
                        {
                            _controller.Camera.MoveCamera(MoveType.Right);
                        }

                        ILadder ladder = _controller.Stage.GetPlayerLadder();

                        if (climbingType == MoveType.Up)
                        {
                            transform.position = new Vector2(transform.position.x, ladder.MaxY + Mathf.Abs(_playerFoot.localPosition.y) + 0.05f);
                        }
                    }
                }
            }
        }

        // Debug.Log(_playerState);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer(GameLayer.GROUND))
        {
            if (collision.gameObject.TryGetComponent(out BoxCollider2D boxCollider2D))
            {
                float groundPosition = collision.gameObject.transform.position.y + (boxCollider2D.size.y / 2f);

                // 점프 중 GROUND 충돌 체크
                if (_playerFoot.position.y > groundPosition - 0.05f)
                {
                    _jumpType = JumpType.None;
                }
            }
            else
            {
                _jumpType = JumpType.None;
            }
        }
    }

}
