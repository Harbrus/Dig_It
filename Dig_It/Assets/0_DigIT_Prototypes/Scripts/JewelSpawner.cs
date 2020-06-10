using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JewelSpawner : MonoBehaviour
{
    public GameObject jewelToSpawn;
    [SerializeField] int numberOfJewel = 0;
    private bool spawn = true;
    // check if the object has jewel

    public void SpawnJewel()
    {
        if(jewelToSpawn != null)
        {
            Instantiate(jewelToSpawn, this.transform.position, Quaternion.identity);
        }
    }

    private void OnDisable()
    {
        if (this.enabled && spawn && numberOfJewel >=0)
        {
            while(numberOfJewel>0)
            {
                SpawnJewel(); // spawn them slightly offset
                numberOfJewel--;
            }
        }
    }

    public void IncreaseJewels()
    {
        numberOfJewel++;
    }

    private void OnApplicationQuit()
    {
        spawn = false;
    }
}
