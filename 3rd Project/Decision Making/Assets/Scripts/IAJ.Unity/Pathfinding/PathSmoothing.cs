using System.Linq;
using Assets.Scripts.IAJ.Unity.Pathfinding.Path;
using Assets.Scripts.IAJ.Unity.Utils;
using RAIN.Navigation.NavMesh;
using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts.IAJ.Unity.Pathfinding
{
    public class PathSmoothing
    {
        private static PathSmoothing instance;

        public static PathSmoothing Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PathSmoothing();
                }
                return instance;
            }
        }
        
        private List<Vector3> smoothedPositions = new List<Vector3>();
        private Vector3 lastAdded;
        private int count;
        private float distance;

        private const float ratio = 0.02f;  // 0.09999083f should work since it's the distance between nodes, but sometimes fails;
        private int mode; // 0 for sampling, 1 for ray, 2 for 1 / distance sampling
        private float distanceRatio;

        private Ray ray;
        private RaycastHit hit;

        private GlobalPath smoothedPath;

        private NavMeshPathGraph navMesh;
        private NavMeshPathGraph NavMesh
        {
            get
            {
                return navMesh;
            }
            set
            {
                if(navMesh == null)
                {
                    navMesh = value;
                }
            }
        }

        private void setMode(int _mode)
        {
            mode = _mode;
            if (mode < 0 || mode > 2)
                mode = 1;

            if (mode == 1)
            {
                ray = new Ray();
                hit = new RaycastHit();
            }
        }

        public GlobalPath SmoothPath(GlobalPath globalPath, int _mode = 1)
        {
            setMode(_mode);

            smoothedPath = new GlobalPath
            {
                IsPartial = globalPath.IsPartial
            };

            count = globalPath.PathPositions.Count;
            lastAdded = globalPath.PathPositions.First();
            smoothedPath.PathPositions.Add(lastAdded);

            for (int i = 0; i < count - 2; i++)
            {
                if (!CanBeDeleted(lastAdded, globalPath.PathPositions[i + 2]))
                {
                    smoothedPath.PathPositions.Add(globalPath.PathPositions[i + 1]);
                    lastAdded = globalPath.PathPositions[i + 1];
                }
            }

            smoothedPath.PathPositions.Add(globalPath.PathPositions.Last());
            return smoothedPath;

        }

        public List<Vector3> SmoothPath(List<Vector3> pathPositions, NavMeshPathGraph _navMesh, int _mode)
        {
            setMode(_mode);

            navMesh = _navMesh;
            smoothedPositions.Clear();
            count = pathPositions.Count;

            lastAdded = pathPositions.First();
            smoothedPositions.Add(lastAdded);

            for (int i = 0; i < count - 2; i++)
            {
                if (!CanBeDeleted(lastAdded, pathPositions[i + 2]))
                {
                    smoothedPositions.Add(pathPositions[i + 1]);
                    lastAdded = pathPositions[i + 1];
                }
            }

            smoothedPositions.Add(pathPositions.Last());
            return smoothedPositions;
        }

        private bool CanBeDeleted(Vector3 point, Vector3 secondPoint)
        {
            if(mode == 0)
            {
                for (float i = 0.0f; i < 1.0f; i += ratio)
                {
                    if ((navMesh.QuantizeToNode(Vector3.Lerp(point, secondPoint, i), 1.0f) == null))
                        return false;
                }
                return true;
            }

            else if(mode == 1)
            {
                ray.origin = point;
                ray.direction = (secondPoint - point).normalized;
                distance = Vector3.Distance(point, secondPoint);

                bool collision = Physics.Raycast(ray, out hit, distance);

                return collision ? false : true;
            }
            else if(mode == 2)
            {
                distanceRatio = 1 / Vector3.Distance(point, secondPoint);
                for (float i = 0.0f; i < 1.0f; i += distanceRatio)
                {
                    if ((navMesh.QuantizeToNode(Vector3.Lerp(point, secondPoint, i), 1.0f) == null))
                        return false;
                }
                return true;
            }
            else
            {
                throw new System.Exception("Mode doesn't exist exception");
            }
        }
    }
}