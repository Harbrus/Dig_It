using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    [SerializeField] GameObject cameraObj;
    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player" && !cameraObj.activeInHierarchy)
        {
            cameraObj.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            cameraObj.SetActive(false);
        }
    }
}
