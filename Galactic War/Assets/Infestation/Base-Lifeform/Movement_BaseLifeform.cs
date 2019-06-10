using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement_BaseLifeform : MonoBehaviour
{
    [HideInInspector]
    public Rigidbody RB;

    float inputX, inputY, inputZ;
    public float moveSpeed = 15f;
    public float rotSpeed = 20;

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
    Vector3 aNormalVector = Vector3.zero;

    Vector3 normalVector = Vector3.zero;

    public float checkDist = 0.5f;
    int normalDiv = 0;
    Vector3 vect = Vector3.zero;
    Vector3 offset = Vector3.zero;

    void Start()
    {
        RB = GetComponent<Rigidbody>();
        RB.useGravity = false;
        normalVector = RB.transform.up;
    }

    void FixedUpdate()
    {
        CheckIsGrounded();
        NewNormalDetect();

        ArtificialGravity();
        Rotate();
        Move();
    }

    void Move()
    {
        Jump();
        
        inputX = Input.GetAxis("Horizontal");
        inputZ = Input.GetAxis("Vertical");

        Vector3 newPositionVector = RB.transform.forward * inputZ + RB.transform.right * inputX;

        if (!isGrounded)
        {
            newPositionVector = newPositionVector / 2;
        }

        RB.MovePosition(RB.transform.position + newPositionVector * 10f * Time.deltaTime + jumpVector * Time.deltaTime);
    }

    void Rotate()
    {
        RotateToNewNormal();

        inputY = Input.GetAxis("Mouse X");

        Vector3 newRotationVector = transform.InverseTransformDirection(RB.transform.up) * inputY * rotSpeed;
        RB.MoveRotation(RB.transform.localRotation * Quaternion.Euler(newRotationVector * Time.deltaTime));
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
        if (moveY <= 0)
        {
            aGravityEffect = -aNormalVector.normalized * gravity;
        }
        else
        {
            aGravityEffect = Vector3.zero;
        }

        RB.AddForce(aGravityEffect);
    }

    void NewNormalDetect()
    {
        normalDiv = 0;
        vect = Vector3.zero;
        offset = Vector3.zero;

        RaycastHit hit;
        for (var i = 0; i < 8; i++)
        {
            if (i == 0)
            {
                offset = RB.transform.forward;
            }
            else if (i == 1)
            {
                offset = RB.transform.forward + RB.transform.right;
            }
            else if (i == 2)
            {
                offset = RB.transform.right;
            }
            else if (i == 3)
            {
                offset = -RB.transform.forward + RB.transform.right;
            }
            else if (i == 4)
            {
                offset = -RB.transform.forward;
            }
            else if (i == 5)
            {
                offset = -RB.transform.forward - RB.transform.right;
            }
            else if (i == 6)
            {
                offset = -RB.transform.right;
            }
            else if (i == 7)
            {
                offset = RB.transform.forward - RB.transform.right;
            }

            offset = offset.normalized;

            Debug.DrawRay(RB.transform.position - RB.transform.up * 0.5f + offset * checkDist, -RB.transform.up * 0.5f, Color.blue);
            if (Physics.Raycast(RB.transform.position - RB.transform.up * 0.5f + offset * checkDist, -RB.transform.up, out hit, 1, layerMask))
            {
                if (hit.transform.gameObject.tag == "Crawlable")
                {
                    vect += hit.normal;
                    normalDiv++;
                }
            }

            Debug.DrawRay(RB.transform.position - RB.transform.up * 0.5f, offset * 0.5f, Color.blue);
            if (Physics.Raycast(RB.transform.position - RB.transform.up * 0.5f, offset, out hit, 1, layerMask))
            {
                if (hit.transform.gameObject.tag == "Crawlable")
                {
                    vect += hit.normal;
                    normalDiv++;
                }
            }
        }

        Debug.DrawRay(RB.transform.position - RB.transform.up * 0.5f, -RB.transform.up * 0.5f, Color.blue);
        if (Physics.Raycast(RB.transform.position, -RB.transform.up * 0.5f, out hit, 1, layerMask))
        {
            if (hit.transform.gameObject.tag == "Crawlable")
            {
                vect += hit.normal;
                normalDiv++;
                aNormalVector = hit.normal;
            }
        }

        if (vect == Vector3.zero)
        {
            vect = RB.transform.up;
            normalDiv = 1;
        }

        vect = vect / normalDiv;
    }

    void RotateToNewNormal()
    {
        Vector3 tempVec = Vector3.RotateTowards(RB.transform.up, vect, 1, 0.0f);

        RB.transform.localRotation = Quaternion.FromToRotation(RB.transform.up, tempVec) * RB.rotation;
    }
}
