using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameInit : MonoBehaviour
{
    void Awake()
    {
        // 프레임 고정
        Application.targetFrameRate = 60;
        // 화면 꺼짐 방지
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    void Start()
    {
        SceneManager.LoadScene(1);
    }

}
