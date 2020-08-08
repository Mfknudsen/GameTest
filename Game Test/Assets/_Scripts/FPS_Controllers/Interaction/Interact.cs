using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact : MonoBehaviour
{
    public float dist = 2;
    public LayerMask layer;
    public GameObject Character = null;
    public KeyCode interactButton = KeyCode.E;

    private void Update()
    {
        if (Input.GetKeyDown(interactButton))
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, layer))
            {
                InteractReceiver IR = hit.transform.gameObject.GetComponent<InteractReceiver>();
                if (IR != null)
                {
                    IR.Receive(Character);
                }
            }
        }
        Debug.DrawRay(transform.position, transform.forward * dist, Color.green);
    }
}
