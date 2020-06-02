using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    Idle, Walking, Digging, Dead, Frozen
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
    private Vector3 movement;

    // State
    public PlayerState CurrentState;
    public FacingDirection CurrFacingDirection = FacingDirection.Down;
    public FacingDirection LastFacingDirection = FacingDirection.Down;

    // Cached Components
    public Rigidbody2D MyRigidbody;
    public Animator CharacterAnimator;

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
            CurrentState = PlayerState.Frozen;
            return;
        }

        movement = Vector3.zero;
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        SetFacingDirection();

        if(movement!=Vector3.zero)
        {
            CurrentState = PlayerState.Walking;
            MoveCharacter();
            CharacterAnimator.SetFloat("moveX", movement.x);
            CharacterAnimator.SetFloat("moveY", movement.y);
        }
        else
        {
            CurrentState = PlayerState.Idle;
        }

        if (Input.GetButtonDown("Fire1") && CurrentState != PlayerState.Digging && currDigCD <= 0)
        {
            Dig();
            currDigCD = digCD;
        }

        currDigCD -= Time.deltaTime;
    }

    private void Dig()
    {
        CurrentState = PlayerState.Digging;
    }

    private void SetFacingDirection()
    {

        if (Mathf.Abs(movement.y) > Mathf.Abs(movement.x))
        {
            CurrFacingDirection = (movement.y > 0) ? FacingDirection.Up: FacingDirection.Down;
            LastFacingDirection = CurrFacingDirection;
        }
        else if (Mathf.Abs(movement.y) < Mathf.Abs(movement.x))
        {
            CurrFacingDirection = (movement.x > 0) ? FacingDirection.Right : FacingDirection.Left;
            LastFacingDirection = CurrFacingDirection;
        }
        else
        {
            CurrFacingDirection = LastFacingDirection;
        }
    }

    private void MoveCharacter()
    {
        MyRigidbody.MovePosition(transform.position + movement * speed * Time.deltaTime);
    }

    // reset player position.
}
