using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA.AI
{
    public class AIDirector : MonoBehaviour
    {
        public List<Transform> CoverPosition = new List<Transform>();

        public Transform GetCover(Vector3 From, Vector3 Towards)
        {
            Transform R = null;
            float MinDistance = Mathf.Infinity;

            for(int i = 0; i < CoverPosition.Count; i++)
            {
                Vector3 Direction = Towards - CoverPosition[i].position;
                Direction.y = 0;

                float Angel = Vector3.Angle(Direction, CoverPosition[i].forward);

                if (Angel < 75)
                {
                    float TempDirection = Vector3.Distance(From, CoverPosition[i].position);

                    if (TempDirection < MinDistance)
                    {
                        MinDistance = TempDirection;
                        R = CoverPosition[i];
                    }
                }
            }

            return R;
        }

        public static AIDirector Singleton;
        void Awake()
        {
            Singleton = this;
        }
    }
}
