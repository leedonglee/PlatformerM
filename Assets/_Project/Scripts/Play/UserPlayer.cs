using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserPlayer : BasePlayer
{
    private MoveType _moveType;
    private bool _attackState;
    private bool _jumpState;

    Rigidbody2D _rigidbody;

    private bool _isJumping;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {

        /*
        _moveType = MoveType.None;
        // _jumpState = false;

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            _moveType = MoveType.Left;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            _moveType = MoveType.Right;
        }

        

        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            _jumpState = true;
        }
        */
    }

    void FixedUpdate()
    {
        if (_jumpState)
        {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, 9f);
        }

        if (_rigidbody.velocity.y < -10f)
        {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, -10f);
        }

        _jumpState = false;
    }

    public override Transform GetPlayerTransform()
    {
        return transform;
    }

    public override void InputEvent(MoveType moveType, AttackType attackType, JumpType jumpType)
    {

    }

    public override JumpType GetJumpType()
    {
        return JumpType.None;
    }

}
