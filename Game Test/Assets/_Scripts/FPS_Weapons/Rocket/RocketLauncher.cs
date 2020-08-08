using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketLauncher : MonoBehaviour
{
    public Transform SpawnPoint = null;
    public GameObject RocketPrefab = null;
    public KeyCode fireButton = KeyCode.Mouse0;
    public float fireRate = 10;
    public float readyTime = 1;

    private Coroutine curCoroutine = null;

    private void Start()
    {
        transform.parent = Camera.main.transform;

        curCoroutine = StartCoroutine(ReadyRocketLauncher());

        fireRate = 1 / fireRate;
    }

    private void Update()
    {
        if (curCoroutine == null && Input.GetKeyDown(fireButton))
            curCoroutine = StartCoroutine(FireRocketLauncher());
    }

    IEnumerator ReadyRocketLauncher()
    {
        yield return new WaitForSeconds(readyTime);

        curCoroutine = null;
    }

    IEnumerator FireRocketLauncher()
    {
        GameObject obj = Instantiate(RocketPrefab);
        obj.transform.position = SpawnPoint.position;
        obj.transform.rotation = SpawnPoint.rotation;

        yield return new WaitForSeconds(fireRate);

        curCoroutine = null;
    }
}
