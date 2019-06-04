﻿using System.Collections;
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

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Rotate();
        Move();
        Jump();
        CheckIsGrounded();
        if (moveY <= 0)
        {
            ArtificialGravity();
        }
    }

    void Move()
    {
        inputX = Input.GetAxis("Horizontal");
        inputZ = Input.GetAxis("Vertical");

        Vector3 pos = rb.transform.forward * inputZ * 100f + rb.transform.right * inputX * 100f;

        rb.MovePosition(rb.transform.position + pos * Time.deltaTime);
    }

    void Rotate()
    {
        inputY = Input.GetAxis("Mouse X");

        Vector3 rot = new Vector3(0, inputY * rotSpeed, 0);

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

        if (moveY > 0) {
            moveY -= gravity * Time.deltaTime;
        } else if (moveY <= 0)
        {
            moveY = 0;
        }

        Vector3 jumpVector = -rb.transform.up.normalized * moveY;

        rb.MovePosition(rb.transform.position + jumpVector * Time.deltaTime);

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

        if (Physics.Raycast(rb.transform.position - rb.transform.up * 0.85f, -rb.transform.up, hitDistance, layerMask))
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
        Vector3 aGravityEffect = -rb.transform.up * gravity;

        rb.MovePosition(rb.transform.position + aGravityEffect * Time.deltaTime);
    }
}
