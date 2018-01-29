using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A scrip i have made to let me spawn prefabs from a targeted location
public class ObjektLife : MonoBehaviour {

    //Create the lists of what is to be Spawned and where
    public Transform[] SpawnLocations;
    public GameObject[] SpawnPrefab;
    public GameObject[] SpawnClone;
    
    //State that the scrip is able to spawn
    bool AbleSpawn = true;

    //Get the position from the Platformen witch is the distination
    public Transform PodPlatform;
    //Get the position of the SpawnLocation which is the DropPod
    public Transform DropPod;

    //The values that determins the distance of witch the Spawn are spawned away from the SpawnLocation
    public float SpawnOffset;
    
    //Values to add in Spawn to make sure that the Clones won't spawn inside the SpawnLocation
    Vector3 DirFront;
    Vector3 DirBack;
    Vector3 DirLeft;
    Vector3 DirRight;

    //Created a number to determin when Spawn can be used
    public float minDistance;
    //A number that determins when the objeckt is destroyed
    public float LifeTime;

    void Update()
    {
        //Calling this avery update of this script
        LandAndSpawn();
    }

    void LandAndSpawn()
    {
        //Value to add to the position, so that the Spawn result don't spawn the same place
        DirFront = new Vector3(0, 2, SpawnOffset);
        DirBack = new Vector3(0, 2, -SpawnOffset);
        DirLeft = new Vector3(SpawnOffset, 2, 0);
        DirRight = new Vector3(-SpawnOffset, 2, 0);

        //Ask if the Pod has reached the Platform
        if (DropPod.position.y < PodPlatform.position.y + minDistance)
        {
            //Ask if it may Spawn something
            if (AbleSpawn == true)
            {
                //Bind the Spawn action to the key R for testing
                if (Input.GetKey(KeyCode.R))
                {
                    //Front
                    SpawnClone[0] = Instantiate(SpawnPrefab[0], SpawnLocations[0].transform.position + DirFront, Quaternion.Euler(0, 0, 0)) as GameObject;
                    //Back
                    SpawnClone[1] = Instantiate(SpawnPrefab[1], SpawnLocations[0].transform.position + DirBack, Quaternion.Euler(0, -180, 0)) as GameObject;
                    //Left
                    SpawnClone[2] = Instantiate(SpawnPrefab[2], SpawnLocations[0].transform.position + DirLeft, Quaternion.Euler(0, 90, 0)) as GameObject;
                    //Right
                    SpawnClone[3] = Instantiate(SpawnPrefab[3], SpawnLocations[0].transform.position + DirRight, Quaternion.Euler(0, -90, 0)) as GameObject;
                    
                    //Remove the DropPod
                    Destroy(gameObject, LifeTime);

                    //State that Spawn is done and must not be done again
                    AbleSpawn = false;
                }
            }
        }
    }
}
