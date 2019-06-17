using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using UnityEngine.UI;

public class FreeForAll : MonoBehaviourPun, IPunObservable
{
    public GameObject LobbyCam;
    public Text SpawnCountdown;

    public float SpawnTime;
    float timer;
    bool readyToSpawn = false;

    bool HasPlayerSpawned = false;

    void Start()
    {
        LobbyCam.SetActive(true);
    }

    void Update()
    {       
        if (timer >= SpawnTime && !readyToSpawn)
        {
            readyToSpawn = true;
            timer = 0;
        } else if (timer < SpawnTime && !readyToSpawn)
        {
            timer += Time.deltaTime;
        }

        SpawnCountdown.text = "Spawning in: " + (SpawnTime - Mathf.Round(timer));
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {

        }
        else if (stream.IsReading)
        {

        }
    }

    public void SpawnPlayer(string character, Vector3 pos, Quaternion rot)
    {
        if (!HasPlayerSpawned && readyToSpawn)
        {
            LobbyCam.SetActive(false);
            SpawnCountdown.gameObject.SetActive(false);
            PhotonNetwork.Instantiate(character, pos, rot, 0);
            HasPlayerSpawned = true;
            readyToSpawn = false;
        }
    }
}
