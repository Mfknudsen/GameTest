using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CalcWallWalking
{
    public class WallWalkingCalc : MonoBehaviour
    {
        public Vector3 lastSuccessfullWallTrace = Vector3.zero;
        public bool isWallWalkingAllowed = true;

        public float Fraction(Vector3 dir, Vector3 normal)
        {
            float result = 0;
            Vector3 fracDir = Vector3.Reflect(dir, normal);
            result = Vector3.Angle(-dir, fracDir);

            return result;
        }

        public RaycastHit Shared_TraceCapsule(Vector3 startPoint, Vector3 endPoint, float feelerSize, LayerMask physicsMask)
        {
            RaycastHit result;

            if (Physics.SphereCast(startPoint, feelerSize, (endPoint - startPoint), out result, physicsMask))
            {
                return result;
            }

            return result;
        }

        public Vector3 SmoothWallNormal(Vector3 currentNormal, Vector3 goalNormal, float fraction)
        {
            Vector3 result = goalNormal;

            if (fraction < 1)
            {
                float diff = Vector3.Dot(goalNormal, currentNormal);

                if (diff < 0.98f)
                {
                    Vector3 normalDiff = goalNormal - currentNormal;

                    if (diff == -1)
                    {
                        if (Vector3.Dot(transform.right, goalNormal) != -1)
                            normalDiff = goalNormal - currentNormal;
                        else
                            normalDiff = goalNormal - (-currentNormal);
                    }

                    result = (transform.up + normalDiff * 1);
                }
            }

            if (result.magnitude < 0.01)
                result = Vector3.up;

            return result;
        }

        public Vector3 GetAngelsFromWallNormal(RaycastHit hit)
        {
            GameObject obj = new GameObject();
            obj.transform.position = hit.point;
            obj.transform.up = hit.normal;
            obj.transform.forward = Vector3.Cross(obj.transform.up, obj.transform.forward);

            if (obj.transform.right.magnitude < 0.001)
                return Vector3.zero;
            else
            {
                obj.transform.forward = Vector3.Cross(obj.transform.right, obj.transform.up);

                Vector3 angels = obj.transform.rotation.eulerAngles;
                return angels;
            }
        }

        public bool ValidWallTrace(RaycastHit trace)
        {
            if (trace.transform.tag == "")
            {
                GameObject entity = trace.collider.gameObject;
                bool entityClingable = entity != null && (entity.GetComponent<>().changeLocalOrientation && isWallWalkingAllowed);
                return (entity != null || entityClingable);
            }

            return false;
        }

        public bool TraceWallNormal(Vector3 startPoint, Vector3 endPoint, List<Vector3> result, float feelerSize, LayerMask physicsMask)
        {
            RaycastHit localTrace;

            if (Physics.Raycast(startPoint, (endPoint - startPoint), out localTrace, feelerSize, physicsMask))
            {
                if (ValidWallTrace(localTrace))
                {
                    result.Add(localTrace.normal);
                    return true;
                }
            }

            return false;
        }

        public Vector3 GetAverageWallWalkingNormal(float extraRange, float feelerSize, LayerMask physicsMaskOverride)
        {
            LayerMask physicsMask = physicsMaskOverride;

            Vector3 startPoint = transform.position;
            Vector3 extents = Vector3.zero;
            startPoint.y = extents.y;

            int numTraces = 8;
            List<Vector3> wallNormals = new List<Vector3>();

            float wallWalkingRange = Mathf.Max(extents.x, extents.y) + extraRange;
            Vector3 endPoint = Vector3.zero;
            Vector3 directionVector = Vector3.zero;
            float angle = 0;
            bool normalFound = true;

            if (lastSuccessfullWallTrace == null || TraceWallNormal(startPoint, startPoint + lastSuccessfullWallTrace * wallWalkingRange, wallNormals, feelerSize, physicsMask))
            {
                normalFound = false;

                for (int i = 0; i < (numTraces - 1); i++)
                {
                    angle = ((i * 360 / numTraces) / 360) * Mathf.PI * 2;
                    directionVector = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));

                    endPoint.x = startPoint.x + directionVector.x * wallWalkingRange;
                    endPoint.y = startPoint.y;
                    endPoint.z = startPoint.z + directionVector.z * wallWalkingRange;

                    if (TraceWallNormal(startPoint, endPoint, wallNormals, 1, physicsMask))
                    {
                        lastSuccessfullWallTrace = directionVector;
                        normalFound = true;
                        break;
                    }
                }
            }

            //Check above.
            if (!normalFound)
                normalFound = TraceWallNormal(startPoint, endPoint + new Vector3(0, wallWalkingRange, 0), wallNormals, feelerSize, physicsMask);

            if (!normalFound)
            {
                for (int i = 0; i < 8; i++)
                {
                    float theta = (i / 8) * Mathf.PI * 2;
                    normalFound = TraceWallNormal(startPoint, startPoint + new Vector3(Mathf.Cos(theta) * wallWalkingRange * 0.707f, wallWalkingRange * 0.707f, Mathf.Sin(theta) * wallWalkingRange * 0.707f), wallNormals, feelerSize, physicsMask);
                    if (normalFound)
                        break;
                }
            }


            if (normalFound)
            {
                //CheckUnder
                RaycastHit hit;
                if (Physics.Raycast(startPoint, startPoint + new Vector3(0, -wallWalkingRange, 0), out hit))
                {
                    float fraction = Fraction(new Vector3(0, -wallWalkingRange, 0), hit.normal);
                    if (fraction < 1 && fraction > 0 && hit.transform.gameObject != null)
                        return hit.normal;
                }

                return wallNormals[1];
            }
            return Vector3.zero;
        }
    }
}