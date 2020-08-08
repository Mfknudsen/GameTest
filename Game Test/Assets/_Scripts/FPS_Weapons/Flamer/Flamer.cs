using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flamer : MonoBehaviour
{
    [Header("Object References")]
    public float readyTime = 1;
    public Transform SpawnPoint = null;
    public KeyCode fireButton = KeyCode.Mouse0;
    public GameObject flameObj = null;
    public float fireRate = 1;
    private Coroutine curCoroutine = null;

    private void Start()
    {
        transform.parent = Camera.main.transform;

        curCoroutine = StartCoroutine(ReadyFlamer());
    }

    private void Update()
    {
        if (Input.GetKeyDown(fireButton) && curCoroutine == null)
        {
            curCoroutine = StartCoroutine(FireFlame());
        }
    }

    IEnumerator ReadyFlamer()
    {
        yield return new WaitForSeconds(readyTime);

        curCoroutine = null;
    }

    IEnumerator FireFlame()
    {
        GameObject obj = Instantiate(flameObj);
        obj.transform.position = SpawnPoint.position;
        obj.transform.rotation = SpawnPoint.rotation;

        yield return new WaitForSeconds(fireRate);

        if (Input.GetKey(fireButton))
            curCoroutine = StartCoroutine(FireFlame());
        else
            curCoroutine = null;
    }
}
