using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;


public class CommonGhost : MonoBehaviour
{
    enum BehaviourStates { Moving, Turning, Fleeing };
    public enum FacingDirection { Up, Right, Down, Left }
    public float speed = 2f;
    float initialSpeed;
    public bool hasTreasure = false;
    bool reverseDirectionNow = false;
    public float collisionCheckDistance = 0f;
    private float initialCheckDistance;
    public float offsetRay = 0.5f;
    bool randomTurn = true;
    bool depositJewel = false;
    int numberOfStolenJewel = 0;
    GameObject blockToDeposit;

    Vector3 currentDirection;
    bool startDirection = true;
    public float turningWait = 0.5f;
    [SerializeField] protected FacingDirection CurrentFacing;
    [SerializeField] FacingDirection InitialFacing = FacingDirection.Down;
    BehaviourStates CurrentBehaviour = BehaviourStates.Moving;
    bool canMove = true;
    public LayerMask collisionMask; 
    public LayerMask collisionMaskOnlyOuterWall;
    public LayerMask ground;
    public float respawnTimer = 1.5f;
    public GameObject spawnPosition;
    private Vector3 spawnPos;
    private bool isRespawing = false;
    private bool ignoreCollisionWithBlocks = false;
    private bool checkCollisionAfterRespawn = false;
    int layerMask;
    public FacingDirection CurrentFacingDirection { get => CurrentFacing; set => CurrentFacing = value; }

    // Start is called before the first frame update
    void Awake()
    {
        CurrentFacing = InitialFacing;
        initialCheckDistance = collisionCheckDistance;
        initialSpeed = speed;
        spawnPos = spawnPosition.transform.position;

        if (GetComponent<Steal>())
        {
            GetComponent<Steal>().jewelstolen += HasTreasure;
        }


        StartCoroutine(Turn());
    }

    // Update is called once per frame
    void Update()
    {
        if(isRespawing)
        {
            return;
        }

        if (hasTreasure)
        {
            randomTurn = false;
            CurrentBehaviour = BehaviourStates.Fleeing;
            Fleeing();
        }
        else
        {
            Move();
        }
    }

    public void HasTreasure()
    {
        hasTreasure = true;
        reverseDirectionNow = true;
        numberOfStolenJewel++;
        GetComponent<SpriteRenderer>().color = Color.yellow;
    }

    private void Move()
    {
        CheckNextMove();
       
        if (canMove && CurrentBehaviour != BehaviourStates.Turning)
        {
            transform.position += currentDirection * speed * Time.deltaTime;
        }
    }

    private void CheckNextMove()
    {
        if(checkCollisionAfterRespawn)
        {
            RaycastHit2D checkDistanceGround = Physics2D.Raycast(transform.position, currentDirection, 0.1f, ground.value);

            if(checkDistanceGround.collider !=null)
            {
                if(checkDistanceGround.collider.gameObject.tag != "Ground")
                {
                    ignoreCollisionWithBlocks = true;
                }
                else
                {
                    ignoreCollisionWithBlocks = false;
                    checkCollisionAfterRespawn = false;
                }
            }
        }

        if (ignoreCollisionWithBlocks)
        {
            layerMask = collisionMaskOnlyOuterWall.value;
        }
        else
        {
            layerMask = collisionMask.value;
        }

        Vector3 offSetRayOrigin = currentDirection * offsetRay;

        RaycastHit2D checkDistance = Physics2D.Raycast(transform.position + offSetRayOrigin, currentDirection, collisionCheckDistance, layerMask);

        if (checkDistance.collider != null)
        {
            Debug.Log("Did Hit " + checkDistance.collider.gameObject.name.ToString());

            if (hasTreasure && (checkDistance.collider.gameObject.tag == "Block" || checkDistance.collider.gameObject.tag == "SolidBlock"))
            {
                depositJewel = true;
                blockToDeposit = checkDistance.collider.gameObject;
                return;
            }

            StartCoroutine(Turn());
        }
        else if (reverseDirectionNow)
        {
            StartCoroutine(Turn());
            reverseDirectionNow = false;
        }
    }

    private void UpdateFacingDirection()
    {
        if (currentDirection.x > currentDirection.y)
        {
            if (currentDirection.x > 0)
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
        else if (!randomTurn)
        {
            currentDirection *= -1;
            UpdateFacingDirection();
        }

        if (randomTurn)
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

        if (hasTreasure)
        {
            CurrentBehaviour = BehaviourStates.Fleeing;
        }
        else
        {
            CurrentBehaviour = BehaviourStates.Moving;
        }

        randomTurn = true;

        if (startDirection)
        {
            startDirection = false;
        }
    }
    private void Fleeing()
    {
        if (depositJewel)
        {
            //speed--;
            DepositJewel();
            GetComponent<SpriteRenderer>().color = Color.white;
        }

        if (speed == initialSpeed)
        {
            //speed++;
        }

        Move();
    }

    private void DepositJewel()
    {
        // deposit jewel on target block 
        blockToDeposit.GetComponent<JewelSpawner>().IncreaseJewels(numberOfStolenJewel);
        hasTreasure = false;
        depositJewel = false;
        randomTurn = true;
        numberOfStolenJewel = 0;
        collisionCheckDistance = initialCheckDistance;
    }

    private void OnDrawGizmos()
    {
        Vector3 offSetRayOrigin = Vector3.up * offsetRay;
        Debug.DrawRay(transform.position + offSetRayOrigin, Vector3.up * collisionCheckDistance, Color.red);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "SolidBlock" && !checkCollisionAfterRespawn)
        {
            StartCoroutine(Respawn());
        }
        else if (collision.gameObject.tag == "Jewel")
        {
            if (hasTreasure)
            {
                randomTurn = true;
                StartCoroutine(Turn());
            }
            else
            {
                collision.gameObject.GetComponent<Jewel>().CollectAndDestroy();
                HasTreasure();
            }

        }
    }

    private IEnumerator Respawn()
    {
        // set collider to false 
        isRespawing = true;
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        yield return new WaitForSeconds(respawnTimer);
        this.transform.position = spawnPos;
        GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<Collider2D>().enabled = true;
        isRespawing = false;
        checkCollisionAfterRespawn = true;
    }
}
