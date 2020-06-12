using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject floatingGhostPrefab;

    private void Awake()
    {
        LevelManager.Instance.timerEvent += SpawnFloatingGhost;
        LevelManager.Instance.jewelCountEvent += SpawnFloatingGhost;
    }

    public void SpawnFloatingGhost()
    {
        GameObject floatingGhost = Instantiate(floatingGhostPrefab, transform.position, Quaternion.identity);
        
        if(LevelManager.Instance.jewelCountEvenFired)
        {
            LevelManager.Instance.jewelCountEvent -= SpawnFloatingGhost;
        }
        else if(LevelManager.Instance.timerEventFired)
        {
            LevelManager.Instance.timerEvent -= SpawnFloatingGhost;
        }
    }
}
