using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    [SerializeField] GameObject cameraObj;
    public event Action changeOfCamera;

    private void Start()
    {
        ComponentLists.Instance.TriggerableCamerasInTheScene.Add(cameraObj);
        ComponentLists.Instance.CameraTriggers.Add(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player" && !cameraObj.activeInHierarchy)
        {
            cameraObj.SetActive(true);
            
            if(changeOfCamera!=null)
            {
                changeOfCamera();
            }
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
