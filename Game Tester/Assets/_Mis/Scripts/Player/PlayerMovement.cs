using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public float MoveSpeed = 5f;
    public float RotationSpeed = 5f;

    float DirectionX, DirectionY, DirectionZ;
    float RotationDirX;

    Vector3 MoveDirection;
    Vector3 RotationDirection;

    Rigidbody RBody;

    private void Start()
    {
        RBody = GetComponent<Rigidbody>();
    }

    void Update () {
        Movement();

        Rotation();
	}

    void Movement()
    {
        DirectionX = Input.GetAxis("Horizontal");
        DirectionZ = Input.GetAxis("Vertical");

        MoveDirection = new Vector3(DirectionX, 0, DirectionZ);
        transform.Translate(MoveDirection * MoveSpeed * Time.deltaTime);
    }

    void Rotation()
    {
        RotationDirX = Input.GetAxis("Mouse X");

        RotationDirection = new Vector3(0, RotationDirX);
        transform.Rotate(RotationDirection * RotationSpeed * Time.deltaTime);
    }
}
