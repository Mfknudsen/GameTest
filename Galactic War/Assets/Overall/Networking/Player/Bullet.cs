using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int DamageAmount = 50;

    void Start()
    {
        gameObject.GetComponent<Rigidbody>().velocity = gameObject.transform.forward * 10;
    }

    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Player")
        {
            PlayerData PD = collision.gameObject.GetComponent<PlayerData>();
            PD.TakeDamage(DamageAmount);
        }
        DestroyBullet();
    }

    void DestroyBullet()
    {
        Destroy(gameObject);
    }
}
