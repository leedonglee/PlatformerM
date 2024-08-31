using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIPadJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IEndDragHandler, IJoystick
{
    [SerializeField]
    private Transform _stickTransform;
    [SerializeField]
    private Transform _stickAreaRight;
    [SerializeField]
    private Transform _undetectableAreaRight;

    // 월드 좌표 변수
    private Vector2 _centerPosition;        // stick의 처음 중심값
    private float   _stickAreaMagnitude;    // stick의 처음 중심값 ~ stickArea 영역 거리값
    private float   _undetectableMagnitude; // stick의 처음 중심값 ~ undetectableArea 영역 거리값

    public MoveType StickMoveType
    {
        get
        {
            MoveType moveType = MoveType.None;

            Vector2 stickPosition = _stickTransform.position;
            float magnitude = (stickPosition - _centerPosition).magnitude;

            if (magnitude > _undetectableMagnitude)
            {
                Vector2 vector2 = (stickPosition - _centerPosition).normalized;

                float angle = Mathf.Atan2(vector2.y, vector2.x) * Mathf.Rad2Deg; // -180 ~ 180

                if (angle < 0f)
                {
                    angle += 360f;
                }

                float half = 45f / 2f;

                // 8방향 각 45도씩
                if (angle >= 45f - half && angle < 45f + half)
                {
                    moveType = MoveType.UpRight;
                }
                else if (angle >= 90f - half && angle < 90f + half)
                {
                    moveType = MoveType.Up;
                }
                else if (angle >= 135f - half && angle < 135f + half)
                {
                    moveType = MoveType.UpLeft;
                }
                else if (angle >= 180f - half && angle < 180f + half)
                {
                    moveType = MoveType.Left;
                }
                else if (angle >= 225f - half && angle < 225f + half)
                {
                    moveType = MoveType.DownLeft;
                }
                else if (angle >= 270f - half && angle < 270f + half)
                {
                    moveType = MoveType.Down;
                }
                else if (angle >= 315f - half && angle < 315f + half)
                {
                    moveType = MoveType.DownRight;
                }
                else if (angle >= 360f - half || angle < 0f + half)
                {
                    moveType = MoveType.Right;
                }
            }

            return moveType;
        }
    }

    void Start()
    {
        _centerPosition = _stickTransform.position;
        _stickAreaMagnitude = _stickAreaRight.position.x - _centerPosition.x;
        _undetectableMagnitude = _undetectableAreaRight.position.x - _centerPosition.x;
    }

    public void OnPointerDown(PointerEventData eventData) 
	{
		Control(eventData);
	}

    public void OnPointerUp(PointerEventData eventData) 
	{
		Cancel();
	}

    public void OnDrag(PointerEventData eventData)
    {
        
        Control(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
	{
		Cancel();
	}

    private void Control(PointerEventData eventData)
    {
        Vector2 touchPoint = eventData.position;
        float touchPointMagnitude = (touchPoint - _centerPosition).magnitude;

        if (touchPointMagnitude > _undetectableMagnitude)
        {
            Vector2 point = touchPoint;

            bool isFixed = false; // 끝에 고정

            if (touchPointMagnitude > _stickAreaMagnitude || isFixed)
            {
                Vector2 dir = (touchPoint - _centerPosition).normalized;
                point = _centerPosition + (dir * _stickAreaMagnitude);
            }

            _stickTransform.transform.position = point;
        }
        else
        {
            Cancel();
        }
    }

    private void Cancel()
    {
        _stickTransform.position = _centerPosition;
    }

}