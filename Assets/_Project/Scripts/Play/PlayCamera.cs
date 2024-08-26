using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayCamera : BaseCamera
{
    [SerializeField]
    private Transform _minPoint;
    [SerializeField]
    private Transform _maxPoint;

    // 캐릭터 높이 위치 비율(0 ~ 1)
    private const float HEIGHT_RATIO = 0.4f;

    private Camera _camera;
    private Transform _playerTransform;

    private float _minPointX;
    private float _maxPointX;

    // _playerTransform의 Y값에 추가되는 높이값
    private float _addPointY;

    private MoveType _moveType;

    void Start()
    {
        _camera = GetComponent<Camera>();
        _playerTransform = _controller.GetPlayerTransform();

        Vector3 camMinPoint = _camera.ViewportToWorldPoint(new Vector3(0, 0, _camera.nearClipPlane));
        Vector3 camMaxPoint = _camera.ViewportToWorldPoint(new Vector3(1, 1, _camera.nearClipPlane));

        float distanceX = Mathf.Abs(camMaxPoint.x - camMinPoint.x);

        if (distanceX > 20f)
        {
            float midpointX = distanceX / 2f;
            _minPointX = _minPoint.position.x + midpointX;
            _maxPointX = _maxPoint.position.x - midpointX;
        }

        float distanceY = Mathf.Abs(camMaxPoint.y - camMinPoint.y);
        float midpointY = distanceY / 2f;
        float ratioHeight = distanceY * HEIGHT_RATIO;

        if (midpointY > ratioHeight)
        {
            _addPointY = midpointY - ratioHeight;
        }
    }

    void Update()
    {
        /*
        float positionX = 0f;
        positionX = Mathf.Clamp(positionX, _minPointX, _maxPointX);
        */

        float positionY = _playerTransform.position.y + _addPointY;

        // _dropTransform.localPosition = Vector2.Lerp(_dropTransform.localPosition, _targetTransform.localPosition, 10f * Time.deltaTime);
        _camera.transform.position =  Vector3.Lerp(_camera.transform.position, new Vector3(_playerTransform.position.x, positionY, _camera.transform.position.z), 10f * Time.deltaTime);
    }

    public override void MoveCamera(MoveType moveType)
    {
        _moveType = moveType;
    }

}
