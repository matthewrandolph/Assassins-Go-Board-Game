using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

public class Board : MonoBehaviour
{
    public static float spacing = 2f;

    public static readonly Vector2[] directions =
    {
        new Vector2(spacing, 0f),
        new Vector2(-spacing, 0f),
        new Vector2(0f, spacing),
        new Vector2(0f, -spacing)
    };

    private Node m_playerNode;
    public Node PlayerNode { get { return m_playerNode; }}

    private List<Node> m_allNodes = new List<Node>();
    public List<Node> AllNodes { get { return m_allNodes; }}

    private PlayerMover m_player;

    private Node m_goalNode;
    public Node GoalNode => m_goalNode;

    [SerializeField] private GameObject goalPrefab;
    [SerializeField] private float drawGoalTime = 2f;
    [SerializeField] private float drawGoalDelay = 2f;
    [SerializeField] private iTween.EaseType drawGoalEaseType = iTween.EaseType.easeInOutExpo;

    private void Awake()
    {
        m_player = GameObject.FindObjectOfType<PlayerMover>().GetComponent<PlayerMover>();
        GetNodeList();

        m_goalNode = FindGoalNode();
    }

    public void GetNodeList()
    {
        Node[] nList = GameObject.FindObjectsOfType<Node>();
        m_allNodes = new List<Node>(nList);
    }

    public Node FindNodeAt(Vector3 pos)
    {
        Vector2 boardCoord = Utility.Vector2Round(new Vector2(pos.x, pos.z));
        return m_allNodes.Find(n => n.Coordinate == boardCoord);
    }

    private Node FindGoalNode()
    {
        return m_allNodes.Find(n => n.IsLevelGoal);
    }

    public Node FindPlayerNode()
    {
        if (m_player != null && !m_player.isMoving)
        {
            return FindNodeAt(m_player.transform.position);
        }

        return null;
    }

    public void UpdatePlayerNode()
    {
        m_playerNode = FindPlayerNode();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 1f, 1f, 0.5f);
        if (m_playerNode != null)
        {
            Gizmos.DrawSphere(m_playerNode.transform.position, 0.2f);
        }
    }

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
