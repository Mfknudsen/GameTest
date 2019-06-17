using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;

public class Sync : MonoBehaviourPun, IPunObservable
{
    [HideInInspector]
    public Vector3 ObjectPosition;
    [HideInInspector]
    public Quaternion ObjectRotation;
    [HideInInspector]
    public Vector3 ObjectScale;


    public float LerpSpeed = 3f;

    void Start()
    {
        
    }

    void Update()
    {
        if (!photonView.IsMine)
        {
            UpdateTransform();
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(gameObject.transform.position);
            stream.SendNext(gameObject.transform.rotation);
            stream.SendNext(gameObject.transform.localScale);
        }
        else if (stream.IsReading)
        {
            ObjectPosition = (Vector3)stream.ReceiveNext();
            ObjectRotation = (Quaternion)stream.ReceiveNext();
            ObjectScale = (Vector3)stream.ReceiveNext();
        }
    }

    private void UpdateTransform()
    {
        gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, ObjectPosition, LerpSpeed * Time.deltaTime);
        gameObject.transform.rotation = Quaternion.Lerp(gameObject.transform.rotation, ObjectRotation, LerpSpeed * Time.deltaTime);
        gameObject.transform.localScale = Vector3.Lerp(gameObject.transform.localScale, ObjectScale, LerpSpeed * Time.deltaTime);
    }
}
