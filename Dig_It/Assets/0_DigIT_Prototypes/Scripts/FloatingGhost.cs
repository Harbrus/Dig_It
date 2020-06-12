using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingGhost : MonoBehaviour
{
    public List<GameObject> triggerableCameras;
    public float angularSpeed;
    public float radius = 1f;
    public float angle = 0;
    public float offset;
    public float restTime = 3f;

    float coordX;
    float coordY;

    private enum FloatingGhostStates { Floating, Attacking, Rest }
    private FloatingGhostStates CurrentFloatingGhostState = FloatingGhostStates.Floating;

    private Vector3 positionInTheCircle;
    private Transform circleCenter;
    private bool canAttack = false;
    private float attackCD = 15f;
    private bool justSpawned = true;

    void Start()
    {
        foreach (GameObject cameraTrigger in ComponentLists.Instance.CameraTriggers)
        {
            cameraTrigger.GetComponent<CameraTrigger>().changeOfCamera += ChangeScreen;
        }

        Initialisation();
    }

    private void Initialisation()
    {
        offset = 1f;

        triggerableCameras.Clear();

        if (triggerableCameras.Count == 0 )
        {
            foreach(GameObject camera in ComponentLists.Instance.TriggerableCamerasInTheScene)
            {
                triggerableCameras.Add(camera);

                if(camera.activeInHierarchy)
                {
                    circleCenter = camera.transform;
                }
            }
        }

        this.transform.position = circleCenter.position;

        angle = UnityEngine.Random.Range(0, 360);
        coordX = circleCenter.position.x + radius * Mathf.Cos(Mathf.Deg2Rad * angle);
        coordY = circleCenter.position.y + radius * Mathf.Sin(Mathf.Deg2Rad * angle);
        positionInTheCircle = new Vector3(coordX, coordY, 0);
        transform.position = positionInTheCircle;

        CurrentFloatingGhostState = FloatingGhostStates.Rest;
    }

    // Update is called once per frame
    void Update()
    {
        if (justSpawned)
        {
            StartCoroutine(RestAfterAttack(restTime));
            // play fill opacity
        }

        if (CurrentFloatingGhostState == FloatingGhostStates.Floating)
        {
            coordX = circleCenter.position.x + radius * Mathf.Cos(Mathf.Deg2Rad * angle);
            coordY = circleCenter.position.y + radius * Mathf.Sin(Mathf.Deg2Rad * angle);
            positionInTheCircle = new Vector3(coordX, coordY, 0);
            angle += Time.deltaTime * angularSpeed;

            CircularMovement();

            if(angle > 360)
            {
                angle = 0;
            }
        }

        if(canAttack)
        {
            CurrentFloatingGhostState = FloatingGhostStates.Attacking;
            AttackPlayer();
        }
    }

    private void ChangeScreen()
    {
        justSpawned = true;
        Initialisation();
    }


    private void AttackPlayer()
    {
        // do stuff
        CurrentFloatingGhostState = FloatingGhostStates.Rest;
        canAttack = false;
        // make low opacity
        StartCoroutine(RestAfterAttack(restTime));
    }

    private IEnumerator RestAfterAttack(float rest)
    {
        // play fill opacity while resting and check to do this just once
        yield return new WaitForSeconds(rest);
        CurrentFloatingGhostState = FloatingGhostStates.Floating;
        
        if(justSpawned)
        {
            justSpawned = false;
        }
    }

    private void CircularMovement()
    {
        transform.position = positionInTheCircle;
    }
}
