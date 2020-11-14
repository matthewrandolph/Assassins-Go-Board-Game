using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CleanupObjects : MonoBehaviour
{
    public void RestartGame()
    {
        FindObjectOfType<GameManager>().RestartGame();
    }
}
