using UnityEngine;

namespace CC_Controller
{
    public enum State { Grounded, Airborne, Dashing, AirDash, BarJump }
    public class CharacterControllerMovement : MonoBehaviour
    {
        public State State;

        [Header("Debug")]
        public bool DebugSystem = false;

        [Header("Object Reference")]
        public CharacterController CC = null;
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
        public float dashDistance = 2f;
        public float dashResistent = 5f;
        public float dashRechargeTime = 2f;
        private int curDashAmount = 0;
        private float currentDashSpeed = 0;
        private float currentDashRecharge = 0;
        private Vector3 dashVector = Vector3.zero;

        [Header("BarJump")]
        public float checkDistance = 1f;
        public float upForce = 1f;
        public float forwardForce = 2f;
        private JumpBar currentBar = null;

        private void Start()
        {
            CC = GetComponent<CharacterController>();
            if (CC == null)
                CC = gameObject.AddComponent<CharacterController>();
        }

        private void Update()
        {
            GetInput();

            switch (State)
            {
                case State.Grounded:
                    Move();
                    Rot();
                    Dash();
                    Gravity();
                    break;

                case State.Airborne:
                    Move();
                    Rot();
                    Dash();
                    Gravity();
                    DoubleJump();
                    BarJump();
                    break;

                case State.Dashing:
                    Rot();
                    Dash();
                    Gravity();
                    break;

                case State.AirDash:
                    Rot();
                    Dash();
                    BarJump();
                    break;

                case State.BarJump:
                    BarJump();
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
            TargetPos = (CC.transform.forward * InputZ + CC.transform.right * InputX).normalized * MoveSpeed * Time.deltaTime;

            if (State == State.Airborne)
                TargetPos *= 0.5f;

            CC.Move(TargetPos);
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
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

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

            Jump();
            DoubleJump();

            if (localGravity)
                velocity += transform.up * gravity * Time.deltaTime;
            else
                velocity.y += gravity * Time.deltaTime;


            CC.Move(velocity * Time.deltaTime);
        }

        private void Jump()
        {
            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            {
                if (localGravity)
                    velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                else
                    velocity = transform.up * Mathf.Sqrt(jumpHeight * -2f * gravity);

                hasDoubleJumped = false;

                State = State.Airborne;
            }
        }

        private void DoubleJump()
        {
            if (Input.GetKeyDown(KeyCode.Space) && !isGrounded && !hasDoubleJumped)
            {
                if (localGravity)
                    velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                else
                    velocity = transform.up * Mathf.Sqrt(jumpHeight * -2f * gravity);

                hasDoubleJumped = true;
            }
        }

        private void Dash()
        {
            if (Input.GetKeyDown(KeyCode.LeftControl) && dashAmount > curDashAmount)
            {
                currentDashSpeed = Mathf.Sqrt(dashDistance * 2f * dashResistent);
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

                curDashAmount++;
            }
            else if (curDashAmount != 0)
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
                    currentDashRecharge += Time.deltaTime;
            }

            if (currentDashSpeed > 0)
                currentDashSpeed -= dashResistent * Time.deltaTime;
            else
            {
                if (State == State.AirDash || State == State.Dashing)
                {
                    if (State == State.AirDash)
                        State = State.Airborne;
                    else
                        State = State.Grounded;
                }
                currentDashSpeed = 0;
            }

            CC.Move(dashVector * currentDashSpeed * Time.deltaTime);
        }

        private void Jetpack()
        {

        }

        private void BarJump()
        {
            if (!currentBar)
            {
                RaycastHit hit;
                if (Physics.Raycast(Cam.transform.position, Cam.transform.forward, out hit, checkDistance))
                {
                    if (hit.collider.GetComponent<JumpBar>() != null)
                    {
                        currentBar = hit.collider.GetComponent<JumpBar>();
                        currentBar.StartBarJump(CC);
                    }
                }
            }
            else
                currentBar.UpdateBarJump(CC);
        }
    }
}
