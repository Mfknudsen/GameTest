using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    Vector3 Offset;
    public float EkstraOffset;
    public Transform[] SpawnLocations;
    public GameObject[] SpawnPrefab;
    public GameObject[] SpawnClone;

    bool AbleSpawn = true;
	

	void Update () {

        Offset = new Vector3(0, EkstraOffset,0);

        if (AbleSpawn == true)
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                SpawnSomething();

                AbleSpawn = false;
            }
        }
        if (AbleSpawn == false)
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                AbleSpawn = true;
            }
        }
	}

    void SpawnSomething()
    {
        SpawnClone[0] = Instantiate(SpawnPrefab[0], SpawnLocations[0].transform.position + Offset, Quaternion.Euler(0, 0, 0)) as GameObject;
    }
}