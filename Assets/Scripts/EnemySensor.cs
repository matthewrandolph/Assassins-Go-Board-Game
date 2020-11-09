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

    // check if the Player has moved into our sensor
    public void UpdateSensor(Node enemyNode)
    {
        // convert the local directionToSearch into a world space 3d position
        Vector3 worldSpacePositionToSearch = transform.TransformVector(directionToSearch) + transform.position;

        if (m_board != null)
        {
            // find the node at the world space position to search
            m_nodeToSearch = m_board.FindNodeAt(worldSpacePositionToSearch);

            if (!enemyNode.LinkedNodes.Contains(m_nodeToSearch))
            {
                m_foundPlayer = false;
                return;
            }

            // if the node to search is the PlayerNode, then we have found the Player
            if (m_nodeToSearch == m_board.PlayerNode)
            {
                m_foundPlayer = true;
            }
        }
    }
}
