using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DigAbility : MonoBehaviour
{
    // make one collider with an offsett and dimension to be set in the inspector. See weapon in MM for reference
    public GameObject weapon;
    public GameObject holder;
    public GameObject colliderAndVisual;
    bool digging = false;

    public IEnumerator ActivateCollider()
    {
        weapon.SetActive(true);
        colliderAndVisual.SetActive(true);

        switch (this.GetComponent<Player>().CurrFacingDirection)
        {
            case FacingDirection.Up:
                holder.transform.rotation = Quaternion.Euler(0, 0, 180f);
                break;
            case FacingDirection.Left:
                holder.transform.rotation = Quaternion.Euler(0, 0, -90f);
                break;
            case FacingDirection.Down:
                holder.transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case FacingDirection.Right:
                holder.transform.rotation = Quaternion.Euler(0, 0, 90f);
                break;
            default:
                break;
        }
        
        yield return new WaitForSeconds(0.03f);
        weapon.SetActive(false);
        colliderAndVisual.SetActive(false);
        holder.transform.rotation = Quaternion.Euler(0, 0, 0);
        digging = false;
        this.GetComponent<Player>().CurrentState = PlayerState.Idle;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.IsTouching(colliderAndVisual.GetComponent<BoxCollider2D>()) && collision.CompareTag("Block") && collision.GetComponent<Health>() != null && !digging)
        {
            digging = true;
            collision.GetComponent<Health>().Damage(1, 0.2f, 0.5f);
        }
    }
}
