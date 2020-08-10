#region Unity Libraries
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#endregion
#region DevClasses
using CalcWallWalking;
#endregion

namespace RB_Controller
{
    public enum State { Grounded, Airborne, Dashing, AirDash, BarJump }
    public class RigidbodyMovement : MonoBehaviour
    {
        public State State;

        [Header("System")]
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
        private float camX = 0;

        [Header("Jump")]
        public float jumpHeight = 3f;
        private bool hasDoubleJumped = false;

        [Header("Jetpack")]
        public float fuelCount = 100;
        public float jetConsume = 5;

        [Header("Gravity")]
        public bool localGravity = false;
        public float gravity = -9.81f;
        private Vector3 velocity = Vector3.zero, gravDir = Vector3.up;
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

        [Header("Local Orientation")]
        public float sphereRadius;
        public float maxDistance;
        public LayerMask layerMask;
        private WallWalkingCalc WW_Calc = null;
        private Vector3 goalNormal = Vector3.up;
        private Vector3 origin;
        private Vector3 direction;

        void Start()
        {
            WW_Calc = ScriptableObject.CreateInstance("WallWalkingCalc") as WallWalkingCalc;
            WW_Calc.feelerSize = sphereRadius;

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

            RB.MovePosition(transform.position + TargetPos);
        }

        private void Rot()
        {
            RotX *= Time.fixedDeltaTime;

            camX -= RotY * Time.fixedDeltaTime * RotSpeed * 10;
            camX = Mathf.Clamp(camX, -90f, 90f);

            Cam.transform.localRotation = Quaternion.Euler(camX, 0f, 0f);

            transform.RotateAround(transform.position, transform.up, RotX * RotSpeed * 10);
        }

        private void Gravity()
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask, QueryTriggerInteraction.Ignore);

            Jump();

            RB.AddForce(velocity);

            if (localGravity)
                velocity += gravDir * gravity * Time.deltaTime;
            else
                velocity.y += gravity * Time.deltaTime;

            if (isGrounded)
            {
                if (localGravity)
                    velocity = gravDir * -2.0f;
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
            origin = transform.position;
            direction = (transform.forward * InputZ + transform.right * InputX).normalized;

            goalNormal = WW_Calc.GetAverageWallWalkingNormal(origin, maxDistance, sphereRadius, layerMask);

            Vector3 newNormal = WW_Calc.SmoothWallNormal(transform, goalNormal);
            transform.rotation = Quaternion.FromToRotation(transform.up, newNormal) * transform.rotation;
        }
    }
}
/*if (changeLocalOrientation)
{
    if (State == State.Grounded && isGrounded)
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position - transform.up * offset, -transform.up, out hit, heightDist, groundMask))
        {
            newUpVec = hit.normal;
        }
        if (InputZ != 0 || InputX != 0)
        {
            if (Physics.Raycast(transform.position - transform.up * offset + (transform.forward * InputZ + transform.right * InputX).normalized * radiusDist, -transform.up, out hit, heightDist, groundMask))
            {
                newUpVec += hit.normal;
            }

            if (DebugSystem)
                Debug.DrawRay(transform.position - transform.up * offset + (transform.forward * InputZ + transform.right * InputX).normalized * radiusDist, -transform.up * heightDist);
        }
    }
    else if (State == State.Airborne && !isGrounded)
        newUpVec = Vector3.up;

    targetRotation = newUpVec.normalized;

    if (Vector3.Angle(curRotation, targetRotation) >= 2.5f)
        curRotation = Vector3.Lerp(curRotation, targetRotation, turnSpeed * Time.deltaTime);
    else
        curRotation = targetRotation;

    transform.rotation = Quaternion.FromToRotation(transform.up, curRotation) * transform.rotation;

    gravDir = targetRotation;
}*/
