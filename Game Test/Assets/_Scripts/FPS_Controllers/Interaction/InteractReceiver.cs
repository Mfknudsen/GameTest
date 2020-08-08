using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractReceiver : MonoBehaviour
{
    private List<GameObject> objects = new List<GameObject>();

    public GameObject[] CheckState(int count)
    {
        if ((count <= objects.Count && count > 0) || objects.Count != 0)
        {
            List<GameObject> toSend = new List<GameObject>();

            for (int i = 0; i < count; i++)
                toSend.Add(objects[i]);

            return toSend.ToArray();
        }
        else
            return null;
    }

    public void Receive(GameObject obj)
    {
        objects.Add(obj);
    }
}
