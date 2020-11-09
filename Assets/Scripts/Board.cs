using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

public class Board : MonoBehaviour
{
    // uniform distance between nodes
    public static float spacing = 2f;

    // four compass directions
    public static readonly Vector2[] directions =
    {
        new Vector2(spacing, 0f),
        new Vector2(-spacing, 0f),
        new Vector2(0f, spacing),
        new Vector2(0f, -spacing)
    };

    // the Node directly under the Player
    private Node m_playerNode;
    public Node PlayerNode { get { return m_playerNode; }}

    // list of all of the Nodes on the Board
    private List<Node> m_allNodes = new List<Node>();
    public List<Node> AllNodes { get { return m_allNodes; }}

    // the PlayerMover component
    private PlayerMover m_player;

    // the Node representing the end of the maze
    private Node m_goalNode;
    public Node GoalNode => m_goalNode;
    
    // iTween parameters for drawing the goal
    [SerializeField] private GameObject goalPrefab;
    [SerializeField] private float drawGoalTime = 2f;
    [SerializeField] private float drawGoalDelay = 2f;
    [SerializeField] private iTween.EaseType drawGoalEaseType = iTween.EaseType.easeInOutExpo;

    public List<Transform> capturedPositions;
    private int m_currentCapturePosition = 0;
    public int CurrentCapturePosition
    {
        get => m_currentCapturePosition;
        set => m_currentCapturePosition = value;
    }

    [SerializeField] private float capturePositionIconSize = 0.4f;
    [SerializeField] private Color capturePositionIconColor = Color.blue;
    
    private void Awake()
    {
        m_player = GameObject.FindObjectOfType<PlayerMover>().GetComponent<PlayerMover>();
        GetNodeList();

        m_goalNode = FindGoalNode();
    }

    // sets the AllNodes and m_allNodes fields
    public void GetNodeList()
    {
        Node[] nList = GameObject.FindObjectsOfType<Node>();
        m_allNodes = new List<Node>(nList);
    }

    // returns a Node at a given position
    public Node FindNodeAt(Vector3 pos)
    {
        Vector2 boardCoord = Utility.Vector2Round(new Vector2(pos.x, pos.z));
        return m_allNodes.Find(n => n.Coordinate == boardCoord);
    }

    private Node FindGoalNode()
    {
        return m_allNodes.Find(n => n.IsLevelGoal);
    }

    // return the PlayerNode
    public Node FindPlayerNode()
    {
        if (m_player != null && !m_player.isMoving)
        {
            return FindNodeAt(m_player.transform.position);
        }

        return null;
    }

    public List<EnemyManager> FindEnemiesAt(Node node)
    {
        List<EnemyManager> foundEnemies = new List<EnemyManager>();
        EnemyManager[] enemies = GameObject.FindObjectsOfType<EnemyManager>() as EnemyManager[];

        foreach (EnemyManager enemy in enemies)
        {
            EnemyMover mover = enemy.GetComponent<EnemyMover>();

            if (mover.CurrentNode == node)
            {
                foundEnemies.Add(enemy);
            }
        }

        return foundEnemies;
    }

    // set the m_playerNode
    public void UpdatePlayerNode()
    {
        m_playerNode = FindPlayerNode();
    }

    // draw a colored sphere at the PlayerNode
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 1f, 1f, 0.5f);
        if (m_playerNode != null)
        {
            Gizmos.DrawSphere(m_playerNode.transform.position, 0.2f);
        }

        Gizmos.color = capturePositionIconColor;
        
        foreach (Transform capturePos in capturedPositions)
        {
            Gizmos.DrawCube(capturePos.position, Vector3.one * capturePositionIconSize);
        }
    }

    // draw the Goal prefab at the Goal node
    public void DrawGoal()
    {
        if (goalPrefab != null && m_goalNode != null)
        {
            GameObject goalInstance = Instantiate(goalPrefab, m_goalNode.transform.position, Quaternion.identity);
            iTween.ScaleFrom(goalInstance, iTween.Hash(
                "scale", Vector3.zero,
                "time", drawGoalTime,
                "delay", drawGoalDelay,
                "easetype", drawGoalEaseType
                ));
        }
    }

    public void InitBoard()
    {
        if (m_playerNode != null)
        {
            m_playerNode.InitNode();
        }
    }
}
