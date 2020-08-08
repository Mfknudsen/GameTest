using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    [Header("Object References")]
    public Transform Platform = null;
    public Transform StartPoint = null, EndPoint = null;
    public KeyCode interactButton = KeyCode.Alpha1;
    [HideInInspector] public List<GameObject> RB_List = new List<GameObject>();

    [Header("Platform")]
    public bool activate = false;
    private bool running = false;

    [Header("Movement")]
    public float MoveSpeed = 1;
    private Vector3 dir = Vector3.zero, curTarget = Vector3.zero, lastTarget = Vector3.zero;
    private float dist = 0;

    private void Start()
    {
        EndPoint.position = new Vector3(StartPoint.position.x, EndPoint.position.y, StartPoint.position.z);

        lastTarget = EndPoint.position;
        curTarget = StartPoint.position;

        dist = Vector3.Distance(curTarget, lastTarget);
        dir = (StartPoint.position - EndPoint.position).normalized;
    }

    private void Update()
    {
        if (activate && !running)
        {
            Vector3 temp = curTarget;
            curTarget = lastTarget;
            lastTarget = temp;

            dir *= -1;

            running = true;
            activate = false;
        }
        else if (activate && running)
        {
            activate = false;
        }

        if (RB_List.Count != 0 && Input.GetKeyDown(interactButton))
            activate = true;

        if (running)
        {
            Platform.position += dir * MoveSpeed * Time.fixedDeltaTime;
            foreach (GameObject obj in RB_List)
            {
                obj.transform.position += dir * MoveSpeed * Time.fixedDeltaTime;
            }

            if (Vector3.Distance(lastTarget, Platform.position) > dist)
            {
                Vector3 toMove = curTarget - Platform.position;
                Platform.position += toMove;

                foreach (GameObject obj in RB_List)
                {
                    obj.transform.position += toMove;
                }

                running = false;
            }
        }
    }
}
