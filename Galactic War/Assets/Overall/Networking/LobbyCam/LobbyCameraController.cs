using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon;
using Photon.Pun;

public class LobbyCameraController : MonoBehaviourPun
{
    public int CurrentTeam = 0;
    public int CurrentActionScreen = 0;

    public GameObject BackgroundColor;

    public GameObject MainScreen;
    public GameObject ChooseTeamScreen;
    public GameObject ChooseCharacterScreen;
    public GameObject SpawningPlayer;

    public GameObject BackToMainButtom;

    public FreeForAll FFA;

    void Start()
    {
        if (photonView.IsMine)
        {
            foreach (GameObject o in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                if (o.name == "GameManager")
                {
                    FFA = o.GetComponent<FreeForAll>();
                }
            }

            if (CurrentTeam == 0)
            {
                BackgroundColor.SetActive(true);
                MainScreen.SetActive(false);
                ChooseTeamScreen.SetActive(true);
                ChooseCharacterScreen.SetActive(false);
                SpawningPlayer.SetActive(false);
                BackToMainButtom.SetActive(false);
            }
            else
            {
                BackgroundColor.SetActive(true);
                MainScreen.SetActive(true);
                ChooseTeamScreen.SetActive(false);
                ChooseCharacterScreen.SetActive(false);
                SpawningPlayer.SetActive(false);
            }
        }
    }

    public void ChooseTeam(int teamNumber)
    {
        if (photonView.IsMine)
        {
            CurrentTeam = teamNumber;
        }
    }

    public void SwithToMainScreen()
    {
        MainScreen.SetActive(true);
        ChooseTeamScreen.SetActive(false);
        ChooseCharacterScreen.SetActive(false);
        SpawningPlayer.SetActive(false);
    }

    public void SwithToTeamSelector()
    {
        MainScreen.SetActive(false);
        ChooseTeamScreen.SetActive(true);
        ChooseCharacterScreen.SetActive(false);
        SpawningPlayer.SetActive(false);
        BackToMainButtom.SetActive(true);
    }

    public void SwithToCharacterSelector()
    {
        MainScreen.SetActive(false);
        ChooseTeamScreen.SetActive(false);
        ChooseCharacterScreen.SetActive(true);
        SpawningPlayer.SetActive(false);
    }

    public void SwithToSpawnPlayer()
    {
        MainScreen.SetActive(false);
        ChooseTeamScreen.SetActive(false);
        ChooseCharacterScreen.SetActive(false);
        SpawningPlayer.SetActive(true);
    }

    public void SpawnPlayerWithSelectedCharacter(string characterPrefabName)
    {
        FFA.SpawnPlayer(characterPrefabName, Vector3.zero, Quaternion.identity);
        //SwithToSpawnPlayer();
    }
}
