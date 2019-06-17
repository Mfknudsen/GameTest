using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;

public class Movement : MonoBehaviourPun
{
    public KeyCode Left, Right, Forward, Backward;

    [SerializeField]
    private float moveSpeed = 2.5f;
    public float shiftSpeedMultiplier = 2f;

    private Rigidbody RB;
    private Animator anim;
    public GameObject Cam;
    public Transform Head;
    public Transform HeadLock;
    public Vector3 HeadOffset;

    public Transform GunHoldOne;

    public float AnimationTransisionSpeed = 3f;

    GunManager gm;

    private float inputX, inputY, inputZ;

    public float rotSpeed = 20f;

    public bool isGrounded = true;
    public float hitDistance;
    public LayerMask layerMask;

    public float jumpForce = 50f;
    bool readyToJump = false;
    public float jumpDelay = 30f;
    private float jumpTimer = 0f;
    private float moveY = 0f;

    public float gravity = 9.81f;

    Vector3 jumpVector = Vector3.zero;
    Vector3 aGravityEffect = Vector3.zero;

    void Start()
    {
        RB = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        gm = GetComponent<GunManager>();

        if (photonView.IsMine)
        {
            Cam.SetActive(true);
        }
        else
        {
            Cam.SetActive(false);
        }
    }

    private void OnAnimatorIK(int layerIndex)
    {
        anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);
        anim.SetIKPosition(AvatarIKGoal.RightHand, GunHoldOne.transform.position);
    }

    private void LateUpdate()
    {
        Quaternion rot = Quaternion.Euler(Cam.transform.eulerAngles.x, HeadLock.transform.eulerAngles.y, HeadLock.transform.eulerAngles.z) * Quaternion.Euler(HeadOffset); ;
        Head.transform.rotation = rot;
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            CheckIsGrounded();
        }
    }

    void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            Move();
            Rotate();
        }
    }

    void Test()
    {
        float x = Input.GetAxis("Mouse X");
        float y = Input.GetAxis("Mouse Y");

        if (Input.GetKey(Left))
        {
            anim.SetFloat("Move", Mathf.Lerp(anim.GetFloat("Move"), 0.5f, AnimationTransisionSpeed * Time.deltaTime));
            RB.AddRelativeForce(Vector3.left * moveSpeed * Time.deltaTime, ForceMode.Impulse);
        }
        else if (Input.GetKeyUp(Left))
        {
            anim.SetFloat("Move", 0);
        }

        if (Input.GetKey(Right))
        {
            anim.SetFloat("Move", Mathf.Lerp(anim.GetFloat("Move"), 0.5f, AnimationTransisionSpeed * Time.deltaTime));
            RB.AddRelativeForce(Vector3.left * -moveSpeed * Time.deltaTime, ForceMode.Impulse);
        }
        else if (Input.GetKeyUp(Right))
        {
            anim.SetFloat("Move", 0);
        }

        if (Input.GetKey(Forward))
        {
            anim.SetFloat("Move", Mathf.Lerp(anim.GetFloat("Move"), 0.5f, AnimationTransisionSpeed * Time.deltaTime));
            RB.AddRelativeForce(Vector3.forward * moveSpeed * Time.deltaTime, ForceMode.Impulse);
        }
        else if (Input.GetKeyUp(Forward))
        {
            anim.SetFloat("Move", 0);
        }

        if (Input.GetKey(Backward))
        {
            anim.SetFloat("Move", Mathf.Lerp(anim.GetFloat("Move"), 0.5f, AnimationTransisionSpeed * Time.deltaTime));
            RB.AddRelativeForce(Vector3.forward * -moveSpeed * Time.deltaTime, ForceMode.Impulse);
        }
        else if (Input.GetKeyUp(Backward))
        {
            anim.SetFloat("Move", 0);
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            anim.SetFloat("Move", Mathf.Lerp(anim.GetFloat("Move"), 1f, AnimationTransisionSpeed * Time.deltaTime));
            RB.AddRelativeForce(Vector3.forward * (moveSpeed + 10) * Time.deltaTime, ForceMode.Impulse);
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            anim.SetFloat("Move", 0);
        }

        gameObject.transform.Rotate(new Vector3(0, x, 0));
        Cam.transform.Rotate(new Vector3(-y, 0, 0));
        gm.HoldPosition.transform.Rotate(new Vector3(-y, 0, 0));
    }

    void Move()
    {
        ArtificialGravity();
        Jump();

        if (Input.GetKey(KeyCode.LeftShift))
        {
            inputX = Input.GetAxis("Horizontal") * 2f;
            inputZ = Input.GetAxis("Vertical") * 2f;
        }
        else
        {
            inputX = Input.GetAxis("Horizontal");
            inputZ = Input.GetAxis("Vertical");
        }

        Vector3 newPositionVector = RB.transform.forward * inputZ * moveSpeed + RB.transform.right * inputX * moveSpeed;

        RB.MovePosition(RB.transform.position + newPositionVector * Time.deltaTime + jumpVector * Time.deltaTime);
    }

    void Rotate()
    {
        inputY = Input.GetAxis("Mouse X");

        Vector3 newRotationVector = RB.transform.up * inputY * rotSpeed;

        RB.MoveRotation(Quaternion.Euler(RB.transform.rotation.eulerAngles + newRotationVector * Time.deltaTime));


        float y = Input.GetAxis("Mouse Y");
        Cam.transform.Rotate(new Vector3(-y, 0, 0));
        gm.HoldPosition.transform.Rotate(new Vector3(-y, 0, 0));
    }

    void Jump()
    {
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

        if (isGrounded && readyToJump && Input.GetAxis("Jump") == 1)
        {
            moveY = jumpForce;
            readyToJump = false;
        }

        jumpVector = RB.transform.up * moveY;
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

        if (Physics.Raycast(RB.transform.position + RB.transform.up * 0.10f, -RB.transform.up, hitDistance, layerMask))
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
            aGravityEffect = -RB.transform.up * gravity;
            moveY = 0;
        }
        else
        {
            aGravityEffect = Vector3.zero;
        }

        RB.AddRelativeForce(aGravityEffect, ForceMode.Force);

        moveY -= gravity * Time.deltaTime;
    }
}
