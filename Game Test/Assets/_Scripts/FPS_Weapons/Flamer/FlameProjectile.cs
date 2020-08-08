using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class FlameProjectile : MonoBehaviour
{
    public float timeDelay = 1;
    public float damage = 20;
    public float speed = 1, minSpeed = 1, maxSpeed = 5;
    public float xMaxDirOffset = 0.2f, yMaxDirOffset = 0.2f;
    public LayerMask layerCheck;
    public float scaleSpeed = 1;
    private Vector3 dir = Vector3.zero, scaleVec = Vector3.one;


    private void Start()
    {
        StartCoroutine(delayDestroy());

        dir = transform.forward + (transform.right * (Random.Range(-xMaxDirOffset, xMaxDirOffset) / 10) + transform.up * (Random.Range(-yMaxDirOffset, yMaxDirOffset) / 10));

        speed = Random.Range(minSpeed, maxSpeed);
    }

    private void Update()
    {
        transform.position += dir * speed * Time.deltaTime;

        transform.localScale += scaleVec * scaleSpeed * Time.deltaTime;
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
