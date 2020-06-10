using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingGhost : MonoBehaviour
{
    public GameObject[] cameras;
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
        Initialisation();
    }

    private void Initialisation()
    {
        offset = 1f;

        foreach (GameObject camera in cameras)
        {
            if (camera.activeInHierarchy)
            {
                circleCenter = camera.transform;
            }
        }

        this.transform.position = circleCenter.position;

        angle = UnityEngine.Random.Range(0, 360);
        coordX = radius * Mathf.Cos(Mathf.Deg2Rad * angle);
        coordY = radius * Mathf.Sin(Mathf.Deg2Rad * angle);
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
            coordX = radius * Mathf.Cos(Mathf.Deg2Rad * angle);
            coordY = radius * Mathf.Sin(Mathf.Deg2Rad * angle);
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
