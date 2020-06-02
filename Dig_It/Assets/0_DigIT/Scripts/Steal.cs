using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Steal : MonoBehaviour
{
    public int  stealAmout = 1;
    public float freezeCharacter = 1f;
    public GameObject playerObject;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !playerObject.GetComponent<Health>().Invulnerable)
        {
            if (playerObject.GetComponent<Health>() != null)
            {
                playerObject.GetComponent<Health>().DamageDisabled();

                if(GameManager.Instance.Points > 0)
                {
                    GameManager.Instance.LosePoint(stealAmout);
                    GetComponent<CommonGhost>().StealJewel();
                }
              
                StartCoroutine(playerObject.GetComponent<Health>().DamageEnabled(freezeCharacter));
            }
        }
    }
}
