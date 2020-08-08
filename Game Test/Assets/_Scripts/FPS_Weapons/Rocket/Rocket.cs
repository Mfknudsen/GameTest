using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    public float damage = 20;
    public float dist = 30, speed = 1;
    public LayerMask layerCheck;
    public float exploRad = 1;

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

    public void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, exploRad, layerCheck);
        foreach(Collider hit in colliders)
        {
            DamageReceiver DR = hit.GetComponent<DamageReceiver>();
            if (DR != null)
                DR.ReceiveNewDamage(damage);
        }

        Destroy(gameObject);
    }

    IEnumerator delayDestroy()
    {
        yield return new WaitForSeconds(timeDelay);

        Explode();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (layerCheck == (layerCheck | (1 << other.gameObject.layer)))
        {
            DamageReceiver DR = other.GetComponent<DamageReceiver>();
            if (DR != null)
                DR.ReceiveNewDamage(damage);

            Explode();
        }
    }
}
