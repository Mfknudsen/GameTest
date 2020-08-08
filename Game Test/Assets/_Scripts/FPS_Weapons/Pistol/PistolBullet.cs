using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistolBullet : MonoBehaviour
{
    public float damage = 20;
    public float dist = 30, speed = 1;
    public LayerMask layerCheck;

    private float timeDelay = 1;

    private void Start()
    {
        timeDelay = dist / speed;

        StartCoroutine(delayDestroy());
    }

    private void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    IEnumerator delayDestroy()
    {
        yield return new WaitForSeconds(timeDelay);

        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (layerCheck == (layerCheck | (1 << other.gameObject.layer)))
        {
            DamageReceiver DR = other.GetComponent<DamageReceiver>();
            if (DR != null)
                DR.ReceiveNewDamage(damage);

            Destroy(gameObject);
        }
    }
}
