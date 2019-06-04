using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement_Infantry : MonoBehaviour
{
    Rigidbody rb;

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
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    void FixedUpdate()
    {
        Rotate();
        Move();
        CheckIsGrounded();
    }

    void Move()
    {
        Jump();
        ArtificialGravity();

        inputX = Input.GetAxis("Horizontal");
        inputZ = Input.GetAxis("Vertical");

        Vector3 pos = rb.transform.forward * inputZ + rb.transform.right * inputX;

        if (!isGrounded)
        {
            pos = pos / 2;
        }

        rb.MovePosition(rb.transform.position + pos * 10f * Time.deltaTime + jumpVector * Time.deltaTime);
    }

    void Rotate()
    {
        inputY = Input.GetAxis("Mouse X");

        Vector3 rot = rb.transform.up.normalized * inputY * rotSpeed;

        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + rot * Time.deltaTime);
        rb.MoveRotation(transform.rotation);
    }

    void Jump()
    {
        if (isGrounded && readyToJump && Input.GetAxis("Jump") == 1)
        {
            moveY = jumpForce;
            readyToJump = false;
        }

        jumpVector = rb.transform.up.normalized * moveY;

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

        if (Physics.Raycast(rb.transform.position - rb.transform.up.normalized * 0.85f, -rb.transform.up.normalized, hitDistance, layerMask))
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
            aGravityEffect = -rb.transform.up.normalized * gravity;
        }
        else
        {
            aGravityEffect = Vector3.zero;
        }

        rb.AddForce(aGravityEffect);
    }
}
