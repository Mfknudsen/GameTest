using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : MonoBehaviour
{
    public Transform SpawnPoint = null;
    public GameObject BulletPrefab = null;
    public KeyCode fireButton = KeyCode.Mouse0;
    public int bulletCount = 10;
    public float fireRate = 10, readyTime = 1;

    private Coroutine curCoroutine = null;

    private void Start()
    {
        transform.parent = Camera.main.transform;

        curCoroutine = StartCoroutine(ReadyShotgun());

        fireRate = 1 / fireRate;
    }

    private void Update()
    {
        if (curCoroutine == null && Input.GetKeyDown(fireButton))
            curCoroutine = StartCoroutine(FireShotgun());
    }

    IEnumerator ReadyShotgun()
    {
        yield return new WaitForSeconds(readyTime);

        curCoroutine = null;
    }

    IEnumerator FireShotgun()
    {
        for (int i = 0; i < bulletCount; i++)
        {
            GameObject obj = Instantiate(BulletPrefab);
            obj.transform.position = SpawnPoint.position;
            obj.transform.rotation = SpawnPoint.rotation;
        }

        yield return new WaitForSeconds(fireRate);

        if (Input.GetKey(fireButton))
            curCoroutine = StartCoroutine(FireShotgun());
        else
            curCoroutine = null;
    }
}
