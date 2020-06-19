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
        var offsetSpawn = Random.Range(-0.2f, 0.2f);
        Vector3 offsetSpawnPosition = new Vector2(offsetSpawn, offsetSpawn);

        if(jewelToSpawn != null)
        {
            Instantiate(jewelToSpawn, (this.transform.position + offsetSpawnPosition), Quaternion.identity);
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

    public void IncreaseJewels(int increase)
    {
        numberOfJewel += increase;
    }

    private void OnApplicationQuit()
    {
        spawn = false;
    }
}
