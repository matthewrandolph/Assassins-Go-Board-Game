using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySensor : MonoBehaviour
{

    [SerializeField] private Vector3 directionToSearch = new Vector3(0f, 0f, 2f);

    private Node m_nodeToSearch;
    private Board m_board;

    private bool m_foundPlayer = false;
    public bool FoundPlayer => m_foundPlayer;

    private void Awake()
    {
        m_board = Object.FindObjectOfType<Board>().GetComponent<Board>();
    }

    public void UpdateSensor()
    {
        Vector3 worldSpacePositionToSearch = transform.TransformVector(directionToSearch) + transform.position;

        if (m_board != null)
        {
            m_nodeToSearch = m_board.FindNodeAt(worldSpacePositionToSearch);

            if (m_nodeToSearch == m_board.PlayerNode)
            {
                m_foundPlayer = true;
            }
        }
    }

    // For testing only
/*    void Update()
    {
        UpdateSensor();
    }*/
}
