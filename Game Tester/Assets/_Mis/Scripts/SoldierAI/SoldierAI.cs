using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//Made following Sharp Accent on Youtube.
//This script is the main controller of the Soldier AI.
//It will determen what to do and when to do it.
//It will take in values of other scripts and it will also give values to those scripts.

//Namespace is used to gather all scripts under one name.
namespace SA.AI
{
    //Stating the name of the class and stating that it is a MonoBehavior script.
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
        public bool ChangeSt;

        public InViewSubCat InviewSub;

        //Valiues to help optimize the frame rate of which the script will update.
        delegate void EveryFrame();
        EveryFrame everyFrame;
        delegate void LateFrame();
        LateFrame lateFrame;
        delegate void LateLateFrame();
        LateLateFrame llateFrame;

        //Making an Agent to help the AI move around.
        [HideInInspector]
        public NavMeshAgent Agent;

        //Making a value to hold the stats which is determend by the script "EnemyStats"
        public EnemyStats EnStats = new EnemyStats();

        //The Start function is the first one to be called when the script is updated and will only be done on the very first update.
        void Start()
        {
            //Getting the Agent component from the objekt.
            Agent = GetComponent<NavMeshAgent>();
            
            //Stating that the AI must start at idle.
            aistate = AIState.idle;
            ChangeState(AIState.idle);
        }

        //The Update function is called at every update of this script.
        public void Update()
        {
            //Made a change state buttom to change the state of the AI manually in the inspector.
            if (ChangeSt)
            {
                ChangeState(TargetState);
                ChangeSt = false;
            }

            //Calling the Monitor state.
            MonitorStates();
            
            //Creating 3 different values to delay som functions to improve performens.
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

        //Making a function called MonitorStates which is essentialy a behavior tree with a state idle as its root.
        void MonitorStates()
        {
            //This "switch" function determens what state the AI should be in based on other values.
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
                    if(EnStats.DistanceFromTarget > Radius)
                            ChangeState(AIState.inRadius);
                    if(!IsClear)
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

        //A function that determens the life of a state.
        void MonitorBehaviorLife()
        {
            EnStats.BehaviorLife += Time.deltaTime;
            if (EnStats.BehaviorLife > EnStats.MaxBehaviorLife)
            {
                EnStats.BehaviorLife = 0;
                ChangeState(TargetState);
            }
        }

        //A function that let me manually change the state from the inspector.
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
                    everyFrame = InViewbehaviour;
                    EnStats.LastKnownPosition = PlayerTarget.position;
                    StopMoving();
                    InviewSub = InViewSubCat.LookCover;
                    if(Vector3.Distance(transform.position, EnStats.LastKnownPosition) < 10)
                        InviewSub = InViewSubCat.InChase;
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

        //The "Idle" behavior will make the AI use the patrol from the script "CommonBehavior" if it finds an active PlayerTarget.
        void IdleBehaviour()
        {
            if (PlayerTarget == null)
                return;

            DistanceCheckPlayer(PlayerTarget);
            CommonBehavior.PatrolBehavior(this);
        }

        //The "InRadius" behavior wiil make the AI use the patrol from the script "CommonBehavior" if it finds an active PlayerTarget and will begin to check if the PlayerTarget is visble to the AI using the "Sight" function.
        void InRadiusBehaviour()
        {
            if (PlayerTarget == null)
                return;

            Sight();
            CommonBehavior.PatrolBehavior(this);
        }

        //The "InView" behavior will switch the functions of "InViewSub" if it finds an active PlayerTarget and rotate towards the PlayerTarget and find the distance between the AI and the PlayerTarget.
        void InViewbehaviour()
        {
            if (PlayerTarget == null)
                return;

            FindDirection(PlayerTarget);
            DistanceCheckPlayer(PlayerTarget);

            switch (InviewSub)
            {
                case InViewSubCat.InRange:
                    Debug.Log("InRange");
                    RotateTorwardsTarget();
                    StopMoving();
                    break;
                case InViewSubCat.InChase:
                    Debug.Log("InChase");
                    CommonBehavior.ChaseBehavior(this);
                    break;
                case InViewSubCat.InCover:
                    Debug.Log("InCover");
                    RotateTorwardsTarget();
                    StopMoving();
                    break;
                case InViewSubCat.LookCover:
                    Debug.Log("LookCover");
                    CommonBehavior.GetToCover(this);
                    break;
            }
        }

        //A function that let me manually change the sub state from the inspector.
        public void ChangeInViewState(InViewSubCat Target)
        {
            InviewSub = Target;
        }

        //A secondary function to the "InView" behavior that are used to monitor if it is still valid.
        void InViewBehaviorSecondary()
        {
            Sight();
            MonitorTargetPosition();
        }

        //A function that take the use of the script "SearchBehavior" in CommomBehavior. Its function is to find the PlayerTarget after it has lost sight of it.
        void InSearchBehavior()
        {
            CommonBehavior.SearchBehavior(this);
            Sight();
        }

        //A function that produces a field of view for the AI and checks if the line of sight is clear of obsicals.
        void Sight()
        {
            DistanceCheckPlayer(PlayerTarget);
            FindDirection(PlayerTarget);
            AngelCheck();
            if(IsInAngel)
                IsClearView(PlayerTarget);
        }

        //Finding the distance between the AI and the PlayerTarget for the EnemyStats script.
        void DistanceCheckPlayer(Transform Target)
        {
            EnStats.DistanceFromTarget = Vector3.Distance(transform.position, Target.position);
        }

        //Checking if there is a direct line to the PlayerTarget.
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

        //Checking if the PlayerTarget is within the the line of sight. (If it is right in front of the AI)
        void AngelCheck()
        {
            RotDirection = Direction;
            RotDirection.y = 0;
            if (RotDirection == Vector3.zero)
                RotDirection = transform.forward;

            float Angle = Vector3.Angle(transform.forward, RotDirection);

            IsInAngel = (Angle < 60);
        }

        //Rotating the AI towards the PlayerTarget.
        void RotateTorwardsTarget()
        {
            if (RotDirection == Vector3.zero)
                RotDirection = transform.forward;

            Quaternion TargetRotation = Quaternion.LookRotation(RotDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, TargetRotation, Time.deltaTime * RotSpeed);
        }

        //Finding the direction towards the PlayerTarget.
        void FindDirection(Transform Target)
        {
            Direction = Target.position - transform.position;
            RotDirection.y = 0;
        }

        //Finding the direction to the "LastKnowLocation".
        void MonitorTargetPosition()
        {
            float DeltaDistance = Vector3.Distance(PlayerTarget.position, EnStats.LastKnownPosition);
            if (DeltaDistance > 2)
            {
                EnStats.LastKnownPosition = PlayerTarget.position;
                MoveToPosition(EnStats.LastKnownPosition);
            }
        }

        //Instuctin the AI's agent to move it to the TargetPosition.
        public void MoveToPosition(Vector3 TargetPosition)
        {
            Agent.Resume();
            Agent.SetDestination(TargetPosition);
        }

        //A function to tell the Agent to stop moving.
        public void StopMoving()
        {
            Agent.Stop();
        }

        //Creating a random position for the AI to relocate to when it is attempting to search for the PlayerTarget on the PlayerTargets LastKnowLocations.
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

        //Created the behavior tree "InViewSubCat" and stating the different behaviors.
        public enum InViewSubCat
        {
            InRange, InChase, InCover, LookCover
        }

        //Created the behavior tree "AIState" and stating the different behaviors.
        public enum AIState
        {
            idle, lateIdle, inRadius, inView, inSearch
        }
    }
}