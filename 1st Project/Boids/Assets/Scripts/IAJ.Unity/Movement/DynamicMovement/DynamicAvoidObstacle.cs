using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{

    public class DynamicAvoidObstacle : DynamicSeek
    {
        public Collider collisionDetector;
        public float avoidDistance;
        public float lookAhead;

        private RaycastHit hit;
        private Vector3 orientationVector;
        private bool collision = false;
        private Ray ray = new Ray();

        public DynamicAvoidObstacle()
        {
            this.Target = new KinematicData();
        }

        public override string Name
        {
            get { return "AvoidObstacle"; }
        }

        public override MovementOutput GetMovement()
        {
            orientationVector = this.Character.GetOrientationAsVector();

            ray.origin = Character.Position;
            ray.direction = orientationVector;

            collision = collisionDetector.Raycast(ray, out hit, lookAhead);

            if (!collision)
            {
                return null;
            }
            else
            {
                this.Target.Position = hit.transform.position + hit.normal * avoidDistance;
            }

            return base.GetMovement();
        }
    }
}
