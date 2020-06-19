using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    Idle, Walking, Digging, Dead, Frozen, Pushing
}
public enum FacingDirection
{
    Up, Right, Down, Left
}


public class Player : MonoBehaviour
{
    // Configuration
    public float speed;
    public float digCD;
    public float currDigCD;
    public float moveLimiter = 0.7f;
    private Vector3 movementAmount;
    
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
                
        if (this.gameObject.GetComponent<Health>().Invulnerable)
        {
            movementAmount = Vector3.zero;
            CurrentState = PlayerState.Frozen;
            return;
        }

        movementAmount = Vector3.zero;
        movementAmount.x = Input.GetAxisRaw("Horizontal");
        movementAmount.y = Input.GetAxisRaw("Vertical");
        
        if (Input.GetButtonDown("Fire1") && CurrentState != PlayerState.Digging && currDigCD <= 0)
        {
            Dig();
            currDigCD = digCD;
        }

        currDigCD -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        if (movementAmount != Vector3.zero && (CurrentState != PlayerState.Digging || CurrentState != PlayerState.Frozen))
        {
            CurrentState = PlayerState.Walking;
            MoveCharacter();
            CharacterAnimator.SetFloat("moveX", movementAmount.x);
            CharacterAnimator.SetFloat("moveY", movementAmount.y);
            SetFacingDirection();
        }
        else
        {
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

        if (movementAmount.x != 0 && movementAmount.y != 0) // Check for diagonal movement
        {
            // limit movement speed diagonally, so you move at 70% speed
            movementAmount *= moveLimiter;
            movementAmount.y *= moveLimiter;
        }

        MyRigidbody.MovePosition(transform.position + movementAmount * speed * Time.fixedDeltaTime);
    }

    // reset player position.
}
