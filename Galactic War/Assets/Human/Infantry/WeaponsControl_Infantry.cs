using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponsControl_Infantry : MonoBehaviour
{
    [HideInInspector]
    public Loadout_Infantry loadout;

    void Start()
    {
        loadout = GetComponent<Loadout_Infantry>();
    }

    void Update()
    {
        
    }
}
