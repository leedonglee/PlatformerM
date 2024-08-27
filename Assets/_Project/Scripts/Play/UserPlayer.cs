using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserPlayer : BasePlayer
{
    private enum PlayerState
    {
        None, Climbing, Attacking
    }

    [SerializeField]
    private Transform _playerFoot;

    private const float PLAYER_MAX_GRAVITY = -10f;
    private const float PLAYER_MOVE_SPEED = 3f;

    private Rigidbody2D _rigidbody;

    // Sprite
    private SpriteRenderer _spriteRenderer;
    // private Vector3 _spriteMinPoint;

    // State
    private PlayerState _playerState = PlayerState.None;
    private MoveType _moveType = MoveType.None;
    private JumpType _jumpType = JumpType.None;
    
    // State Ladder
    private ILadder _currentLadder;
    private bool _ladderJump;

    // Platform LayerMask
    private int _platformLayerMask;

    public override Transform Transform => transform;

    public override JumpType  JumpType  => _jumpType;

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rigidbody = GetComponent<Rigidbody2D>();

        // _spriteMinPoint = _spriteRenderer.sprite.bounds.min;

        // Platform Layers
        string[] platformLayers = new string[2]; // Ground, Box, ...

        platformLayers[0] = "Ground";
        platformLayers[1] = "Box";

        _platformLayerMask = LayerMask.GetMask(platformLayers);
    }

    void Update()
    {
        /*
        Vector2 footPosition = new Vector2(transform.position.x, transform.position.y + _spriteMinPoint.y);

        RaycastHit2D hit = Physics2D.Raycast(footPosition, Vector2.down, Mathf.Infinity, _platformLayerMask);

        if (hit.collider != null)
        {
            // TODO : 수정(뭔가 좀 안맞음)
            // 땅 착지 판단
            if (Vector2.Distance(footPosition, hit.point) < 0.02f) // Log -> 0.0149
            {
                _jumpType = JumpType.None;
            }
        }
        */

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

        // 최대 중력 고정
        if (_rigidbody.velocity.y < PLAYER_MAX_GRAVITY)
        {
            // TODO : 낙하 데미지 true
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, PLAYER_MAX_GRAVITY);
        }

        // Move
        if (_moveType != MoveType.None)
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
        if (_jumpType == JumpType.Single || _jumpType == JumpType.LadderJump)
        {
            _jumpType = JumpType.SingleJump;
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, 9f);
        }
        else if (_jumpType == JumpType.Double)
        {
            _jumpType = JumpType.DoubleJump;
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, 9f); // TODO : velocity.x 수정
        }
    }

    public override void Control(MoveType moveType, AttackType attackType, JumpType jumpType)
    {
        _moveType = moveType;
        _jumpType = jumpType;

        if (_moveType != MoveType.None)
        {
            // Move
            if (_playerState == PlayerState.None)
            {
                // Check Ladder
                if (_moveType != MoveType.Left && _moveType != MoveType.Right)
                {
                    bool moveUp = false;

                    if (_moveType == MoveType.Up || _moveType == MoveType.UpLeft || _moveType == MoveType.UpRight)
                    {
                        moveUp = true;
                    }

                    if (moveUp || (!moveUp && _jumpType == JumpType.None))
                    {
                        ILadder ladder = _controller.Stage.GetLadder(_playerFoot, moveUp);

                        if (ladder != null)
                        {
                            // TODO : interface와 state로 같은 상황에 동시에 사용하고 있음
                            _currentLadder = ladder;
                            _playerState = PlayerState.Climbing;

                            // 중력 비활성화
                            _rigidbody.gravityScale = 0f;
                        }
                    }
                }

                // Move Left || Right
                if (_playerState != PlayerState.Climbing)
                {
                    if (_moveType == MoveType.Left || _moveType == MoveType.UpLeft || _moveType == MoveType.DownLeft)
                    {
                        _moveType = MoveType.Left;
                        _spriteRenderer.flipX = true;
                        _controller.Camera.MoveCamera(MoveType.Left);
                    }
                    else if (_moveType == MoveType.Right || _moveType == MoveType.UpRight || _moveType == MoveType.DownRight)
                    {
                        _moveType = MoveType.Right;
                        _spriteRenderer.flipX = false;
                        _controller.Camera.MoveCamera(MoveType.Right);
                    }
                }
                // else if 아델 지하 통로 같은 특수한 경우 8방향 이동
            }

            // Climbing
            else if (_playerState == PlayerState.Climbing)
            {
                // Jump
                if (_moveType != MoveType.Up && _moveType != MoveType.Down && _jumpType == JumpType.Single)
                {
                    _jumpType = JumpType.LadderJump;

                    // 점프 후 바로 사다리 오르기 방지
                    _currentLadder.CanClimb = false;
                    _playerState = PlayerState.None;

                    // 중력 활성화
                    _rigidbody.gravityScale = 1f;
                }

                // Move Ladder
                if (_currentLadder != null)
                {
                    if (_moveType != MoveType.Left && _moveType != MoveType.Right)
                    {
                        bool isEscape = false;

                        if (_moveType == MoveType.Up || _moveType == MoveType.UpLeft || _moveType == MoveType.UpRight)
                        {
                            transform.position = new Vector2(transform.position.x, transform.position.y + PLAYER_MOVE_SPEED * Time.deltaTime);
                            _controller.Camera.MoveCamera(MoveType.Up);

                            if (_playerFoot.position.y > _currentLadder.MaxY + 0.1f)
                            {
                                isEscape = true;
                            }
                        }
                        else
                        {
                            transform.position = new Vector2(transform.position.x, transform.position.y - PLAYER_MOVE_SPEED * Time.deltaTime);
                            _controller.Camera.MoveCamera(MoveType.Down);

                            if (_playerFoot.position.y < _currentLadder.MinY)
                            {
                                isEscape = true;
                            }
                        }

                        if (isEscape)
                        {
                            _currentLadder = null;
                            _playerState = PlayerState.None;

                            // 중력 활성화
                            _rigidbody.gravityScale = 1f;

                            MoveType dirType = MoveType.Left;

                            if (_moveType == MoveType.UpRight || _moveType == MoveType.DownRight)
                            {
                                dirType = MoveType.Right;
                            }

                            if (dirType == MoveType.Left)
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
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            _jumpType = JumpType.None;
        }
    }

}
