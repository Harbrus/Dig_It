using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;


public class CommonGhost : MonoBehaviour
{
    enum BehaviourStates {Moving, Turning, Flee, Waiting};
    public enum FacingDirection { Up, Right, Down, Left}
    public float speed = 3f;
    bool hasTreasure = false;
    public float collisionCheckDistance = 0f;
    public float offsetRay = 0.5f;
    bool randomTurn = true;

    Vector3 currentDirection;
    Rigidbody2D rigidbody2D;
    bool startDirection = true;
    public float turningWait = 0.5f;
    [SerializeField] protected FacingDirection CurrentFacing;
    [SerializeField] FacingDirection InitialFacing = FacingDirection.Down;
    BehaviourStates CurrentBehaviour = BehaviourStates.Moving;
    bool canMove = true;
    public LayerMask collisionMask;

    public FacingDirection CurrentFacingDirection { get => CurrentFacing; set => CurrentFacing = value; }

    // Start is called before the first frame update
    void Awake()
    {
        CurrentFacing = InitialFacing;
        StartCoroutine(Turn());
    }

    // Update is called once per frame
    void Update()
    {
        if (hasTreasure) { PathFinding(); }
        else
        {
            CheckNextMove();
            PatrolMovement();
        }
    }

    private void PatrolMovement()
    {
        if(canMove && CurrentBehaviour == BehaviourStates.Moving)
        {
            transform.position += currentDirection * speed * Time.deltaTime;
        }
    }

    private void CheckNextMove()
    {
        int layerMask = collisionMask.value;
        Vector3 offSetRayOrigin = currentDirection * offsetRay;
        RaycastHit2D checkDistance = Physics2D.Raycast(transform.position + offSetRayOrigin, currentDirection, collisionCheckDistance, layerMask);
        Debug.DrawRay(transform.position, currentDirection * checkDistance.distance, Color.red);

        if (checkDistance.collider != null)
        {
            Debug.Log("Did Hit" + checkDistance.collider.gameObject.tag.ToString());
            
            if (checkDistance.collider.gameObject.tag == "Enemy")
            {
                randomTurn = false;
            }

            StartCoroutine(Turn());
        }
    }

    private void UpdateFacingDirection()
    {
        if(currentDirection.x > currentDirection.y)
        {
            if(currentDirection.x > 0)
            {
                CurrentFacing = FacingDirection.Right;
            }
            else
            {
                CurrentFacing = FacingDirection.Left;
            }
        }
        else
        {
            if (currentDirection.y > 0)
            {
                CurrentFacing = FacingDirection.Up;
            }
            else
            {
                CurrentFacing = FacingDirection.Down;
            }
        }
    }

    private IEnumerator Turn()
    {
        if (!startDirection && randomTurn)
        {
            CurrentFacing = (FacingDirection)UnityEngine.Random.Range(0, 4);
        }
        else if(!randomTurn)
        {
            currentDirection *= -1;
            UpdateFacingDirection();
            
        }

        if(randomTurn)
        {
            switch (CurrentFacing)
            {
                case FacingDirection.Down:
                    currentDirection = Vector3.down;
                    break;
                case FacingDirection.Left:
                    currentDirection = Vector3.left;
                    break;
                case FacingDirection.Right:
                    currentDirection = Vector3.right;
                    break;
                case FacingDirection.Up:
                    currentDirection = Vector3.up;
                    break;
            }
        }

        CurrentBehaviour = BehaviourStates.Turning;

        yield return new WaitForSeconds(turningWait);

        CurrentBehaviour = BehaviourStates.Moving;
        randomTurn = true;

        if (startDirection)
        {
            startDirection = false;
        }
    }
    private void PathFinding()
    {
        // Call pathfinding
        // If near the targeted block
        DepositJewel();
    }

    private void DepositJewel()
    {
        // deposit jewel on target block
        hasTreasure = false;
        randomTurn = false;
    }

    public void StealJewel()
    {
        // StealJewel logic

        hasTreasure = true;

        // if flee is not implemented
        LevelManager.Instance.currentJewelAvailable--;
    }

    private void OnDrawGizmos()
    {
        Vector3 offSetRayOrigin = Vector3.up * offsetRay;
        Debug.DrawRay(transform.position + offSetRayOrigin, Vector3.up * collisionCheckDistance, Color.red);
    }
}
