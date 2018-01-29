using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//Namespace is used to 
namespace SA.AI
{
    public class SoldierAI : MonoBehaviour
    {

        //Used to contain the Player position, rotation and scale.
        public Transform PlayerTarget;

        //Valiues used to determin where the Player is compared to the AI.
        Vector3 Direction;
        Vector3 RotDirection;
        Vector3 LastKnowPosition;

        //Public valiues used to let me control how fast it can turn and how long it can see.
        public float RotSpeed;
        public float ViewDistance;

        //Valiues that help control when parts of the script can be used.
        bool IsInView;
        bool IsInAngel;
        bool IsClear; 

        float Radius = 50;
        float MaxDistance = 80;

        //Valiues used to delay certain parts of the scripts to optimize the overall performans of the game.
        int lFrame = 10;
        int lFrameCounter = 0;

        //Valiues used to delay certain parts of the scripts futher the lFrame to optimize the overall performans of the game.
        int llFrame = 30;
        int llFrameCounter = 0;

        //Creating a AIState called aistate to help maintain a form for behaviour tree.
        public AIState aistate;
        public AIState TargetState;
        public InViewSubCat InviewSub;

        //Valiues to help optimize the frame rate of witch the script will update.
        delegate void EveryFrame();
        EveryFrame everyFrame;
        delegate void LateFrame();
        LateFrame lateFrame;
        delegate void LateLateFrame();
        LateLateFrame llateFrame;

        [HideInInspector]
        public NavMeshAgent Agent;
        public EnemyStats EnStats = new EnemyStats();

        public bool ChangeSt;

        void Start()
        {
            Agent = GetComponent<NavMeshAgent>();
            
            aistate = AIState.idle;
            ChangeState(AIState.idle);
        }

        public void Update()
        {
            if (ChangeSt)
            {
                ChangeState(TargetState);
                ChangeSt = false;
            }

            MonitorStates();

            if (everyFrame != null)
                everyFrame();

            lFrameCounter++;
            if (lFrameCounter > lFrame)
            {
                if (lateFrame != null)
                    lateFrame();

                lFrameCounter = 0;
            }

            llFrameCounter++;
            if (llFrameCounter > llFrame)
            {
                if (llateFrame != null)
                    llateFrame();

                llFrameCounter = 0;
            }
        }

        void MonitorStates()
        {
            switch (aistate)
            {
                case AIState.idle:
                    if (EnStats.DistanceFromTarget < Radius)
                        ChangeState(AIState.inRadius);
                    if (EnStats.DistanceFromTarget > MaxDistance)
                        ChangeState(AIState.lateIdle);
                    break;
                case AIState.lateIdle:
                    if (EnStats.DistanceFromTarget < MaxDistance)
                        ChangeState(AIState.idle);
                    break;
                case AIState.inRadius:
                    if (EnStats.DistanceFromTarget > Radius)
                        ChangeState(AIState.idle);
                    if (IsClear)
                        ChangeState(AIState.inView);
                    break;
                case AIState.inView:
                    if (EnStats.DistanceFromTarget > Radius)
                            ChangeState(AIState.inRadius);
                    if (!IsClear)
                        ChangeState(AIState.inSearch);
                    break;
                case AIState.inSearch:
                    if (IsClear)
                        ChangeState(AIState.inView);
                    MonitorBehaviorLife();
                    break;
                default:
                    break;
            }
        }

        void MonitorBehaviorLife()
        {
            EnStats.BehaviorLife += Time.deltaTime;
            if (EnStats.BehaviorLife > EnStats.MaxBehaviorLife)
            {
                EnStats.BehaviorLife = 0;
                ChangeState(TargetState);
            }
        }

        public void ChangeState(AIState TargetState)
        {
            aistate = TargetState;
            everyFrame = null;
            lateFrame = null;
            llateFrame = null;

            switch (TargetState)
            {
                case AIState.idle:
                    lateFrame = IdleBehaviour;
                    break;
                case AIState.lateIdle:
                    llateFrame = IdleBehaviour;
                    break;
                case AIState.inRadius:
                    lateFrame = InRadiusBehaviour;
                    break;
                case AIState.inView:
                    lateFrame = InViewBehaviorSecondary;
                    llateFrame += MonitorTargetPosition;
                    everyFrame = InViewbehaviour;
                    EnStats.LastKnownPosition = PlayerTarget.position;
                    StopMoving();
                    InviewSub = InViewSubCat.LookCover;
                    //MoveToPosition(LastKnowPosition);
                    break;
                case AIState.inSearch:
                    lateFrame = InSearchBehavior;
                    EnStats.LastKnownPosition = PlayerTarget.position;
                    EnStats.CandiatePosition = EnStats.LastKnownPosition;
                    StopMoving();
                    MoveToPosition(EnStats.LastKnownPosition);
                    float BehaviourLifeOffset = Random.Range(-2, 3);
                    EnStats.MaxBehaviorLife = CommonBehavior.SearchLife + BehaviourLifeOffset;
                    TargetState = AIState.idle;
                    WaypointBase WP = new WaypointBase();
                    EnStats.CurrentWaypoint = WP;
                    break;
                default:
                    break;
            }
        }

