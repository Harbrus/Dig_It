using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;


public class CommonGhost : MonoBehaviour
{
    enum BehaviourStates {Moving, Turning, Fleeing};
    public enum FacingDirection { Up, Right, Down, Left}
    public float speed = 3f;
    bool hasTreasure = false;
    bool reverseDirectionNow = false;
    public float collisionCheckDistance = 0f;
    private float initialCheckDistance;
    public float offsetRay = 0.5f;
    bool randomTurn = true;
    bool depositJewel = false;

    GameObject blockToDeposit;

    Vector3 currentDirection;
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
        initialCheckDistance = collisionCheckDistance;
        StartCoroutine(Turn());
    }

    // Update is called once per frame
    void Update()
    {
        if (hasTreasure) 
        { 
            randomTurn = false;
            CurrentBehaviour = BehaviourStates.Fleeing;
            CheckNextMove();
            Fleeing();
        }
        else
        {
            CheckNextMove();
            Move();
        }
    }

    private void Move()
    {
        if(canMove && CurrentBehaviour != BehaviourStates.Turning)
        {
            transform.position += currentDirection * speed * Time.deltaTime;
        }
    }

    private void CheckNextMove()
    {
        int layerMask = collisionMask.value;
        Vector3 offSetRayOrigin = currentDirection * offsetRay;
        
        if(hasTreasure)
        {
            collisionCheckDistance = 0;
        }
        
        RaycastHit2D checkDistance = Physics2D.Raycast(transform.position + offSetRayOrigin, currentDirection, collisionCheckDistance, layerMask);
        
        Debug.DrawRay(transform.position, currentDirection * checkDistance.distance, Color.red);

        if (checkDistance.collider != null)
        {
            Debug.Log("Did Hit" + checkDistance.collider.gameObject.tag.ToString());
            
            if (checkDistance.collider.gameObject.tag == "Enemy")
            {
                randomTurn = false;
            }
            else if(hasTreasure && checkDistance.collider.gameObject.tag == "Block")
            {
                depositJewel = true;
                blockToDeposit = checkDistance.collider.gameObject;
                return;
            }

            StartCoroutine(Turn());
        }
        else if(reverseDirectionNow)
        {
            StartCoroutine(Turn());
            reverseDirectionNow = false;
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

        if(hasTreasure)
        {
            CurrentBehaviour = BehaviourStates.Fleeing;
        }
        else
        {
            CurrentBehaviour = BehaviourStates.Moving;
        }

        randomTurn = true;

        if(startDirection)
        { 
            startDirection = false; 
        }
    }
    private void Fleeing()
    {
        Move();
        if (depositJewel)
        {
            DepositJewel();
        }
    }

    private void DepositJewel()
    {
        // deposit jewel on target block 
        blockToDeposit.GetComponent<JewelSpawner>().IncreaseJewels();
        hasTreasure = false;
        randomTurn = true;
        collisionCheckDistance = initialCheckDistance;
    }

    public void StealJewel()
    {
        hasTreasure = true;
        reverseDirectionNow = true;
    }

    private void OnDrawGizmos()
    {
        Vector3 offSetRayOrigin = Vector3.up * offsetRay;
        Debug.DrawRay(transform.position + offSetRayOrigin, Vector3.up * collisionCheckDistance, Color.red);
    }
}
