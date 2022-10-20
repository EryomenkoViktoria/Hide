using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Morkwa.Test.Mechanics.AI
{
    public class AIView : MonoBehaviour
    {
        [SerializeField] private float _radiusView;
        [SerializeField, Range(0, 360)] private float _viewAngle;
        [SerializeField] private MeshFilter _viewMeshFilter;
        [SerializeField] private Mesh _viewMesh;

        [SerializeField] private LayerMask _targetMask;
        [SerializeField] private LayerMask _obstacleLayerMask;
        private List<Transform> _visibleTargets = new List<Transform>();
        [SerializeField] private float _meshResolution = 1f;
        [SerializeField] private int _edgeResolveIterations = 5;
        [SerializeField] private float _edgeDistanceThreshold = 0.5f;
        [HideInInspector] public bool IsSeeing { get; private set; }
        [SerializeField] private float _timeDelay = 0.5f;

        private void Start()
        {
            MeshConfig();
            StartCoroutine(FindTargetsWithDelay());
        }

        private void FixedUpdate()
        {
            DrawFieldOfView();
        }

        private void MeshConfig()
        {
            _viewMesh = new Mesh();
            _viewMesh.name = "View Mesh";
            _viewMeshFilter.mesh = _viewMesh;
        }

        private IEnumerator FindTargetsWithDelay()
        {
            while (true)
            {
                FindVisibleTargets();
                yield return new WaitForSeconds(_timeDelay);
            }
        }

        private void FindVisibleTargets()
        {
            _visibleTargets.Clear();

            Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, _radiusView, _targetMask);

            for (int i = 0; i < targetsInViewRadius.Length; i++)
            {
                Transform target = targetsInViewRadius[i].transform;
                Vector3 dirToTarget = (target.position - transform.position).normalized; 

                if (Vector3.Angle(transform.forward, dirToTarget) < _viewAngle / 2)
                {
                    float dstToTarget = Vector3.Distance(transform.position, target.position);
                    
                    if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, _obstacleLayerMask))
                    {
                        _visibleTargets.Add(target);
                        IsSeeing = true;
                    }
                }
            }
        }

        private void DrawFieldOfView()
        {
            int stepCount = Mathf.RoundToInt(_viewAngle * _meshResolution);

            float stepAngleSize = _viewAngle / stepCount;
            List<Vector3> viewPoints = new List<Vector3>();
            ViewCastInfo oldViewCast = new ViewCastInfo();

            for (int i = 0; i <= stepCount; i++)
            {
                float angle = transform.eulerAngles.y - _viewAngle / 2 + stepAngleSize * i;
                ViewCastInfo newViewCast = ViewCast(angle);

                if (i > 0)
                {
                    bool edgeDistanceThresholdExceeded = Mathf.Abs(oldViewCast.distance - newViewCast.distance) > _edgeDistanceThreshold;

                    if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDistanceThresholdExceeded))
                    {
                        EdgeInfo edge = FindCast(oldViewCast, newViewCast);
                        if (edge.pointA != Vector3.zero)
                            viewPoints.Add(edge.pointA);

                        if (edge.pointB != Vector3.zero)
                            viewPoints.Add(edge.pointB);
                    }

                }
                viewPoints.Add(newViewCast.point);
                oldViewCast = newViewCast;
            }

            var vertexCount = viewPoints.Count + 1;

            Vector3[] vertices = new Vector3[vertexCount];
            int[] triangles = new int[(vertexCount - 2) * 3];

            vertices[0] = Vector3.zero;

            for (int i = 0; i < vertexCount - 1; i++)
            {
                vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

                if (i < vertexCount - 2)
                {
                    triangles[i * 3] = 0;
                    triangles[i * 3 + 1] = i + 1;
                    triangles[i * 3 + 2] = i + 2;
                }
            }

            _viewMesh.Clear(); 
            _viewMesh.vertices = vertices;
            _viewMesh.triangles = triangles;
            _viewMesh.RecalculateNormals();
        }

        private EdgeInfo FindCast(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
        {
            float minAngle = minViewCast.angle;
            float maxAngle = maxViewCast.angle;
            Vector3 minPoint = Vector3.zero;
            Vector3 maxPoint = Vector3.zero;

            for (int i = 0; i < _edgeResolveIterations; i++)
            {
                float angle = (minAngle + maxAngle) / 2;
                ViewCastInfo newViewCast = ViewCast(angle);

                bool edgeDistanceThresholdExceeded = Mathf.Abs(minViewCast.distance - newViewCast.distance) > _edgeDistanceThreshold;
                if (newViewCast.hit == minViewCast.hit && !edgeDistanceThresholdExceeded)
                {
                    minAngle = angle;
                    minPoint = newViewCast.point;
                }
                else
                {
                    maxAngle = angle;
                    maxPoint = newViewCast.point;
                }
            }
            return new EdgeInfo(minPoint, maxPoint);
        }

        private ViewCastInfo ViewCast(float globalAngle)
        {
            Vector3 direction = DirectionFromAngle(globalAngle, true);
            RaycastHit hit;

            if (Physics.Raycast(transform.position, direction, out hit, _radiusView, _obstacleLayerMask))
                return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
            else
                return new ViewCastInfo(false, transform.position + direction * _radiusView, _radiusView, globalAngle);
        }

        public Vector3 DirectionFromAngle(float angleInDegrees, bool angleIsGlobal)
        {
            if (!angleIsGlobal)
                angleInDegrees += transform.eulerAngles.y;

            return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
        }

        public float GetRadiusView()
        {
            return _radiusView;
        }

        public float GetViewAngle()
        {
            return _viewAngle;
        }

        public List<Transform> GetVisibleTargets()
        {
            return _visibleTargets;
        }

        public void SetStatusIsSeeing(bool status)
        {
            IsSeeing = status;
        }

        public struct ViewCastInfo
        {
            public bool hit;
            public Vector3 point;
            public float distance;
            public float angle;

            public ViewCastInfo(bool hit, Vector3 point, float distance, float angle)
            {
                this.hit = hit;
                this.point = point;
                this.distance = distance;
                this.angle = angle;
            }
        }

        public struct EdgeInfo
        {
            public Vector3 pointA;
            public Vector3 pointB;

            public EdgeInfo(Vector3 pointA, Vector3 pointB)
            {
                this.pointA = pointA;
                this.pointB = pointB;
            }
        }
    }
}