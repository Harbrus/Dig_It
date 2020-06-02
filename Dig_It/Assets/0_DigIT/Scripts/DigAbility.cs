using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DigAbility : MonoBehaviour
{
    // make one collider with an offsett and dimension to be set in the inspector. See weapon in MM for reference
    public GameObject digUp;
    public GameObject digLeft;
    public GameObject digDown;
    public GameObject digRight;

    // Update is called once per frame
    void Update()
    {
        if(this.GetComponent<Player>().CurrentState == PlayerState.Digging)
        {
            switch(this.GetComponent<Player>().CurrFacingDirection)
            {
                case FacingDirection.Up:
                    StartCoroutine(ActivateCollider(digUp));
                    break;

                case FacingDirection.Left:
                    StartCoroutine(ActivateCollider(digLeft));
                    break;
                case FacingDirection.Down:
                    StartCoroutine(ActivateCollider(digDown));
                    break;
                case FacingDirection.Right:
                    StartCoroutine(ActivateCollider(digRight));
                    break;
                default:
                    break;
            }
        }
    }

    IEnumerator ActivateCollider(GameObject digDirection)
    {
        digDirection.SetActive(true);
        yield return new WaitForSeconds(0.05f);
        digDirection.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Block") && collision.GetComponent<Health>() != null)
        {
            collision.GetComponent<Health>().Damage(1, this.gameObject, 0.2f, 0.5f);
        }
    }
}
