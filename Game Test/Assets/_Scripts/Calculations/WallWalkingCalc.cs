using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CalcWallWalking
{
    public class WallWalkingCalc : ScriptableObject
    {
        public Vector3 lastSuccessfullWallTrace = Vector3.zero;
        public bool isWallWalkingAllowed = true;
        public float feelerSize = 1;

        public float Fraction(Vector3 dir, Vector3 normal)
        {
            Vector3 fracDir = Vector3.Reflect(dir, normal);
            float result = Vector3.Angle(-dir, fracDir);

            return result;
        }

        public RaycastHit Shared_TraceCapsule(Vector3 startPoint, Vector3 endPoint, float feelerSize, LayerMask physicsMask)
        {
            RaycastHit result;

            //Bruger en SphereCast da det kan også tager i betrægning om hvorvidt firguren har tilstrækelige plads til at bevæge sig hen til overfladen
            if (Physics.SphereCast(startPoint, feelerSize, (endPoint - startPoint), out result, physicsMask))
            {
                return result;
            }

            return result;
        }

        public Vector3 SmoothWallNormal(Transform transform, Vector3 goalNormal)
        {
            Vector3 result = goalNormal;
            float fraction = Fraction(transform.up, goalNormal);

            if (fraction < 1)
            {
                float diff = Vector3.Dot(goalNormal, transform.up);

                if (diff < 0.98f)
                {
                    Vector3 normalDiff = goalNormal - transform.up;

                    if (diff == -1)
                    {
                        if (Vector3.Dot(transform.right, goalNormal) != -1)
                            normalDiff = goalNormal - transform.up;
                        else
                            normalDiff = goalNormal - (-transform.up);
                    }

                    result = (transform.up + normalDiff * fraction);
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
            GameObject entity = trace.collider.gameObject;

            if (entity != null)
            {
                if (entity.tag == "Clingable" && trace.normal != Vector3.zero)
                {
                    Debug.Log(trace.normal);
                    return true;
                }
            }

            return false;
        }

        public List<Vector3> TraceWallNormal(Vector3 startPoint, Vector3 endPoint, List<Vector3> result, float feelerSize, LayerMask physicsMask)
        {
            RaycastHit localTrace;

            Debug.DrawRay(startPoint, (endPoint - startPoint).normalized * feelerSize);
            if (Physics.SphereCast(startPoint, feelerSize, (endPoint - startPoint).normalized, out localTrace, (endPoint - startPoint).magnitude, physicsMask, QueryTriggerInteraction.Ignore))
            {
                if (ValidWallTrace(localTrace))
                {
                    result.Insert(0, localTrace.normal);
                    return result;
                }
            }
            return result;
        }

        public Vector3 GetAverageWallWalkingNormal(Transform startTransform, float extraRange, float feelerSize, LayerMask physicsMaskOverride)
        {
            LayerMask physicsMask = physicsMaskOverride;

            Vector3 startPoint = startTransform.position;
            Vector3 extents = Vector3.one;
            startPoint.y = extents.y;

            int numTraces = 8;
            List<Vector3> wallNormals = new List<Vector3>();
            List<Vector3> checkNormals = new List<Vector3>();

            float wallWalkingRange = Mathf.Max(extents.x, extents.y) + extraRange;
            Vector3 endPoint = Vector3.zero;
            Vector3 directionVector = Vector3.zero;
            float angle = 0;
            bool normalFound = true;

            checkNormals = TraceWallNormal(startPoint, startPoint + lastSuccessfullWallTrace * wallWalkingRange, wallNormals, feelerSize, physicsMask);
            if (lastSuccessfullWallTrace == Vector3.zero || wallNormals.Count != checkNormals.Count)
            {
                normalFound = false;

                for (int i = 0; i < (numTraces); i++)
                {
                    angle = ((i * 360 / numTraces) / 360) * Mathf.PI * 2;
                    directionVector = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));

                    endPoint.x = startPoint.x + directionVector.x * wallWalkingRange;
                    endPoint.y = startPoint.y;
                    endPoint.z = startPoint.z + directionVector.z * wallWalkingRange;

                    checkNormals = TraceWallNormal(startPoint, endPoint, wallNormals, feelerSize, physicsMask);
                    if (wallNormals.Count != checkNormals.Count)
                    {
                        lastSuccessfullWallTrace = directionVector;
                        wallNormals = checkNormals;
                        normalFound = true;
                        break;
                    }
                }
            }

            //Check above.
            if (!normalFound)
            {
                checkNormals = TraceWallNormal(startPoint, endPoint + new Vector3(0, wallWalkingRange, 0), wallNormals, feelerSize, physicsMask);
                if (wallNormals.Count != checkNormals.Count)
                {
                    wallNormals = checkNormals;
                    normalFound = true;
                }
            }

            if (!normalFound)
            {
                for (int i = 0; i < 8; i++)
                {
                    float theta = (i / 8) * Mathf.PI * 2;

                    checkNormals = TraceWallNormal(startPoint, startPoint + new Vector3(Mathf.Cos(theta) * wallWalkingRange * 0.707f, wallWalkingRange * 0.707f, Mathf.Sin(theta) * wallWalkingRange * 0.707f), wallNormals, feelerSize, physicsMask);
                    if (wallNormals.Count != checkNormals.Count)
                    {
                        wallNormals = checkNormals;
                        normalFound = true;
                        break;
                    }
                }
            }


            if (normalFound)
            {
                //CheckUnder
                checkNormals = TraceWallNormal(startPoint, startPoint + new Vector3(0, -wallWalkingRange, 0), wallNormals, feelerSize, physicsMask);
                if (wallNormals.Count != checkNormals.Count)
                {
                    wallNormals = checkNormals;
                    float fraction = Fraction(new Vector3(0, -wallWalkingRange, 0), wallNormals[0]);
                    if (fraction < 1 && fraction > 0)
                        return wallNormals[0];
                }

                if (wallNormals.Count > 0)
                    return wallNormals[wallNormals.Count - 1];
            }

            return Vector3.up;
        }
    }
}