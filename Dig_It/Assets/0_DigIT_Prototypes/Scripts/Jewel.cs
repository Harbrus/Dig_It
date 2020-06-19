using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jewel : MonoBehaviour
{
    public int jewelPointValue = 1;
    bool collected = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player" && !collected)
        {
            Destroy(gameObject);
            collected = true;
            GameManager.Instance.AddPoints(jewelPointValue);
        }
    }
}
