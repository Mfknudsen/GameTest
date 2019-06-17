using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;

public class GunManager : MonoBehaviourPun
{
    public GameObject HoldPosition;
    public GameObject Righthand;
    public GameObject CurrentOwnedWeapon;
    GameObject cam;

    void Start()
    {
        cam = gameObject.GetComponent<Movement>().Cam;
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (Physics.Raycast(ray, out hit, 3f))
                {
                    if (hit.collider.tag == "Weapon")
                    {
                        //PhotonNetwork.Destroy(hit.collider.GetComponent<PhotonView>());
                        photonView.RPC("ClaimWeapon", RpcTarget.AllBuffered, hit.collider.GetComponent<PhotonView>().ViewID);
                    }
                }
            }

            if (CurrentOwnedWeapon != null)
            {
                RaycastHit hitBullet;
                if (Input.GetMouseButtonDown(0))
                {
                    if (Physics.Raycast(ray, out hitBullet))
                    {
                        //Vector3 dir = hitBullet.point - gameObject.transform.position;
                        CurrentOwnedWeapon.GetComponent<Weapon>().Shoot(hitBullet.point);
                    }
                }
            }
        }

        Righthand.transform.position = HoldPosition.transform.position;
    }

    [PunRPC]
    void ClaimWeapon(int id)
    {
        GameObject gun = PhotonView.Find(id).gameObject;
        gun.gameObject.transform.SetParent(HoldPosition.transform);
        gun.transform.position = HoldPosition.transform.position;
        gun.transform.rotation = HoldPosition.transform.rotation;
        CurrentOwnedWeapon = gun;
    }

}
