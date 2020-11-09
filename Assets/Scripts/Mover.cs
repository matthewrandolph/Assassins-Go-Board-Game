using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// base class for all moving game pieces
public class Mover : MonoBehaviour
{
    // where we are currently headed
    public Vector3 destination;
    
    // option to face the direction of movement
    public bool isMoving = false;
    
    // are we currently moving?
    protected bool faceDestination = false;
    
    // what easetype to use for iTweening
    [SerializeField] private iTween.EaseType easeType = iTween.EaseType.easeInOutExpo;

    // how fast we move
    [SerializeField] private float moveSpeed = 1.5f;
    
    // time to rotate to face destination
    [SerializeField] protected float rotateTime = 0.5f;
    
    // delay to use before any call to iTween
    [SerializeField] private float iTweenDelay = 0f;

    // reference to Board
    protected Board m_board;
    
    // the current Node on the board
    protected Node m_currentNode;
    public Node CurrentNode => m_currentNode;
    
    public UnityEvent finishMovementEvent;

    // setup the Mover
    protected virtual void Awake()
    {
        m_board = Object.FindObjectOfType<Board>().GetComponent<Board>();
    }
    
    protected virtual void Start()
    {
        UpdateCurrentNode();
    }

    public void Move(Vector3 destinationPos, float delayTime = 0.25f)
    {
        if (isMoving)
        {
            return;
        }
        
        if (m_board != null)
        {
            Node targetNode = m_board.FindNodeAt(destinationPos);

            if (targetNode != null && m_currentNode != null)
            {
                if (m_currentNode.LinkedNodes.Contains(targetNode))
                {
                    StartCoroutine(MoveRoutine(destinationPos, delayTime));
                }
                else
                {
                    Debug.Log("MOVER: " + m_currentNode.name + " not connected " + targetNode.name);
                }
            }
        }
    }

    protected virtual IEnumerator MoveRoutine(Vector3 destinationPos, float delayTime)
    {
        isMoving = true;
        destination = destinationPos;
        
        // optional turn to face destination
        if (faceDestination)
        {
            FaceDestination();
            yield return new WaitForSeconds(0.25f);
        }
        
        yield return new WaitForSeconds(delayTime);
        iTween.MoveTo(gameObject, iTween.Hash(
                "x", destinationPos.x,
                "y", destinationPos.y,
                "z", destinationPos.z,
                "delay", iTweenDelay,
                "easetype", easeType,
                "speed", moveSpeed
            ));

        while (Vector3.Distance(destinationPos, transform.position) > 0.01f)
        {
            yield return null;
        }

        iTween.Stop(gameObject);
        transform.position = destinationPos;
        isMoving = false;
        
        UpdateCurrentNode();
    }

    public void MoveLeft()
    {
        Vector3 newPosition = transform.position + new Vector3(-Board.spacing, 0f, 0f);
        Move(newPosition, 0);
    }

    public void MoveRight()
    {
        Vector3 newPosition = transform.position + new Vector3(Board.spacing, 0f, 0f);
        Move(newPosition, 0);
    }

    public void MoveForward()
    {
        Vector3 newPosition = transform.position + new Vector3(0f, 0f, Board.spacing);
        Move(newPosition, 0);
    }

    public void MoveBackward()
    {
        Vector3 newPosition = transform.position + new Vector3(0f, 0f, -Board.spacing);
        Move(newPosition, 0);
    }

    protected void UpdateCurrentNode()
    {
        if (m_board != null)
        {
            m_currentNode = m_board.FindNodeAt(transform.position);
        }
    }

    protected void FaceDestination()
    {
        Vector3 relativePosition = destination - transform.position;

        Quaternion newRotation = Quaternion.LookRotation(relativePosition, Vector3.up);

        float newY = newRotation.eulerAngles.y;
        
        iTween.RotateTo(gameObject, iTween.Hash(
            "y", newY,
            "delay", 0f,
            "easetype", easeType,
            "time", rotateTime
            ));
    }
}
