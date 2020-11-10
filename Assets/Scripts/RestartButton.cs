using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RestartButton : MonoBehaviour
{
    private PlayerDeath playerDeath;
    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
    }
    
    void OnEnable()
    {
        playerDeath = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerDeath>();
        button.onClick.AddListener(playerDeath.Die);
    }
    
    void OnDisable()
    {
        button.onClick.RemoveAllListeners();
    }
}
