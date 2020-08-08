using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorDetecter : MonoBehaviour
{
    public Elevator Elev = null;

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody newRB = other.GetComponent<Rigidbody>();
        CharacterController newCC = other.GetComponent<CharacterController>();
        if (newRB != null || newCC != null)
        {
            Elev.RB_List.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Rigidbody newRB = other.GetComponent<Rigidbody>();
        CharacterController newCC = other.GetComponent<CharacterController>();
        if (newRB != null || newCC != null)
        {
            Elev.RB_List.Remove(newRB.gameObject);
        }
    }
}
