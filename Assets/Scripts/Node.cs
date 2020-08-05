using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    private Vector2 m_coordinate;
    public Vector2 Coordinate { get { return Utility.Vector2Round(m_coordinate); }}
    
    List<Node> m_neighborNodes = new List<Node>();

    public List<Node> NeighborNodes { get { return m_neighborNodes; } }

    private List<Node> m_linkedNodes = new List<Node>();
    public List<Node> LinkedNodes { get { return m_linkedNodes; } }

    private Board m_board;

    [SerializeField] private GameObject geometry;
   
    [SerializeField] private GameObject linkPrefab;

    [SerializeField] private float scaleTime = 0.3f;
    
    [SerializeField] private iTween.EaseType easeType = iTween.EaseType.easeInExpo;

    [SerializeField] private float delay = 1f;

    private bool m_isInitialized = false;

    public LayerMask obstacleLayer;

    void Awake()
    {
        m_board = Object.FindObjectOfType<Board>();
        m_coordinate = new Vector2(transform.position.x, transform.position.z);
    }
    void Start()
    {
        if (geometry != null)
        {
            geometry.transform.localScale = Vector3.zero;
        }

        if (m_board != null)
        {
            m_neighborNodes = FindNeighbors(m_board.AllNodes);
        }
    }

    public void ShowGeometry()
    {
        if (geometry != null)
        {
            iTween.ScaleTo(geometry, iTween.Hash(
                "time", scaleTime,
                "scale", Vector3.one,
                "easetype", easeType,
                "delay", delay
            ));
        }
    }

    public List<Node> FindNeighbors(List<Node> nodes)
    {
        List<Node> nList = new List<Node>();

        foreach (Vector2 dir in Board.directions)
        {
            Node foundNeighbor = nodes.Find(n => n.Coordinate == Coordinate + dir);

            if (foundNeighbor != null && !nList.Contains(foundNeighbor))
            {
                nList.Add(foundNeighbor);
            }
        }
        
        return nList;
    }

    public void InitNode()
    {
        if (!m_isInitialized)
        {
            ShowGeometry();
            InitNeighbors();
            m_isInitialized = true;
        }
    }

    private void InitNeighbors()
    {
        StartCoroutine(InitNeighborsRoutine());
    }

    IEnumerator InitNeighborsRoutine()
    {
        yield return new WaitForSeconds(delay);

        foreach (Node n in m_neighborNodes)
        {
            if (!m_linkedNodes.Contains(n))
            {
                Obstacle obstacle = FindObstacle(n);
                if (obstacle == null)
                {
                    LinkNode(n);
                    n.InitNode();
                }
            }
        }
    }

    void LinkNode(Node targetNode)
    {
        if (linkPrefab != null)
        {
            GameObject linkInstance = Instantiate(linkPrefab, transform.position, Quaternion.identity);
            linkInstance.transform.parent = transform;

            Link link = linkInstance.GetComponent<Link>();
            if (link != null)
            {
                link.DrawLink(transform.position, targetNode.transform.position);
            }

            if (!m_linkedNodes.Contains(targetNode))
            {
                m_linkedNodes.Add(targetNode);
            }

            if (!targetNode.LinkedNodes.Contains(this))
            {
                targetNode.LinkedNodes.Add(this);
            }
        }
    }

    Obstacle FindObstacle(Node targetNode)
    {
        Vector3 checkDirection = targetNode.transform.position - transform.position;
        RaycastHit raycastHit;

        if (Physics.Raycast(transform.position, checkDirection, out raycastHit, Board.spacing + 0.1f, obstacleLayer))
        {
            Debug.Log("NODE FindObstacle: Hit an obstacle from " + this.name + " to " + targetNode.name);
            return raycastHit.collider.GetComponent<Obstacle>();
        }

        return null;
    }
}
