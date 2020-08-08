using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : MonoBehaviour
{
    public Transform SpawnPoint = null;
    public GameObject BulletPrefab = null;
    public KeyCode fireButton = KeyCode.Mouse0;
    public float fireRate = 10;
    public float readyTime = 1;

    private Coroutine curCoroutine = null;

    private void Start()
    {
        transform.parent = Camera.main.transform;

        curCoroutine = StartCoroutine(ReadyPistol());

        fireRate = 1 / fireRate;
    }

    private void Update()
    {
        if (curCoroutine == null && Input.GetKeyDown(fireButton))
            curCoroutine = StartCoroutine(FirePistol());
    }

    IEnumerator ReadyPistol()
    {
        yield return new WaitForSeconds(readyTime);

        curCoroutine = null;
    }

    IEnumerator FirePistol()
    {
        GameObject obj = Instantiate(BulletPrefab);
        obj.transform.position = SpawnPoint.position;
        obj.transform.rotation = SpawnPoint.rotation;

        yield return new WaitForSeconds(fireRate);

            curCoroutine = null;
    }
}
