using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cysharp.Threading.Tasks;

public class StagePlatform : MonoBehaviour, IPlatform
{
    private PlatformEffector2D _platfromEffector;
    private float _initialValue;

    private bool _isInactive = false;

    void Start()
    {
        _platfromEffector = GetComponent<PlatformEffector2D>();
        _initialValue = _platfromEffector.surfaceArc;
    }

    public void SetInactive()
    {
        if (_isInactive) return;

        _isInactive = true;
        _platfromEffector.surfaceArc = 0f;
        SetState();
    }

    private async void SetState()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(0.75f), ignoreTimeScale: false);

        _isInactive = false;
        _platfromEffector.surfaceArc = _initialValue;
    }

}
