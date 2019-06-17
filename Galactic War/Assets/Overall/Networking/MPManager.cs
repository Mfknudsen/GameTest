using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class MPManager : MonoBehaviourPunCallbacks
{
    public GameObject[] EnableObjectsOnConnect;
    public GameObject[] DisableObjectsOnConnect;

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        //PhotonNetwork.ConnectToRegion("au");
    }

    public override void OnConnectedToMaster()
    {
        foreach (GameObject obj in EnableObjectsOnConnect)
        {
            obj.SetActive(true);
        }
        foreach (GameObject obj in DisableObjectsOnConnect)
        {
            obj.SetActive(false);
        }
        //Debug.Log("We are now connected to Photon!");
    }

    public void JoinFreeForAll()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        CreateFreeForAll();
    }

    public void CreateFreeForAll()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        RoomOptions ro = new RoomOptions { MaxPlayers = 10, IsOpen = true, IsVisible = true};
        PhotonNetwork.CreateRoom("defaultFreeForAll", ro, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        SceneManager.LoadScene("FreeForAll");
    }
}
