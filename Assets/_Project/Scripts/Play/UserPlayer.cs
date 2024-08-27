using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserPlayer : BasePlayer
{
    /*
    private MoveType _moveType;
    private bool _attackState;
    private bool _jumpState;
    */

    private const float PLAYER_MAX_GRAVITY = -10f;
    private const float PLAYER_MOVE_SPEED = 3f;

    private Rigidbody2D _rigidbody;

    // Sprite
    private SpriteRenderer _spriteRenderer;
    private Vector3 _spriteMinPoint;

    // State
    private MoveType _moveType = MoveType.None;
    private JumpType _jumpType = JumpType.None;
    private bool _isClimbing = false;

    // Platform LayerMask
    private int _platformLayerMask;

    public override Transform Transform => transform;

    public override JumpType  JumpType  => _jumpType;

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rigidbody = GetComponent<Rigidbody2D>();

        _spriteMinPoint = _spriteRenderer.sprite.bounds.min;

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
        if (_isClimbing)
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
        if (_jumpType == JumpType.Single)
        {
            _jumpType = JumpType.SingleJump;
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, 9f);
        }
        else if (_jumpType == JumpType.Double)
        {
            _jumpType = JumpType.DoubleJump;
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, 9f); // TODO : 수정
        }
    }

    public override void Control(MoveType moveType, AttackType attackType, JumpType jumpType)
    {
        // Move
        _moveType = moveType;
        _jumpType = jumpType;

        if (!_isClimbing)
        {
            // TODO : 사다리 위에서 Down도 추가, 사다리 위에서 Up인지 밑에서 Down인지도 체크
            // Climb
            if (_controller.Stage.CanClimb(transform) && (_moveType == MoveType.Up || _moveType == MoveType.UpLeft || _moveType == MoveType.UpRight))
            {
                _isClimbing = true;
                _rigidbody.gravityScale = 0f;
            }
            // Move
            else
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
        }
        // Climbing
        else if (_moveType != MoveType.Left && _moveType != MoveType.Right)
        {
            if (_moveType == MoveType.Up || _moveType == MoveType.UpLeft || _moveType == MoveType.UpRight || _moveType == MoveType.Down || _moveType == MoveType.DownLeft || _moveType == MoveType.DownRight)
            {
                if (_controller.Stage.CanClimb(transform))
                {
                    if (_moveType == MoveType.Up || _moveType == MoveType.UpLeft || _moveType == MoveType.UpRight)
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
                    _isClimbing = false;
                    _rigidbody.gravityScale = 1f;
                }
            }
        }

        /*
        if (_isClimbing)
        {
            if (_moveType == MoveType.Up)
            {
                
            }
            else if (_moveType == MoveType.Down)
            {
                
            }
        }

        // Check Rope, Ladder
        if (!_isClimbing && _moveType == MoveType.Up)
        {
            if (_controller.Stage.CanClimb())
            {
                // TODO : Gravity Scale = 0
                _isClimbing = true;
            }
        }
        */

        // Jump
        /*
        if (_isClimbing)
        {
            _jumpType = JumpType.None;
            return;
        }

        if (_jumpType == JumpType.None && jumpType == JumpType.Single)
        {
            _jumpType = JumpType.Single;
        }
        else if (_jumpType == JumpType.SingleJump && jumpType == JumpType.Double)
        {
            _jumpType = JumpType.Double;
        }
        */

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            _jumpType = JumpType.None;
        }
    }

}
