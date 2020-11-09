using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    [SerializeField] private Animator playerAnimController;
    
    [SerializeField] private string playerDeathTrigger = "isDead";

    public void Die()
    {
        if (playerAnimController != null)
        {
            playerAnimController.SetTrigger(playerDeathTrigger);
        }
    }
}
