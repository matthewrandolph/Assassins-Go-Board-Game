using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class EnemyDeath : MonoBehaviour
{

    [SerializeField] private Vector3 offscreenOffset = new Vector3(0f, 10f, 0f);

    private Board m_board;

    [SerializeField] private float deathDelay = 0f;
    [SerializeField] private float offscreenDelay = 1f;

    [SerializeField] private float iTweenDelay = 0f;
    [SerializeField] private iTween.EaseType easeType = iTween.EaseType.easeInOutQuint;
    [SerializeField] private float moveTime = 0.5f;

    [SerializeField] private AudioSource enemyAudioSource;

    private void Awake()
    {
        m_board = Object.FindObjectOfType<Board>().GetComponent<Board>();
    }

    public void MoveOffBoard(Vector3 target)
    {
        iTween.MoveTo(gameObject, iTween.Hash(
            "x", target.x,
            "y", target.y,
            "z", target.z,
            "delay", iTweenDelay,
            "easetype", easeType,
            "time", moveTime
            ));
    }

    public void Die()
    {
        StartCoroutine(DieRoutine());
    }

    private IEnumerator DieRoutine()
    {
        enemyAudioSource.Play();
        
        yield return new WaitForSeconds(deathDelay);

        Vector3 offscreenPos = transform.position + offscreenOffset;
        MoveOffBoard(offscreenPos);
        
        yield return new WaitForSeconds(moveTime + offscreenDelay);

        if (m_board.capturedPositions.Count != 0 && m_board.CurrentCapturePosition < m_board.capturedPositions.Count)
        {
            Vector3 capturePos = m_board.capturedPositions[m_board.CurrentCapturePosition].position;
            transform.position = capturePos + offscreenOffset;
            
            MoveOffBoard(capturePos);
            
            yield return new WaitForSeconds(moveTime);
            
            enemyAudioSource.Play();

            m_board.CurrentCapturePosition++;
            m_board.CurrentCapturePosition =
                Mathf.Clamp(m_board.CurrentCapturePosition, 0, m_board.capturedPositions.Count - 1);
        }
    }
}
