using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    [SerializeField] private Animator playerAnimController;
    [SerializeField] private AudioSource playerAudioSource;
    
    [SerializeField] private string playerDeathTrigger = "isDead";

    private void Awake()
    {
        playerAudioSource = GetComponentInChildren<AudioSource>();
    }

    public void Die()
    {
        if (playerAnimController != null)
        {
            playerAnimController.SetTrigger(playerDeathTrigger);
        }

        if (playerAudioSource != null)
        {
            playerAudioSource.PlayDelayed(0.1f);
        }
    }
}