        void IdleBehaviour()
        {
            if (PlayerTarget == null)
                return;

            DistanceCheckPlayer(PlayerTarget);
            CommonBehavior.PatrolBehavior(this);
        }

        void InRadiusBehaviour()
        {
            if (PlayerTarget == null)
                return;

            Sight();
            CommonBehavior.PatrolBehavior(this);
        }

        void InViewbehaviour()
        {
            if (PlayerTarget == null)
                return;

            FindDirection(PlayerTarget);

            switch (InviewSub)
            {
                case InViewSubCat.InRange:
                    Debug.Log("InRange");
                    RotateTorwardsTarget();
                    break;
                case InViewSubCat.InChase:
                    Debug.Log("InChase");
                    CommonBehavior.ChaseBehavior(this);
                    break;
                case InViewSubCat.InCover:
                    Debug.Log("InCover");
                    RotateTorwardsTarget();
                    break;
                case InViewSubCat.LookCover:
                    Debug.Log("LookCover");
                    CommonBehavior.GetToCover(this);
                    break;
            }
        }

        public void ChangeInViewState(InViewSubCat Target)
        {
            InviewSub = Target;
        }

        void InViewBehaviorSecondary()
        {
            Sight();
            MonitorTargetPosition();
        }

        void InSearchBehavior()
        {
            CommonBehavior.SearchBehavior(this);
            Sight();
        }

        void Sight()
        {
            DistanceCheckPlayer(PlayerTarget);
            FindDirection(PlayerTarget);
            AngelCheck();
            if(IsInAngel)
                IsClearView(PlayerTarget);
        }

        void DistanceCheckPlayer(Transform Target)
        {
            EnStats.DistanceFromTarget = Vector3.Distance(transform.position, Target.position);
        }

        void IsClearView(Transform Target)
        {
            IsClear = false;
            RaycastHit Hit;
            Vector3 Origin = transform.position;
            Debug.DrawRay(Origin, Direction * Radius);
            if (Physics.Raycast(Origin, Direction, out Hit, Radius))
            {
                if (Hit.transform.CompareTag("Player"))
                {
                    IsClear = true;
                }
            }
        }

        void AngelCheck()
        {
            RotDirection = Direction;
            RotDirection.y = 0;
            if (RotDirection == Vector3.zero)
                RotDirection = transform.forward;

            float Angle = Vector3.Angle(transform.forward, RotDirection);

            IsInAngel = (Angle < 60);
        }

        void RotateTorwardsTarget()
        {
            if (RotDirection == Vector3.zero)
                RotDirection = transform.forward;

            Quaternion TargetRotation = Quaternion.LookRotation(RotDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, TargetRotation, Time.deltaTime * RotSpeed);
        }

        void FindDirection(Transform Target)
        {
            Direction = Target.position - transform.position;
            RotDirection.y = 0;
        }

        void MonitorTargetPosition()
        {
            float DeltaDistance = Vector3.Distance(PlayerTarget.position, EnStats.LastKnownPosition);
            if (DeltaDistance > 2)
            {
                EnStats.LastKnownPosition = PlayerTarget.position;
                MoveToPosition(EnStats.LastKnownPosition);
            }
        }

        public void MoveToPosition(Vector3 TargetPosition)
        {
            Agent.Resume();
            Agent.SetDestination(TargetPosition);
        }

        public void StopMoving()
        {
            Agent.Stop();
        }

        public Vector3 RandomVector3AroundPosition(Vector3 TargetPosition)
        {
            float Offset = Random.Range(-10, 10);
            Vector3 OriginPosition = TargetPosition;
            OriginPosition.x += Offset;
            OriginPosition.z += Offset;

            NavMeshHit Hit;
            if(NavMesh.SamplePosition(OriginPosition, out Hit, 5, NavMesh.AllAreas))
            {
                return Hit.position;
            }

            return TargetPosition;
        }

        public enum InViewSubCat
        {
            InRange, InChase, InCover, LookCover
        }

        public enum AIState
        {
            idle, lateIdle, inRadius, inView, inSearch
        }
    }
}