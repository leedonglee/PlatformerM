using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class PlayCamera : BaseCamera
{
    [SerializeField]
    private Transform _minPointTransform;
    [SerializeField]
    private Transform _maxPointTransform;

    // 캐릭터 높이 위치 비율(0 ~ 1)
    private const float HEIGHT_RATIO = 0.4f;

    private Camera _camera;
    private Transform _playerTransform;

    private Vector2 _minPoint;
    private Vector2 _maxPoint;

    // _playerTransform의 Y값에 추가되는 높이값
    private float _addPointY;

    private MoveType _moveType = MoveType.Right;

    void Start()
    {
        _camera = GetComponent<Camera>();
        _playerTransform = _controller.Player.Transform;

        Vector3 camMinPoint = _camera.ViewportToWorldPoint(new Vector3(0, 0, _camera.nearClipPlane));
        Vector3 camMaxPoint = _camera.ViewportToWorldPoint(new Vector3(1, 1, _camera.nearClipPlane));

        Vector2 distance = new Vector2(Mathf.Abs(camMaxPoint.x - camMinPoint.x), Mathf.Abs(camMaxPoint.y - camMinPoint.y));
        Vector2 midPoint = new Vector2(distance.x / 2f, distance.y / 2f);
        Vector2 minPoint = new Vector2(_minPointTransform.position.x + midPoint.x, _minPointTransform.position.y + midPoint.y);
        Vector2 maxPoint = new Vector2(_maxPointTransform.position.x - midPoint.x, _maxPointTransform.position.y - midPoint.y);

        _minPoint = minPoint;
        _maxPoint = maxPoint;

        float ratioHeight = distance.y * HEIGHT_RATIO;

        if (midPoint.y > ratioHeight)
        {
            _addPointY = midPoint.y - ratioHeight;
        }
    }

    void LateUpdate()
    {
        // Position X
        float positionX = _playerTransform.position.x;

        if (_moveType == MoveType.Left)
        {
            positionX -= 1f;
        }
        else if (_moveType == MoveType.Right)
        {
            positionX += 1f;
        }

        positionX = Mathf.Clamp(positionX, _minPoint.x, _maxPoint.x);

        // Position Y
        float positionY = _playerTransform.position.y + _addPointY;

        if (_moveType == MoveType.Up)
        {
            positionY += 1f;
        }
        else if (_moveType == MoveType.Down)
        {
            positionY -= 1f;
        }

        positionY = Mathf.Clamp(positionY, _minPoint.y, _maxPoint.y);

        // Set Position
        _camera.transform.position =  Vector3.Lerp(_camera.transform.position, new Vector3(positionX, positionY, _camera.transform.position.z), 2f * Time.deltaTime);
    }

    public override void MoveCamera(MoveType moveType)
    {
        _moveType = moveType;
    }

}
