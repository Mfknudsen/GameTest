using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageReceiver : MonoBehaviour
{
    private float curDamage = 0;
 
    public float CheckDamage()
    {
        float f = curDamage;
        curDamage = 0;
        return f;
    }

    public void ReceiveNewDamage(float newDamage)
    {
        curDamage = newDamage;
    }
}
