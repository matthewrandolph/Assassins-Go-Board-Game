using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinner : MonoBehaviour
{
    [SerializeField] private float rotateSpeed = 20f;

    void Start()
    {
        iTween.RotateBy(gameObject, iTween.Hash(
            "y", 360f,
            "looptype", iTween.LoopType.loop,
            "speed", rotateSpeed,
            "easetype", iTween.EaseType.linear
        ));
    }
}
