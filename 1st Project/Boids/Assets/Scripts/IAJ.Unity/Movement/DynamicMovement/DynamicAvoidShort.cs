using Assets.Scripts.IAJ.Unity.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{

    public class DynamicAvoidShort : DynamicSeek
    {
        public Collider collisionDetector;
        public float avoidDistance;
        public float lookAhead;
        public float whiskerLength;

        private Vector3 orientationVector, rightWhisker, leftWhisker;
        private Ray ray = new Ray(), ray_r = new Ray(), ray_l = new Ray();
        private RaycastHit hit, hit_r, hit_l;
        private MovementOutput output = new MovementOutput();

        public DynamicAvoidShort()
        {
            this.Target = new KinematicData();
            this.avoidDistance = 1.0f;
            this.lookAhead = 5.0f;
            collisionDetector = null;
        }

        public override string Name
        {
            get { return "AvoidObstacleShort"; }
        }

        public override MovementOutput GetMovement()
        {
           if (this.Character.velocity == Vector3.zero) return output;

            orientationVector = this.Character.velocity.normalized;
            rightWhisker = Quaternion.AngleAxis(30, Vector3.up) * orientationVector;
            leftWhisker  = Quaternion.AngleAxis(-30, Vector3.up) * orientationVector;

            ray.origin = ray_r.origin = ray_l.origin = Character.Position;
            ray.direction = orientationVector;
            ray_r.direction = rightWhisker.normalized;
            ray_l.direction = leftWhisker.normalized;

            bool collision = collisionDetector.Raycast(ray, out hit, lookAhead);
            bool collision_r = collisionDetector.Raycast(ray_r, out hit_r, whiskerLength);
            bool collision_l = collisionDetector.Raycast(ray_l, out hit_l, whiskerLength);


            if (collision && collision_r && collision_l)
                Target.Position = hit.point + hit.normal * (avoidDistance * 5);
            else
            {
                if (collision)
                {
                    Target.Position = hit.point + hit.normal * avoidDistance;
                }
                else if (collision_r)
                {
                    Target.Position = hit_r.point + hit_r.normal * (avoidDistance*3);
                }
                else if (collision_l)
                {
                    Target.Position = hit_l.point + hit_l.normal * (avoidDistance*3);
                }
                else return output;
            }

            return base.GetMovement();
        }
    }
}
