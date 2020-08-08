using CC_Controller;
using RB_Controller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropPod : MonoBehaviour
{
    [Header("Debug System")]
    public bool DebugSystem = false;
    public Transform DevPoint = null;

    [Header("Object Reference")]
    public InteractReceiver IR = null;
    public GameObject Player = null;
    public Transform PlayerSeat = null;

    [Header("Movement")]
    public float safetyDist = 20, landingDelay = 60, grav = -9.85f;

    [HideInInspector] public bool loading = false;
    private bool active = false, beyondSafe = false, landed = false;
    private int State = 0;
    private float speed = 1;
    private Vector3 dir = Vector3.zero, rotDir = Vector3.up;
    private Vector3 startPoint = Vector3.zero, middelPoint = Vector3.zero, endPoint = Vector3.zero;

    private void Start()
    {
        startPoint = transform.position;

        middelPoint = startPoint - transform.up * safetyDist;

        setLandingSpot(DevPoint.position);
    }

    private void Update()
    {
        switch (State)
        {
            case 0:
                Idle();
                if (active || Input.GetKeyDown(KeyCode.Alpha1))
                    State = 1;
                break;

            case 1:
                ClearingCarrier();
                if (beyondSafe)
                    State = 2;
                break;

            case 2:
                MoveToLandingSpot();
                if (landed)
                {
                    StartCoroutine(DelayDestoy());
                    State = 3;
                }
                break;
        }

        Debug.DrawRay(startPoint, middelPoint - startPoint, Color.red);
        Debug.DrawRay(middelPoint, endPoint - middelPoint, Color.red);
    }

    public void setLandingSpot(Vector3 newPos)
    {
        endPoint = newPos;

        dir = endPoint - middelPoint;
        dir.y = 0;
        speed = ((endPoint.y - middelPoint.y) / grav);

        speed = dir.magnitude / speed;
        dir = dir.normalized;

        rotDir = (endPoint - middelPoint).normalized;
    }

    private void Idle()
    {
        GameObject[] checkObj = IR.CheckState(1);
        if (checkObj != null)
        {
            if (checkObj[0].tag == "Player")
                Player = checkObj[0];
        }

        if (Player != null)
            active = true;

        if (active)
        {
            RigidbodyMovement RBM = Player.GetComponent<RigidbodyMovement>();
            CharacterControllerMovement CCM = Player.GetComponent<CharacterControllerMovement>();
            if (RBM != null)
            {
                RBM.enabled = false;
                Destroy(RBM.gameObject.GetComponent<Rigidbody>());
            }
            else if (CCM != null)
            {
                CCM.enabled = false;
                CCM.gameObject.GetComponent<CharacterController>().enabled = false;
            }

            Player.transform.position = PlayerSeat.position;
            Player.transform.rotation = PlayerSeat.rotation;
        }
    }

    private void ClearingCarrier()
    {
        transform.position += transform.up * grav * Time.deltaTime;

        if (Vector3.Distance(transform.position, middelPoint) < 0.5f)
            beyondSafe = true;

        if (Player != null)
        {
            Player.transform.position = PlayerSeat.position;
            Player.transform.rotation = PlayerSeat.rotation;
        }
    }

    private void MoveToLandingSpot()
    {
        transform.rotation = Quaternion.FromToRotation(transform.up, -rotDir) * transform.rotation;

        if (!loading)
        {
            transform.position += (Vector3.up * grav) * Time.deltaTime + (dir * speed) * Time.deltaTime;

            if (Vector3.Distance(transform.position, endPoint) < 0.5f)
                landed = true;
        }

        if (Player != null)
        {

            Player.transform.position = PlayerSeat.position;
            Player.transform.rotation = PlayerSeat.rotation;
        }
    }

    private IEnumerator DelayDestoy()
    {
        yield return new WaitForSeconds(landingDelay);

        Destroy(gameObject);
    }
}
