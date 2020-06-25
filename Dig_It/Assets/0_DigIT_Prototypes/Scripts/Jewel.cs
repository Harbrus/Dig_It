using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jewel : MonoBehaviour
{
    public int jewelPointValue = 1;
    bool collected = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && !collected)
        {
            CollectAndDestroy();
            GameManager.Instance.AddPoints(jewelPointValue);
        }
    }

    public void CollectAndDestroy()
    {
        collected = true;
        Destroy(gameObject);
    }
}
