using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cysharp.Threading.Tasks;

public class StageLadder : MonoBehaviour, ILadder
{
    [SerializeField]
    private Transform _maxPoint;
    [SerializeField]
    private Transform _minPoint;

    private bool _canClimb = true;

    public bool CanClimb
    {
        get
        {
            return _canClimb;
        }
        set
        {
            _canClimb = value;
            SetState();
        }
    }

    public float MaxY => _maxPoint.position.y;

    public float MinY => _minPoint.position.y;

    private async void SetState()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(1.0f), ignoreTimeScale: false);

        _canClimb = true;
    }
}
