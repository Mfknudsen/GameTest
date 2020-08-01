using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.AI;

namespace RB_Controller
{
    public enum State { Grounded, Airborne, Dashing, AirDash, BarJump }
    public class RigidbodyMovement : MonoBehaviour
    {
        public State State;

        [Header("Debug")]
        public bool DebugSystem = false;

        [Header("Object Reference")]
        public Rigidbody RB = null;
        public GameObject Cam = null;

        [Header("Movement")]
        public float MoveSpeed = 1.0f;
        private float InputX = 0, InputZ = 0;
        private Vector3 TargetPos = Vector3.zero;

        [Header("Rotation")]
        public float RotSpeed = 1.0f;
        private float RotX = 0, RotY = 0;
        private float xRot = 0;

        [Header("Jump")]
        public float jumpHeight = 3f;
        private bool hasDoubleJumped = false;

        [Header("Jetpack")]
        public float fuelCount = 100;
        public float jetConsume = 5;

        [Header("Gravity")]
        public bool localGravity = false;
        public float gravity = -9.81f;
        private Vector3 velocity = Vector3.zero;
        private bool isGrounded = false;

        [Header("Ground Check")]
        public Transform groundCheck = null;
        public float groundDistance = 0.4f;
        public LayerMask groundMask;

        [Header("Dash")]
        public bool RechargeAll = false;
        public int dashAmount = 2;
        public float dashSpeed = 2f;
        public float dashTime = 2f;
        public float dashRechargeTime = 2f;
        private int curDashAmount = 0;
        private float currentDashSpeed = 0;
        private float currentDashRecharge = 0;
        private float currentDashTime = 0;
        private Vector3 dashVector = Vector3.zero;

        [Header("Bar Jump")]
        public float checkDistance = 1f;
        public float upForce = 1f;
        public float forwardForce = 2f;
        private JumpBar currentBar = null;

        [Header("Local Orientation")]
        public bool changeLocalOrientation = false;
        public float offset = 0, radiusDist = 1, heightDist = 1;
        private Vector3 lastOrientation = Vector3.zero;

        void Start()
        {
            RB = GetComponent<Rigidbody>();
            if (RB == null)
                RB = gameObject.AddComponent<Rigidbody>();
        }

        private void Update()
        {
            GetInput();
            LocalOrientationChange();
        }

        void FixedUpdate()
        {
            switch (State)
            {
                case State.Grounded:
                    Move();
                    Rot();
                    Gravity();
                    Dash();
                    break;

                case State.Airborne:
                    Move();
                    Rot();
                    Gravity();
                    Dash();
                    break;

                case State.Dashing:
                    Dash();
                    Rot();
                    break;

                case State.AirDash:
                    Dash();
                    Rot();
                    break;

                case State.BarJump:
                    break;
            }

            RB.MovePosition(transform.position + TargetPos);
        }

        private void GetInput()
        {
            InputX = Input.GetAxis("Horizontal");
            InputZ = Input.GetAxis("Vertical");

            RotY = Input.GetAxis("Mouse Y");
            RotX = Input.GetAxis("Mouse X");
        }

        private void Move()
        {
            TargetPos = (RB.transform.forward * InputZ + RB.transform.right * InputX).normalized * MoveSpeed * Time.deltaTime;
        }

        private void Rot()
        {
            RotX *= Time.deltaTime;

            xRot -= RotY;
            xRot = Mathf.Clamp(xRot, -90f, 90f);

            Cam.transform.localRotation = Quaternion.Euler(xRot, 0f, 0f);

            transform.Rotate(transform.up * RotX * 250);
        }

        private void Gravity()
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask, QueryTriggerInteraction.Ignore);

            Jump();

            RB.AddForce(velocity);

            if (localGravity)
                velocity += transform.up * gravity * Time.deltaTime;
            else
                velocity.y += gravity * Time.deltaTime;

