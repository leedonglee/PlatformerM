using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserPlayer : BasePlayer
{
    private enum PlayerState
    {
        None, Down, Walking, Jumping, Climbing, Attacking
    }

    [SerializeField]
    private UserPlayerBody _playerBody;
    
    private const string PLAYER_ANIMATION = "PlayerState";
    private const float PLAYER_MAX_GRAVITY = -10f;
    private const float PLAYER_MOVE_SPEED = 3f;

    // Components
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rigidbody;

    // State
    private PlayerState _playerState = PlayerState.None;
    private MoveType _moveType = MoveType.None;
    private JumpType _jumpType = JumpType.None;

    // Foot
    private float _playerBottom;

    // Attacking
    private float _attackCoolTime;

    // Climbing
    private ILadder _playerLadder;

    // Platform LayerMask
    private LayerMask _platformLayerMask;

    // BasePlayer
    public override Transform Transform => transform;
    public override JumpType  JumpType  => _jumpType;

    // Foot
    private Vector2 PlayerFootPosition { get { return new Vector2(transform.position.x, transform.position.y + _playerBottom); } }

    void Start()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rigidbody = GetComponent<Rigidbody2D>();

        _playerBottom = _spriteRenderer.sprite.bounds.min.y;

        // Platform Layers
        string[] platformLayers = new string[2]; // Ground, Box, ...

        platformLayers[0] = GameLayer.GROUND;
        platformLayers[1] = GameLayer.BOX;

        _platformLayerMask = LayerMask.GetMask(platformLayers);
    }

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
        if (_moveType != MoveType.None && _jumpType != JumpType.SingleJump && _jumpType != JumpType.DoubleJump)
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
            SoundManager.Instance?.PlaySound(SoundType.Jump);

            _playerState = PlayerState.Jumping;
            _jumpType = JumpType.SingleJump;
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, 9f);
            _rigidbody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }
        else if (_jumpType == JumpType.Double)
        {
            SoundManager.Instance?.PlaySound(SoundType.Jump);

            _playerState = PlayerState.Jumping;
            _jumpType = JumpType.DoubleJump;
            _rigidbody.velocity = new Vector2(_spriteRenderer.flipX ? -10f : 10f, 9f);
            _rigidbody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }
        else if (_jumpType == JumpType.Down)
        {
            SoundManager.Instance?.PlaySound(SoundType.Jump);

            _playerState = PlayerState.Jumping;
            _jumpType = JumpType.DownJump;
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, 4.5f);
            _rigidbody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        ChangeLayer(_rigidbody.velocity.y > 0f);

        // Ground
        if (Mathf.Abs(_rigidbody.velocity.y) < 0.1f)
        {
            bool isGround = false;

            RaycastHit2D hit = Physics2D.BoxCast(PlayerFootPosition, new Vector2(0.6f, 0.1f), 0f, Vector2.down, 0.1f, _platformLayerMask);

            if (hit.collider != null)
            {
                if (Mathf.Abs(PlayerFootPosition.y - hit.point.y) <= 0.1f)
                {
                    isGround = true;
                }
            }

            if (isGround)
            {
                _playerState = PlayerState.None;
                _jumpType = JumpType.None;
                _rigidbody.collisionDetectionMode = CollisionDetectionMode2D.Discrete;
            }
        }
    }

    // UI Update에서 호출
    public override void Control(MoveType moveType, AttackType attackType, JumpType jumpType)
    {
        _moveType = moveType;

        // Jump
        if ((_jumpType == JumpType.None && jumpType == JumpType.Single) || (_jumpType == JumpType.SingleJump && jumpType == JumpType.Double))
        {
            _jumpType = jumpType;
        }

        if (_playerState == PlayerState.None || _playerState == PlayerState.Down || _playerState == PlayerState.Walking || _playerState == PlayerState.Jumping)
        {
            if (_moveType != MoveType.None)
            {
                // Get Ladder
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
                    ILadder ladder = _controller.Stage.GetLadder(PlayerFootPosition, climbingType == MoveType.Up);

                    if (ladder != null)
                    {
                        _playerLadder = ladder;
                        transform.position = new Vector2(ladder.PosX, transform.position.y);
                        
                        // Set State
                        _playerState = PlayerState.Climbing;
                        gameObject.layer = LayerMask.NameToLayer(GameLayer.PLAYER_CLIMBING);

                        // 중력 비활성화
                        _rigidbody.gravityScale = 0f;
                        return;
                    }
                }

                // Move Down
                if (_moveType == MoveType.DownLeft || _moveType == MoveType.Down || _moveType == MoveType.DownRight)
                {
                    if (_jumpType == JumpType.Single)
                    {
                        RaycastHit2D hit = Physics2D.BoxCast(new Vector2(PlayerFootPosition.x, PlayerFootPosition.y - 0.09f), new Vector2(0.6f, 0.1f), 0f, Vector2.down, 0.1f, _platformLayerMask);

                        if (hit.collider != null)
                        {
                            if (Mathf.Abs(PlayerFootPosition.y - hit.point.y) <= 0.1f)
                            {
                                if (hit.collider.gameObject.TryGetComponent(out IPlatform ground))
                                {
                                    _jumpType = JumpType.Down;
                                    ground.SetInactive();
                                }
                            }
                        }
                    }
                }

                // Move
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
                    if (_playerState != PlayerState.Jumping)
                    {
                        _playerState = PlayerState.Walking;
                    }

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
                else if (_moveType == MoveType.Down)
                {
                    if (_playerState != PlayerState.Jumping)
                    {
                        _playerState = PlayerState.Down;
                    }
                }
            }

            if (attackType != AttackType.None)
            {
                if (attackType == AttackType.A && _attackCoolTime <= 0f)
                {
                    _playerState = PlayerState.Attacking;
                    _attackCoolTime = 0.5f;

                    // TODO: Attack
                }
            }
        }
        else if (_playerState == PlayerState.Climbing)
        {
            if (_moveType != MoveType.None)
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

                    // 점프 후 같은 사다리 오르기 방지
                    _playerLadder.CanClimb = false;
                    _playerLadder = null;

                    // Animation
                    _animator.speed = 1f;
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
                    // Animation
                    _animator.speed = 1f;

                    bool canClimb = CanClimb(climbingType == MoveType.Up, _playerLadder.MaxY, _playerLadder.MinY);

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

                        if (climbingType == MoveType.Up)
                        {
                            transform.position = new Vector2(transform.position.x, _playerLadder.MaxY + Mathf.Abs(_playerBottom));
                        }
                    }
                }
                else
                {
                    _animator.speed = 0f;
                }
            }
            else
            {
                _jumpType = JumpType.None;

                // Animation
                _animator.speed = 0f;
            }
        }

        // Attack
        if (_attackCoolTime > 0f)
        {
            _attackCoolTime -= Time.deltaTime;
        }

        // Animation
        _animator.SetInteger(PLAYER_ANIMATION, (int)_playerState);
    }

    private bool CanClimb(bool climbingUp, float maxY, float minY)
    {
        if (climbingUp)
        {
            if (PlayerFootPosition.y >= minY)
            {
                if (PlayerFootPosition.y > maxY - 0.025f)
                {
                    return false;
                }

                return true;
            }
        }
        else
        {
            if (PlayerFootPosition.y > minY && PlayerFootPosition.y <= maxY + 0.025f)
            {
                return true;
            }
        }

        return false;
    }

    private void ChangeLayer(bool isJumping)
    {
        if (!isJumping)
        {
            gameObject.layer = LayerMask.NameToLayer(GameLayer.PLAYER);
        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer(GameLayer.PLAYER_JUMPING);
        }
    }

}
