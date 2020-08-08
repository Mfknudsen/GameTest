using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunBullet : MonoBehaviour
{
    public float damage = 20;
    public float dist = 30, speed = 1;
    public LayerMask layerCheck;
    public float xMaxDirOffset = 0.2f, yMaxDirOffset = 0.2f;

    private float timeDelay = 1;
    private Vector3 dir = Vector3.zero;

    private void Start()
    {
        timeDelay = dist / speed;
        dir = (transform.forward + transform.right * Random.Range(-xMaxDirOffset, xMaxDirOffset) + transform.up * Random.Range(-yMaxDirOffset, yMaxDirOffset)).normalized;

        StartCoroutine(delayDestroy());
    }

    private void Update()
    {
        transform.position += dir * speed * Time.deltaTime;
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
