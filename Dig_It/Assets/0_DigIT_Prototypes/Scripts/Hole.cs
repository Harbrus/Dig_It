using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hole : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "PlayerFeet" && collision.GetComponentInParent<Player>().CurrentState != PlayerState.Stuck 
            && !collision.GetComponentInParent<Player>().fallenInAHole)
        {
            StartCoroutine(collision.GetComponentInParent<Player>().FallInAHole(transform.position));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "PlayerFeet" && collision.GetComponentInParent<Player>().fallenInAHole)
        {
            collision.GetComponentInParent<Player>().JumpOutsideHole();
        }
    }
}
