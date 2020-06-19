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
    Vector3 collisionCheckDistance;

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
    void Update()
    {
        CheckPushConditions(GetComponent<Player>().MovementAmount);
        
        while(GetComponent<Player>().CurrentState == PlayerState.Pushing)
        {
            pushReleaseTimer -= Time.deltaTime;

            if(pushReleaseTimer <= 0)
            {
                PushSolidBlock();
            }
        }

        pushReleaseTimer = initialPushTimer;
    }

    private void CheckPushConditions(Vector3 currentDirection)
    {
        int layerMask = playerCollisionMask.value;
        collisionCheckDistance = currentDirection * offsetRay;

        RaycastHit2D checkDistance = Physics2D.Raycast(transform.position, currentDirection, collisionCheckDistance.magnitude, layerMask);

        Debug.DrawRay(transform.position, currentDirection * checkDistance.distance, Color.red);

        if (checkDistance.collider != null)
        {
            if (checkDistance.collider.gameObject.tag == "SolidBlock")
            {
                GetComponent<Player>().CurrentState = PlayerState.Pushing;
                blockToPush = checkDistance.collider.gameObject;
            }
            else
            {
                GetComponent<Player>().CurrentState = PlayerState.Walking;
            }
        }
    }

    private void PushSolidBlock()
    {
        var currentDirectionPush = Vector3.zero;

        switch (GetComponent<Player>().CurrFacingDirection)
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

        int layerMask = pushedBlockCollisionMask.value;

        collisionCheckDistance = currentDirectionPush * offsetRay;

        RaycastHit2D checkDistance = Physics2D.Raycast(transform.position, currentDirectionPush, collisionCheckDistance.magnitude, layerMask);

        if (checkDistance.collider != null)
        {
            GetComponent<Player>().CurrentState = PlayerState.Walking;
        }
        else
        {
            blockToPush.transform.position = Vector3.Lerp(transform.position, transform.position + currentDirectionPush, interpolationPushAmount);
        }    
    }

}
