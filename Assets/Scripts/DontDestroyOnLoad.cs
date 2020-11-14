using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DontDestroyOnLoad : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += DestroySelfAtFinish;
    }

    private void DestroySelfAtFinish(Scene arg0, LoadSceneMode arg1)
    {
        if (GameObject.FindGameObjectWithTag("Finish") != null)
        {
            //Destroy(gameObject);
        }
    }
}
