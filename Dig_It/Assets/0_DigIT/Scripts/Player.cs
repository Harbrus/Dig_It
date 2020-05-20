using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Configuration
    public float speed;
    private Vector3 movement;

    // State

    // Cached Components
    Rigidbody2D myRigidbody;
    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        movement = Vector3.zero;
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        if(movement!=Vector3.zero)
        {
            MoveCharacter();
            anim.SetFloat("moveX", movement.x);
            anim.SetFloat("moveY", movement.y);
        }
    }

    private void MoveCharacter()
    {
        myRigidbody.MovePosition(transform.position + movement * speed * Time.deltaTime);
    }
}
