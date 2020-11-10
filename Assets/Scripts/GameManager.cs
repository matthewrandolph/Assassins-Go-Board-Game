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

    private bool m_advanceLevel = false;
    public bool AdvanceLevel
    {
        get => m_advanceLevel;
        set => m_advanceLevel = value;
    }

    private bool m_pauseMenuOpen = false;
    public bool PauseMenuOpen
    {
        get => m_pauseMenuOpen;
        set
        {
            if (value) { pauseMenuOpenEvent?.Invoke(); }
            else { pauseMenuCloseEvent?.Invoke(); }
            Time.timeScale = 1f - Time.timeScale;
            m_pauseMenuOpen = value;
        } 
    }


    public float delay = 1f;

    public UnityEvent setupEvent;
    public UnityEvent startLevelEvent;
    public UnityEvent playLevelEvent;
    public UnityEvent endLevelEvent;
    public UnityEvent loseLevelEvent;
    public UnityEvent pauseMenuOpenEvent;
    public UnityEvent pauseMenuCloseEvent;

    private Coroutine gameLoopCoroutine;
    private Coroutine startLevelCoroutine;
    private Coroutine playLevelCoroutine;
    private Coroutine endLevelCoroutine;


    private void Awake()
    {
        SceneManager.sceneLoaded += InitLevel;
    }

    private void InitLevel(Scene scene, LoadSceneMode mode)
    {
        ResetGameLoopVariables();

        Board board = FindObjectOfType<Board>();
        if (board != null)
        {
            m_board = board.GetComponent<Board>();
        }

        PlayerManager playerManager = FindObjectOfType<PlayerManager>();
        if (playerManager != null)
        {
            m_player = playerManager.GetComponent<PlayerManager>();
        }

        EnemyManager[] enemies = FindObjectsOfType<EnemyManager>();
        if (enemies != null)
        {
            m_enemies = enemies.ToList();
        }
        
        if (m_player != null && m_board != null)
        {
            StartCoroutine("RunGameLoop");
        }
        else
        {
            Debug.Log("GAMEMANAGER Note: no player or board found! Either this is the loading scene or an error has occured.");
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

        AddStartLevelEventListeners();
        if (startLevelEvent != null)
        {
            startLevelEvent.Invoke();
        }
    }

    private void AddStartLevelEventListeners()
    {
        if (m_player != null)
        {
            PlayerCompass playerCompass = m_player.GetComponentInChildren<PlayerCompass>();
            startLevelEvent.AddListener(delegate { playerCompass.ShowArrows(false); });
        }
        if (m_board != null)
        {
            startLevelEvent.AddListener(m_board.InitBoard);
        }
    }
    
    IEnumerator PlayLevelRoutine()
    {
        Debug.Log("PLAY LEVEL");
        m_isGamePlaying = true;
        yield return new WaitForSeconds(delay);
        m_player.playerInput.InputEnabled = true;

        AddPlayLevelEventListeners();
        if (playLevelEvent != null)
        {
            playLevelEvent.Invoke();
        }
        
        // Start in-game UI activity
        StartPauseMenuPolling();
        
        while (!m_isGameOver)
        {
            yield return null;

            // win by reaching the end of the level
            m_isGameOver = IsWinner();
        }
        
        Debug.Log("WIN! ===================");
    }

    private void AddPlayLevelEventListeners()
    {
        if (m_board != null)
        {
            playLevelEvent.AddListener(m_board.DrawGoal);
        }
        if (m_player != null)
        {
            PlayerCompass playerCompass = m_player.GetComponentInChildren<PlayerCompass>();
            playLevelEvent.AddListener(delegate { playerCompass.ShowArrows(true); });
        }
    }

    public void LoseLevel(float fadeWaitForSeconds = 1.5f)
    {
        StartCoroutine(LoseLevelRoutine(fadeWaitForSeconds));
    }

    // trigger the "lose" condition
    private IEnumerator LoseLevelRoutine(float fadeWaitForSeconds)
    {
        // game is over
        m_isGameOver = true;

        yield return new WaitForSeconds(fadeWaitForSeconds);

        // invoke the loseLevelEvent
        if (loseLevelEvent != null)
        {
            Debug.Log("LoseLevelEvent invoking now.");
            loseLevelEvent.Invoke();
            Debug.Log("LoseLevelEvent finished invoking.");
        }

        // pause for two seconds and then restart the level
        yield return new WaitForSeconds(2f);
        
        Debug.Log("LOSE! ===================");

        RestartLevel();
    }

    private void StartPauseMenuPolling()
    {
        StartCoroutine(PauseMenuPollCoroutine());
    }

    private IEnumerator PauseMenuPollCoroutine()
    {
        bool activeLastFrame = false;
        
        while (!m_isGameOver)
        {
            if (Input.GetAxisRaw("Cancel") >= float.Epsilon)
            {
                if (!activeLastFrame)
                {
                    PauseMenuOpen = !PauseMenuOpen;
                }

                activeLastFrame = true;
            }
            else
            {
                activeLastFrame = false;
            }

            yield return null;
        }
    }
    
    // end stage after gameplay is complete
    IEnumerator EndLevelRoutine()
    {
        Debug.Log("END LEVEL");
        m_player.playerInput.InputEnabled = false;
        
        AddEndLevelEventListeners();
        if (endLevelEvent != null)
        {
            endLevelEvent.Invoke();
        }
        
        // show end screen
        while (!m_hasLevelFinished)
        {
            // user presses one of the buttons to continue, which set HasLevelFinished = true
            yield return null;
        }
        
        // Allow a frame to determine whether the player pressed "Next Level" or "Replay Level"
        yield return null;

        if (AdvanceLevel)
        {
            StartNextLevel();
        }
        else
        {
            RestartLevel();
        }
    }

    private void AddEndLevelEventListeners()
    {
        if (m_player != null)
        {
            PlayerCompass playerCompass = m_player.GetComponentInChildren<PlayerCompass>();
            startLevelEvent.AddListener(delegate { playerCompass.ShowArrows(false); });
        }
    }

    private void RestartLevel()
    {
        CleanupEventListeners();
        CleanupLoopRoutines();

        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    private void CleanupLoopRoutines()
    {
        StopCoroutine("RunGameLoop");
    }

    private void StartNextLevel()
    {
        CleanupEventListeners();
        
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.buildIndex + 1);
    }

    public void RestartGame()
    {
        CleanupEventListeners();
        CleanupLoopRoutines();

        SceneManager.LoadScene(1);
    }
    
    private void CleanupEventListeners()
    {
        setupEvent.RemoveAllListeners();
        startLevelEvent.RemoveAllListeners();
        playLevelEvent.RemoveAllListeners();
        endLevelEvent.RemoveAllListeners();
        loseLevelEvent.RemoveAllListeners();
        pauseMenuOpenEvent.RemoveAllListeners();
        pauseMenuCloseEvent.RemoveAllListeners();
    }

    private void ResetGameLoopVariables()
    {
        HasLevelStarted = false;
        IsGamePlaying = false;
        IsGameOver = false;
        HasLevelFinished = false;
        AdvanceLevel = false;
        m_currentTurn = Turn.Player;
    }

    public void QuitGame()
    {
        Application.Quit();
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
