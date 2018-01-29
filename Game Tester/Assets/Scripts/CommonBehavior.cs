using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA.AI
{
    public static class CommonBehavior
    {
        public static float SearchLife = 60;

        public static void PatrolBehavior(SoldierAI AI)
        {
            EnemyStats E = AI.EnStats;

            if (AI.EnStats.Waypoints.Count > 0)
            {
                if (E.CurrentWaypoint.TargetDestination == null)
                {
                    E.CurrentWaypoint = E.Waypoints[E.WaypointIndex];
                    AI.MoveToPosition(E.CurrentWaypoint.TargetDestination.position);
                }

                if(AI.Agent.velocity == Vector3.zero)
                AI.Agent.Resume();

                E.CurrentWaypoint = E.Waypoints[E.WaypointIndex];

                E.WaypointDistance = Vector3.Distance(AI.transform.position, E.CurrentWaypoint.TargetDestination.position);

                if (E.WaypointDistance < 2)
                {
                    E.WaitTimeWP += Time.deltaTime * 15;

                    if (E.WaitTimeWP > E.MaxWaitTime)
                    {
                        E.WaitTimeWP = 0;

                        if (E.WaypointIndex < E.Waypoints.Count - 1)
                        {
                            E.WaypointIndex++;
                        }
                        else
                        {
                            E.WaypointIndex = 0;
                            //To make the AI go in circels just remove .Reverse.
                            //E.Waypoints.Reverse();
                        }

                        E.CurrentWaypoint = E.Waypoints[E.WaypointIndex];
                        E.MaxWaitTime = E.CurrentWaypoint.WaitTime;
                        AI.MoveToPosition(E.CurrentWaypoint.TargetDestination.position);
                    }
                }
            }
        }

        public static void SearchBehavior(SoldierAI AI)
        {
            EnemyStats E = AI.EnStats;

            float DistanceCheck = Vector3.Distance(AI.transform.position, E.CandiatePosition);

            if (DistanceCheck < 2)
            {
                E.WaitTimeWP += Time.deltaTime * 15;

                if (E.WaitTimeWP > E.MaxWaitTime)
                {
                    E.WaitTimeWP = 0;
                    E.MaxWaitTime = Random.Range(3, 6);

                    E.CandiatePosition = AI.RandomVector3AroundPosition(E.LastKnownPosition);
                    AI.MoveToPosition(E.CandiatePosition);
                }
            }

        }

        public static void ChaseBehavior(SoldierAI AI)
        {
            EnemyStats E = AI.EnStats;

            float DistanceCheck = Vector3.Distance(AI.transform.position, E.CandiatePosition);

            if(DistanceCheck < E.inRange)
            {
                AI.StopMoving();
            }
            else
            {
                AI.MoveToPosition(E.CandiatePosition);
            }
        }

        public static void GetToCover(SoldierAI AI)
        {
            EnemyStats E = AI.EnStats;

            if(E.TargetCover == null)
            {
                E.TargetCover = AIDirector.Singleton.GetCover(AI.transform.position, E.LastKnownPosition);

                if (E.TargetCover == null)
                {
                    Debug.Log("No TargetCover Found");
                    //AI.ChangeInViewState(SoldierAI.InViewSubCat.InChase);
                    return;
                }
                else
                    AI.MoveToPosition(E.TargetCover.position);
            }
            else
            {
                float DistanceCheck = Vector3.Distance(AI.transform.position, E.TargetCover.position);

                if (DistanceCheck < 2)
                {
                    AI.StopMoving();
                    AI.ChangeInViewState(SoldierAI.InViewSubCat.InCover);
                }
            }
        }
    }

    [System.Serializable]
    public class WaypointBase
    {
        public Transform TargetDestination;
        public float WaitTime = 1;
    }
}