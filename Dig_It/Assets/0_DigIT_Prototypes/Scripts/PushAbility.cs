using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushAbility : MonoBehaviour
{
    // Configuration
    public float pushReleaseTimer = 0.5f;
    private float initialPushTimer;
    public float interpolationPushAmount = 0.2f;
    public float offsetRay = 0.1f;
    public float offsetRayDistance = .1f;
    public float cellSize = 0.95f;
    bool canBePushed = true;
    Vector3 collisionCheckDistance;
    Vector3 currentDirectionPush;

    // Cached Components
    public LayerMask playerCollisionMask;
    public LayerMask pushedBlockCollisionMask;
    private GameObject blockToPush;

    // Start is called before the first frame update
    void Start()
    {
        initialPushTimer = pushReleaseTimer;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CheckPushConditions(GetComponentInParent<Player>().MovementAmount);
        
        if(GetComponentInParent<Player>().CurrentState == PlayerState.Pushing)
        {
            pushReleaseTimer -= Time.fixedDeltaTime;

            if(pushReleaseTimer <= 0 && canBePushed)
            {
                PushSolidBlock();
            }
        }
        else
        {
            pushReleaseTimer = initialPushTimer;
            canBePushed = true;
        }
    }

    private void CheckPushConditions(Vector3 currentDirection)
    {
        int layerMask = playerCollisionMask.value;
        collisionCheckDistance = currentDirection * offsetRay;

        RaycastHit2D checkDistance = Physics2D.Raycast(transform.position + collisionCheckDistance, currentDirection, offsetRayDistance, layerMask);

        Debug.DrawRay(transform.position + collisionCheckDistance, currentDirection * offsetRayDistance, Color.red);

        if (checkDistance.collider != null)
        {
            if (checkDistance.collider.gameObject.tag == "SolidBlock")
            {
                switch (GetComponentInParent<Player>().CurrFacingDirection)
                {
                    case FacingDirection.Down:
                        currentDirectionPush = Vector3.down;
                        break;
                    case FacingDirection.Left:
                        currentDirectionPush = Vector3.left;
                        break;
                    case FacingDirection.Right:
                        currentDirectionPush = Vector3.right;
                        break;
                    case FacingDirection.Up:
                        currentDirectionPush = Vector3.up;
                        break;
                }

                blockToPush = checkDistance.collider.gameObject;
                int layerMaskBlock = pushedBlockCollisionMask.value;

                collisionCheckDistance = currentDirectionPush * offsetRay;

                RaycastHit2D checkDistanceBlock = Physics2D.Raycast(blockToPush.transform.position + collisionCheckDistance, currentDirectionPush, offsetRayDistance, layerMaskBlock);

                Debug.DrawRay(blockToPush.transform.position + collisionCheckDistance, currentDirectionPush * offsetRayDistance, Color.red);

                if (checkDistanceBlock.collider != null)
                {
                    GetComponentInParent<Player>().CurrentState = PlayerState.Walking;
                    canBePushed = false;
                }
                else
                {
                    GetComponentInParent<Player>().CurrentState = PlayerState.Pushing;
                    canBePushed = true;
                }
            }
            else
            {
                GetComponentInParent<Player>().CurrentState = PlayerState.Walking;
            }
        }
    }

    private void PushSolidBlock()
    {
        canBePushed = false;
        currentDirectionPush.x *= cellSize;
            //blockToPush.GetComponent<Rigidbody2D>().Sleep();
            // blockToPush.transform.position = Vector3.Lerp(transform.position, transform.position + currentDirectionPush, interpolationPushAmount);
        blockToPush.GetComponent<Rigidbody2D>().MovePosition(blockToPush.transform.position + currentDirectionPush);   
    }
}
