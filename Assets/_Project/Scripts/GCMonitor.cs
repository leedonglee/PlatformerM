using System;
using UnityEngine;

public class GCMonitor : MonoBehaviour
{
    void OnEnable()
    {
        // 가비지 컬렉션이 발생할 때 이벤트가 호출됩니다.
        // AppDomain.MonitoringIsEnabled = true;
    }

    void Update()
    {
        // 현재 가비지 컬렉션 횟수 출력
        Debug.Log("GC Count: " + GC.CollectionCount(0));
    }
}