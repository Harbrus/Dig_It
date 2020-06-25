using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public bool beingPushed = false;
    public bool BeingPushed { get => beingPushed; set => beingPushed = value; }
    Vector2 currPos;
    Vector2 lastPos;

    private void Update()
    {
        currPos = transform.position;
        
        if (currPos == lastPos)
        {
            BeingPushed = false;
        }
        else
        {
            BeingPushed = true;
        }

        lastPos = currPos;
    }
}
