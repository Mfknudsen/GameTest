using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageReceiver : MonoBehaviour
{
    [HideInInspector]
    public float currentDamage = 0f;
    float toRemoveDamage = 0f;
    float toRemoveDamageDelay = 1f;

    void Update()
    {
        RemoveDamage();
    }

    void ReceiveDamage(float damage)
    {
        currentDamage += damage;
    }

    void RemoveDamage()
    {
        if (toRemoveDamage >= toRemoveDamageDelay)
        {
            currentDamage = 0;
            toRemoveDamage = 0;
        }
        else if (toRemoveDamage < toRemoveDamageDelay)
        {
            toRemoveDamage += Time.deltaTime;
        }
    }
}
