using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MovementType
{
    Stationary,
    Patrol,
    Spinner
}

public class EnemyMover : Mover
{
    public Vector3 directionToMove = new Vector3(0f, 0f, Board.spacing);

    public MovementType movementType = MovementType.Stationary;
    
    // wait time for stationary enemies
    [SerializeField] private float standTime = 1f;
    
    protected override void Awake()
    {
        base.Awake();
        
        // EnemyMovers always face the direction they are moving
        faceDestination = true;
    }
    
    protected override void Start()
    {
        base.Start();
    }

    public void MoveOneTurn()
    {
        switch (movementType)
        {
            case MovementType.Patrol:
                Patrol();
                break;
            case MovementType.Stationary:
                Stand();
                break;
            case MovementType.Spinner:
                Spin();
                break;
        }
        
    }

    private void Patrol()
    {
        StartCoroutine(PatrolRoutine());
    }

    private IEnumerator PatrolRoutine()
    {
        Vector3 startPos = new Vector3(CurrentNode.Coordinate.x, 0f, CurrentNode.Coordinate.y);

        // one space forward
        Vector3 newDest = startPos + transform.TransformVector(directionToMove);
        
        // two spaces forward
        Vector3 nextDest = startPos + transform.TransformVector(directionToMove * 2f);

        Move(newDest, 0f);

        while (isMoving)
        {
            yield return null;
        }

        if (m_board != null)
        {
            Node newDestNode = m_board.FindNodeAt(newDest);
            Node nextDestNode = m_board.FindNodeAt(nextDest);

            if (nextDestNode == null || !newDestNode.LinkedNodes.Contains(nextDestNode))
            {
                destination = startPos;
                FaceDestination();

                yield return new WaitForSeconds(rotateTime);
            }
        }
        
        // broadcast message at end of movement
        base.finishMovementEvent.Invoke();
    }

    // movement turn for stationary enemies
    private void Stand()
    {
        StartCoroutine(StandRoutine());
    }

    private IEnumerator StandRoutine()
    {
        // time to wait
        yield return new WaitForSeconds(standTime);
        
        // broadcast message at end of movement
        base.finishMovementEvent.Invoke();
    }

    private void Spin()
    {
        StartCoroutine(SpinRoutine());
    }

    private IEnumerator SpinRoutine()
    {
        Vector3 localForward = new Vector3(0f, 0f, Board.spacing);
        destination = transform.TransformDirection(localForward * -1f) + transform.position;
        
        FaceDestination();
        
        yield return new WaitForSeconds(rotateTime);
        
        base.finishMovementEvent.Invoke();
    }
}
