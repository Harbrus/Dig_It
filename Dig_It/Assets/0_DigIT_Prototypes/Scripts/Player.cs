using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    Idle, Walking, Digging, Dead, Frozen, Pushing,
    Stuck
}
public enum FacingDirection
{
    Up, Right, Down, Left
}


public class Player : MonoBehaviour
{
    // Configuration
    public float maxSpeed;
    public float currSpeed;
    public float acceleration;
    public float deceleration;

    public float digCD;
    public float currDigCD;
    public float moveLimiter = 0.7f;
    private Vector3 movementAmount;
    private int previousPoint;
    public float decreaseSpeedAmount = 0.25f;
    public float stunTimer = 3f;
    public bool fallenInAHole = false;
    public float raiseFromHoleForce = 0.2f;

    // State
    public PlayerState CurrentState;
    public FacingDirection CurrFacingDirection = FacingDirection.Down;
    public FacingDirection LastFacingDirection = FacingDirection.Down;

    // Cached Components
    public Rigidbody2D MyRigidbody;


    public Animator CharacterAnimator;


    public Vector3 MovementAmount { get => movementAmount;}

    // Start is called before the first frame update
    void Start()
    {
        MyRigidbody = GetComponent<Rigidbody2D>();
        CharacterAnimator = GetComponent<Animator>();
        currDigCD = digCD;
    }

    // Update is called once per frame
    void Update()
    {
        if(previousPoint != GameManager.Instance.Points && maxSpeed > 1)
        {
            if(previousPoint > GameManager.Instance.Points)
            {
                maxSpeed += decreaseSpeedAmount;
            }
            else
            {
                maxSpeed -= decreaseSpeedAmount;
            }

            previousPoint = GameManager.Instance.Points;
        }

        if (this.gameObject.GetComponent<Health>().Invulnerable)
        {
            movementAmount = Vector3.zero;
            CurrentState = PlayerState.Frozen;
            return;
        }

        movementAmount = Vector3.zero;
        movementAmount.x = Input.GetAxisRaw("Horizontal");
        movementAmount.y = Input.GetAxisRaw("Vertical");
        
        if (Input.GetButtonDown("Dig") && CurrentState != PlayerState.Digging && currDigCD <= 0
            && !fallenInAHole)
        {
            Dig();
            currDigCD = digCD;
        }

        if(currDigCD > 0)
        {
            currDigCD -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        if (CurrentState == PlayerState.Stuck || CurrentState == PlayerState.Digging || CurrentState == PlayerState.Dead)
        {
            return;
        }

        if (movementAmount != Vector3.zero)
        {
            CurrentState = PlayerState.Walking;
            MoveCharacter();
            CharacterAnimator.SetFloat("moveX", movementAmount.x);
            CharacterAnimator.SetFloat("moveY", movementAmount.y);
            SetFacingDirection();
        }
        else if (currSpeed > 0)
        {
            currSpeed -= deceleration * Time.fixedDeltaTime;
            CurrentState = PlayerState.Idle;
        }
    }

    private void Dig()
    {
        CurrentState = PlayerState.Digging;
        StartCoroutine(this.GetComponent<DigAbility>().ActivateCollider());
    }

    private void SetFacingDirection()
    {
        if (Mathf.Abs(movementAmount.y) > Mathf.Abs(movementAmount.x))
        {
            CurrFacingDirection = (movementAmount.y > 0) ? FacingDirection.Up: FacingDirection.Down;
            LastFacingDirection = CurrFacingDirection;
        }
        else if (Mathf.Abs(movementAmount.y) < Mathf.Abs(movementAmount.x))
        {
            CurrFacingDirection = (movementAmount.x > 0) ? FacingDirection.Right : FacingDirection.Left;
            LastFacingDirection = CurrFacingDirection;
        }
        else
        {
            CurrFacingDirection = LastFacingDirection;
        }
    }

    private void MoveCharacter()
    {
        if (currSpeed < maxSpeed)
        {
            currSpeed += acceleration * Time.fixedDeltaTime;
        }

        Vector3 movementVector = Vector2.ClampMagnitude(movementAmount, 1);
        MyRigidbody.MovePosition(transform.position + movementVector * currSpeed * Time.fixedDeltaTime);
    }

    public IEnumerator FallInAHole(Vector3 holePolision)
    {
        transform.position = holePolision;
        GetComponent<SpriteRenderer>().color = Color.gray;
        transform.localScale /= 2;
        fallenInAHole = true;
        CurrentState = PlayerState.Stuck;
        yield return new WaitForSeconds(stunTimer);
        GetComponent<SpriteRenderer>().color = Color.white;
        CurrentState = PlayerState.Idle;
    }

    internal void JumpOutsideHole()
    {
        switch (CurrFacingDirection)
        {
            case FacingDirection.Down:
                transform.position += new Vector3(0, -raiseFromHoleForce, 0f);
                break;
            case FacingDirection.Up:
                transform.position += new Vector3(0, raiseFromHoleForce, 0f);
                break;
            case FacingDirection.Right:
                transform.position += new Vector3(raiseFromHoleForce, 0, 0f);
                break;
            case FacingDirection.Left:
                transform.position += new Vector3(-raiseFromHoleForce, 0, 0f);
                break;
        }
       
        fallenInAHole = false;
        transform.localScale *= 2;
    }

    // reset player position.
}