            if (isGrounded)
            {
                if (localGravity)
                    velocity = transform.up * -2.0f;
                else
                    velocity.y = -2.0f;

                if (State == State.Airborne)
                    State = State.Grounded;
            }
            else if (State != State.Dashing && State != State.AirDash)
                State = State.Airborne;
        }

        private void Jump()
        {
            if (Input.GetKeyDown(KeyCode.Space) && !isGrounded && !hasDoubleJumped)
            {
                hasDoubleJumped = true;

                velocity = Vector3.zero;
                RB.velocity = Vector3.zero;
                RB.AddForce(transform.up * Mathf.Sqrt(jumpHeight * -2 * gravity), ForceMode.VelocityChange);
            }

            if (Input.GetKeyDown(KeyCode.Space) && State == State.Grounded)
            {
                hasDoubleJumped = false;

                velocity = Vector3.zero;
                RB.velocity = Vector3.zero;
                RB.AddForce(transform.up * Mathf.Sqrt(jumpHeight * -2 * gravity), ForceMode.VelocityChange);

                State = State.Airborne;
            }
        }

        private void Dash()
        {
            if (Input.GetKeyDown(KeyCode.LeftControl) && dashAmount > curDashAmount && currentDashSpeed == 0)
            {
                currentDashSpeed = dashSpeed;

                if (InputX != 0 || InputZ != 0)
                    dashVector = (transform.forward * InputZ + transform.right * InputX).normalized;
                else
                    dashVector = transform.forward;

                if (State != State.AirDash && State != State.Dashing)
                {
                    if (State == State.Airborne)
                        State = State.AirDash;
                    else
                        State = State.Dashing;
                }

                velocity = Vector3.zero;
                RB.velocity = Vector3.zero;

                curDashAmount++;

                RB.velocity = dashVector * currentDashSpeed;
            }

            if (curDashAmount != 0)
            {
                if (currentDashRecharge >= dashRechargeTime)
                {
                    currentDashRecharge = 0;

                    if (RechargeAll)
                        curDashAmount = 0;
                    else
                        curDashAmount--;
                }
                else
                    currentDashRecharge += Time.fixedDeltaTime;
            }

            if (State == State.Dashing || State == State.AirDash)
            {
                if (dashTime <= currentDashTime)
                {
                    currentDashSpeed = 0;
                    currentDashTime = 0;
                    RB.velocity = Vector3.zero;

                    if (State == State.AirDash)
                        State = State.Airborne;
                    else
                        State = State.Grounded;
                }
                else
                    currentDashTime += Time.fixedDeltaTime;
            }
        }

        private void LocalOrientationChange()
        {
            if (changeLocalOrientation)
            {
                if (!localGravity)
                    localGravity = true;

                int hitCount = 0;
                Vector3 newUpVec = Vector3.zero;
                RaycastHit hit;

                for (int i = 0; i < 9; i++)
                {
                    Vector3 pos = transform.position - transform.up * offset;
                    float x = 0, y = 0;

                    if (i == 0)
                    {
                        x = 1;
                        y = 1;
                    }
                    else if (i == 1)
                    {
                        y = 1;
                    }
                    else if (i == 2)
                    {
                        x = -1;
                        y = 1;
                    }
                    else if (i == 3)
                    {
                        x = 1;
                    }
                    else if (i == 4)
                    {
                        x = -1;
                    }
                    else if (i == 5)
                    {
                        x = 1;
                        y = -1;
                    }
                    else if (i == 6)
                    {
                        y = -1;
                    }
                    else if (i == 7)
                    {
                        x = -1;
                        y = -1;
                    }

                    pos += (transform.forward * y + transform.right * x).normalized * radiusDist;

                    if (Physics.Raycast(pos, -transform.up, out hit, heightDist, groundMask))
                    {
                        newUpVec += hit.normal;
                        hitCount++;
                    }
                    Debug.DrawRay(pos, -transform.up * heightDist);
                }

                if (hitCount != 0)
                    newUpVec = (newUpVec / hitCount).normalized;
                else
                    newUpVec = Vector3.up;

                Debug.Log(newUpVec);
                if (newUpVec != transform.up)
                {
                    Quaternion toRot = Quaternion.LookRotation(transform.forward, newUpVec) * transform.rotation;
                    transform.rotation = toRot;

                    lastOrientation = newUpVec;
                }
            }
        }
    }
}