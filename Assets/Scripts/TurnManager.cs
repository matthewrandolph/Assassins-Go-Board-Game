﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// base class for player and enemy game pieces
public class TurnManager : MonoBehaviour
{

    protected GameManager m_gameManager;

    // have we completed our turn?
    protected bool m_isTurnComplete = false;
    public bool IsTurnComplete
    {
        get => m_isTurnComplete;
        set => m_isTurnComplete = value;
    }

    // initialize fields
    protected virtual void Awake()
    {
        m_gameManager = Object.FindObjectOfType<GameManager>().GetComponent<GameManager>();
    }

    // complete the turn and notify the GameManager
    public void FinishTurn()
    {

        // update the GameManager
        if (m_gameManager != null)
        {
            m_gameManager.UpdateTurn();
        }
    }
}
