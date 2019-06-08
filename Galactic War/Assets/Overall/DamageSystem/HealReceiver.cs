using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealReceiver : MonoBehaviour
{
    [HideInInspector]
    public float currentHeal = 0f;
    float toRemoveHeal = 0f;
    float toRemoveHealDelay = 1f;

    void Update()
    {
        RemoveHeal();
    }

    void ReceiveHeal(float heal)
    {
        currentHeal += heal;
    }

    void RemoveHeal()
    {
        if (toRemoveHeal >= toRemoveHealDelay)
        {
            currentHeal = 0f;
            toRemoveHeal = 0f;
        } else if (toRemoveHeal < toRemoveHealDelay)
        {
            toRemoveHeal += Time.deltaTime;
        }
    }
}
