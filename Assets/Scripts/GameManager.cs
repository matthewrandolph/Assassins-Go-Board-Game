using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Linq;

[System.Serializable]
public enum Turn
{
    Player,
    Enemy
}

public class GameManager : MonoBehaviour
{
    private Board m_board;
    private PlayerManager m_player;
    
    private List<EnemyManager> m_enemies;
    private Turn m_currentTurn = Turn.Player;
    public Turn CurrentTurn => m_currentTurn;
    
    private bool m_hasLevelStarted = false;
    public bool HasLevelStarted
    {
        get => m_hasLevelStarted;
        set => m_hasLevelStarted = value;
    }

    private bool m_isGamePlaying = false;
    public bool IsGamePlaying
    {
        get => m_isGamePlaying;
        set => m_isGamePlaying = value;
    }

    private bool m_isGameOver = false;
    public bool IsGameOver
    {
        get => m_isGameOver;
        set => m_isGameOver = value;
    }

    private bool m_hasLevelFinished = false;
    public bool HasLevelFinished
    {
        get => m_hasLevelFinished;
        set => m_hasLevelFinished = value;
    }


    public float delay = 1f;

    public UnityEvent setupEvent;
    public UnityEvent startLevelEvent;
    public UnityEvent playLevelEvent;
    public UnityEvent endLevelEvent;
    public UnityEvent loseLevelEvent;
    
    private void Awake()
    {
        m_board = Object.FindObjectOfType<Board>().GetComponent<Board>();
        m_player = Object.FindObjectOfType<PlayerManager>().GetComponent<PlayerManager>();
        EnemyManager[] enemies = Object.FindObjectsOfType<EnemyManager>() as EnemyManager[];
        m_enemies = enemies.ToList();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (m_player != null && m_board != null)
        {
            StartCoroutine("RunGameLoop");
        }
        else
        {
            Debug.Log("GAMEMANAGER Error: no player or board found!");
        }
    }

    IEnumerator RunGameLoop()
    {
        yield return StartCoroutine("StartLevelRoutine");
        yield return StartCoroutine("PlayLevelRoutine");
        yield return StartCoroutine("EndLevelRoutine");
    }

    IEnumerator StartLevelRoutine()
    {
        Debug.Log("SETUP LEVEL");
        if (setupEvent != null)
        {
            setupEvent.Invoke();
        }
        Debug.Log("START LEVEL");
        m_player.playerInput.InputEnabled = false;
        while (!m_hasLevelStarted)
        {
            // show start screen
            // user presses button to start
            // HasLevelStarted = true
            yield return null;
        }

        if (startLevelEvent != null)
        {
            startLevelEvent.Invoke();
        }
    }
    
    IEnumerator PlayLevelRoutine()
    {
        Debug.Log("PLAY LEVEL");
        m_isGamePlaying = true;
        yield return new WaitForSeconds(delay);
        m_player.playerInput.InputEnabled = true;

        if (playLevelEvent != null)
        {
            playLevelEvent.Invoke();
        }
        
        while (!m_isGameOver)
        {
            yield return null;
            // check for Game Over condition
            
            // win
            // reach the end of the level
            m_isGameOver = IsWinner();
            
            // lose
            // player dies
            
            // m_isGameOver = true
        }
        
        Debug.Log("WIN! ===================");
    }

    public void LoseLevel()
    {
        StartCoroutine(LoseLevelRoutine());
    }

    // trigger the "lose" condition
    private IEnumerator LoseLevelRoutine()
    {
        // game is over
        m_isGameOver = true;

        yield return new WaitForSeconds(1.5f);

        // invoke the loseLevelEvent
        if (loseLevelEvent != null)
        {
            loseLevelEvent.Invoke();
        }

        // pause for two seconds and then restart the level
        yield return new WaitForSeconds(2f);
        
        Debug.Log("LOSE! ===================");

        RestartLevel();
    }
    
    // end stage after gameplay is complete
    IEnumerator EndLevelRoutine()
    {
        Debug.Log("END LEVEL");
        m_player.playerInput.InputEnabled = false;

        if (endLevelEvent != null)
        {
            endLevelEvent.Invoke();
        }
        
        // show end screen
        while (!m_hasLevelFinished)
        {
            // user presses button to continue
            
            // HasLevelFinished = true
            yield return null;
        }

        RestartLevel();
    }

    private void RestartLevel()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void PlayLevel()
    {
        m_hasLevelStarted = true;
    }

    private bool IsWinner()
    {
        if (m_board.PlayerNode != null)
        {
            return (m_board.PlayerNode == m_board.GoalNode);
        }
        
        return false;
    }

    private void PlayPlayerTurn()
    {
        m_currentTurn = Turn.Player;
        m_player.TurnComplete = false;
        
        // allow Player to move
    }

    // switch to Enemy turn
    private void PlayEnemyTurn()
    {
        m_currentTurn = Turn.Enemy;

        foreach (EnemyManager enemy in m_enemies)
        {
            if (enemy != null && !enemy.IsDead)
            {
                enemy.TurnComplete = false;

                enemy.PlayTurn();
            }
        }
    }

    // have all of the enemies completed their turns?
    private bool EnemyTurnComplete()
    {
        foreach (EnemyManager enemy in m_enemies)
        {
            if (enemy.IsDead)
            {
                continue;
            }
            if (!enemy.TurnComplete)
            {
                return false;
            }
        }

        return true;
    }

    private bool EnemiesAreAllDead()
    {
        foreach (EnemyManager enemy in m_enemies)
        {
            if (!enemy.IsDead)
            {
                return false;
            }
        }
        return true;
    }
    
    // switch between Player and Enemy Turns
    public void UpdateTurn()
    {
        if (m_currentTurn == Turn.Player && m_player != null)
        {
            if (m_player.TurnComplete && !EnemiesAreAllDead())
            {
                PlayEnemyTurn();
            }
        }
        else if (m_currentTurn == Turn.Enemy)
        {
            if (EnemyTurnComplete())
            {
                PlayPlayerTurn();
            }
        }
    }
}
