using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    [SerializeField]
    private Transform _maxPointY;
    [SerializeField]
    private Transform _minPointY;

    public float MaxY => _maxPointY.position.y;

    public float MinY => _minPointY.position.y;
}
