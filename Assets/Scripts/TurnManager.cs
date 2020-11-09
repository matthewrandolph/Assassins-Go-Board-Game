using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// base class for player and enemy game pieces
public class TurnManager : MonoBehaviour
{

    protected GameManager m_gameManager;

    // have we completed our turn?
    protected bool m_turnComplete = false;
    public bool TurnComplete
    {
        get => m_turnComplete;
        set => m_turnComplete = value;
    }

    // initialize fields
    protected virtual void Awake()
    {
        m_gameManager = Object.FindObjectOfType<GameManager>().GetComponent<GameManager>();
    }

    // complete the turn and notify the GameManager
    public virtual void FinishTurn()
    { 
        m_turnComplete = true;
        
        // update the GameManager
        if (m_gameManager != null)
        {
            m_gameManager.UpdateTurn();
        }
    }
}
