using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA.AI
{
    [System.Serializable]
    public class EnemyStats
    {
        public List<WaypointBase> Waypoints = new List<WaypointBase>();

        public float inRange = 5;

        [HideInInspector]
        public WaypointBase CurrentWaypoint;
        [HideInInspector]
        public int WaypointIndex;
        [HideInInspector]
        public float WaypointDistance;
        [HideInInspector]
        public float WaitTimeWP;
        [HideInInspector]
        public float MaxWaitTime;
        [HideInInspector]
        public float BehaviorLife;
        [HideInInspector]
        public float MaxBehaviorLife;
        [HideInInspector]
        public float DistanceFromTarget;
        [HideInInspector]
        public Vector3 LastKnownPosition;
        [HideInInspector]
        public Vector3 CandiatePosition;

        public Transform TargetCover;
    }
}
