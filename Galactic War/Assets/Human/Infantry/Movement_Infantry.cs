using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement_Infantry : MonoBehaviour
{
    [HideInInspector]
    public Health_Infantry HI;

    [HideInInspector]
    public Rigidbody RB;

    float inputX, inputY, inputZ;
    public float moveSpeed = 15f;

    public float rotSpeed = 20f;

    public bool isGrounded = true;
    public float hitDistance;
    public LayerMask layerMask;

    public float jumpForce = 50f;
    bool readyToJump = false;
    public float jumpDelay = 30f;
    float jumpTimer = 0f;
    float moveY = 0f;

    public float gravity = 9.81f;

    Vector3 jumpVector = Vector3.zero;
    Vector3 aGravityEffect = Vector3.zero;

    void Start()
    {
        HI = GetComponent<Health_Infantry>();
        RB = GetComponent<Rigidbody>();
        RB.useGravity = false;
    }

    void Update()
    {
        CheckIsGrounded();
    }

    void FixedUpdate()
    {
        Rotate();
        Move();
    }

    void Move()
    {
        Jump();
        ArtificialGravity();

        inputX = Input.GetAxis("Horizontal");
        inputZ = Input.GetAxis("Vertical");

        Vector3 newPositionVector = RB.transform.forward * inputZ + RB.transform.right * inputX;

        if (!isGrounded)
        {
            newPositionVector = newPositionVector / 2;
        }

        if (!HI.isDead)
        {
            RB.MovePosition(RB.transform.position + newPositionVector * 10f * Time.deltaTime + jumpVector * Time.deltaTime);
        }
        else
        {
            moveY = 0f;
        }
    }

    void Rotate()
    {
        inputY = Input.GetAxis("Mouse X");

        Vector3 newRotationVector = RB.transform.up * inputY * rotSpeed;

        RB.MoveRotation(Quaternion.Euler(RB.transform.rotation.eulerAngles + newRotationVector * Time.deltaTime));
    }

    void Jump()
    {
        if (isGrounded && readyToJump && Input.GetAxis("Jump") == 1)
        {
            moveY = jumpForce;
            readyToJump = false;
        }

        jumpVector = RB.transform.up * moveY;

        moveY -= gravity * Time.deltaTime;

        if (moveY <= 0)
        {
            moveY = 0;
        }

        if (!readyToJump)
        {
            if (jumpTimer >= jumpDelay)
            {
                readyToJump = true;
                jumpTimer = 0;
            }
            else
            {
                jumpTimer++;
            }
        }
    }

    void CheckIsGrounded()
    {
        if (isGrounded)
        {
            hitDistance = 0.35f;
        }
        else
        {
            hitDistance = 0.15f;
        }

        if (Physics.Raycast(RB.transform.position - RB.transform.up * 0.90f, -RB.transform.up, hitDistance, layerMask))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    void ArtificialGravity()
    {
        if (moveY <= 0) {
            aGravityEffect = -RB.transform.up * gravity;
        }
        else
        {
            aGravityEffect = Vector3.zero;
        }

        RB.AddForce(aGravityEffect);
    }
}
