using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JewelSpawner : MonoBehaviour
{
    public GameObject jewelToSpawn;
    private bool doNotSpawn = false;
    public void SpawnJewel()
    {
        if(jewelToSpawn != null)
        {
            Instantiate(jewelToSpawn, this.transform.position, Quaternion.identity);
        }
    }

    private void OnDisable()
    {
        if (this.enabled && !doNotSpawn)
        {
            SpawnJewel(); // consider spawn multiple times
        }
    }

    private void OnApplicationQuit()
    {
        doNotSpawn = true;
    }
}
