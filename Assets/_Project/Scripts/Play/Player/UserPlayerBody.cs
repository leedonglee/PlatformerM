using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cysharp.Threading.Tasks;

public class UserPlayerBody : MonoBehaviour
{
    private LayerMask _damageLayerMask; // Monster, Trap, ...
    private bool _isDamaged = false;

    void Start()
    {
        string[] damageLayers = new string[2]; // Ground, Box, ...

        damageLayers[0] = GameLayer.MONSTER;
        damageLayers[1] = GameLayer.TRAP;

        _damageLayerMask = LayerMask.GetMask(damageLayers);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if ((_damageLayerMask.value & (1 << collision.gameObject.layer)) != 0)
        {
            _isDamaged = true;
            // callback?.Invoke();
            SetState();
        }
    }

    private async void SetState()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(1.0f), ignoreTimeScale: false);

        _isDamaged = true;
    }

}
