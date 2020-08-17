using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyMover))]
[RequireComponent(typeof(EnemySensor))]
public class EnemyManager : TurnManager
{
    private EnemyMover m_enemyMover;
    private EnemySensor m_enemySensor;
    private Board m_board;

    protected override void Awake()
    {
        base.Awake();
        m_board = Object.FindObjectOfType<Board>().GetComponent<Board>();
        m_enemyMover = Object.FindObjectOfType<EnemyMover>();
        m_enemySensor = Object.FindObjectOfType<EnemySensor>();
    }

    public void PlayTurn()
    {
        StartCoroutine(PlayTurnRoutine());
    }

    private IEnumerator PlayTurnRoutine()
    {
        Debug.Log("PlayTurnRoutine invoked");
        // detect player
        m_enemySensor.UpdateSensor();
        
        // attack player
        
        // wait
        yield return new WaitForSeconds(0f);
        
        // movement
        m_enemyMover.MoveOneTurn();
    }
}
