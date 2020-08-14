﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCompass : MonoBehaviour
{
    private Board m_board;

    [SerializeField] private GameObject arrowPrefab;
    
    private List<GameObject> m_arrows = new List<GameObject>();

    [SerializeField] private float scale = 1f;
    [SerializeField] private float startDistance = 0.25f;
    [SerializeField] private float endDistance = 0.5f;

    [SerializeField] private float moveTime = 1f;
    [SerializeField] private iTween.EaseType easeType = iTween.EaseType.easeInOutExpo;

    private void Awake()
    {
        m_board = Object.FindObjectOfType<Board>().GetComponent<Board>();
        SetupArrows();
    }

    private void SetupArrows()
    {
        if (arrowPrefab == null)
        {
            Debug.LogWarning("PLAYERCOMPASS SetupArrows ERROR: Missing arrow prefab!");
            return;
        }

        foreach (Vector2 dir in Board.directions)
        {
            Vector3 dirVector = new Vector3(dir.normalized.x, 0f, dir.normalized.y);
            Quaternion rotation = Quaternion.LookRotation(dirVector);
            
            GameObject arrowInstance =
                Instantiate(arrowPrefab, transform.position + dirVector * startDistance, rotation);
            arrowInstance.transform.localScale = new Vector3(scale, scale, scale);
            arrowInstance.transform.parent = transform;
            
            m_arrows.Add(arrowInstance);
        }
    }

    private void MoveArrow(GameObject arrowInstance)
    {
        iTween.MoveBy(arrowInstance, iTween.Hash(
            "z", endDistance - startDistance,
            "looptype", iTween.LoopType.loop,
            "time", moveTime,
            "easetype", easeType
            ));
    }

    private void MoveArrows()
    {
        foreach (GameObject arrow in m_arrows)
        {
            MoveArrow(arrow);
        }
    }

    public void ShowArrows(bool state)
    {
        if (m_board == null)
        {
            Debug.LogWarning("PLAYERCOMPASS ShowArrows ERROR: no Board found!");
            return;
        }

        if (m_arrows == null)
        {
            Debug.LogWarning("PLAYERCOMPASS ShowArrows ERROR: no arrows found!");
            return;
        }

        if (m_board.PlayerNode != null)
        {
            for (int i = 0; i < Board.directions.Length; i++)
            {
                Node neighbor = m_board.PlayerNode.FindNeighborAt(Board.directions[i]);

                if (neighbor == null || !state)
                {
                    m_arrows[i].SetActive(false);
                }
                else
                {
                    bool activeState = m_board.PlayerNode.LinkedNodes.Contains(neighbor);
                    m_arrows[i].SetActive(activeState);
                }
            }
        }

        ResetArrows();
        MoveArrows();
    }

    private void ResetArrows()
    {
        for (int i = 0; i < Board.directions.Length; i++)
        {
            iTween.Stop(m_arrows[i]);
            Vector3 dirVector = new Vector3(Board.directions[i].normalized.x, 0f, Board.directions[i].normalized.y);
            m_arrows[i].transform.position = transform.position + dirVector * startDistance;
        }
    }
}
