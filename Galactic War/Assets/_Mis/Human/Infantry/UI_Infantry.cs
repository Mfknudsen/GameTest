using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Infantry : MonoBehaviour
{
    [HideInInspector]
    public Health_Infantry HealthInfantry;

    void Start()
    {
        HealthInfantry = GetComponent<Health_Infantry>();
    }

    void Update()
    {
        
    }
}
