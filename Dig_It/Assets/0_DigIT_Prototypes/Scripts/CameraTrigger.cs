using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    [SerializeField] GameObject cameraObj;
    public event Action<int> changeOfCamera;
    public int id = 0;

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

            // trigger another action to let the ghost disappear.

            if (changeOfCamera != null)
            {
                changeOfCamera(id);
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
