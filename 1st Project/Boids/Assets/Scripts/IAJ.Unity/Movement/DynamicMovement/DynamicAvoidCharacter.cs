using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    public class DynamicAvoidCharacter : DynamicSeek
    {
        public float maxAcceleration;
        public float collisionRadius;
        public float maxTimeLookAhead;

        private Vector3 deltaPos, deltaVel, futureDeltaPos;
        private float deltaSpeed = 0, timeToClosest = 0, futureDistance = 0, twiceCollisionRadius;
        private MovementOutput output = new MovementOutput();

        public DynamicAvoidCharacter(KinematicData target)
        {
            this.Target = target;
            this.maxAcceleration = 0;
            this.collisionRadius = 5.0f;
            maxTimeLookAhead = 2.0f;
            twiceCollisionRadius = 2 * collisionRadius;
        }

        public override MovementOutput GetMovement()
        {
            output.Clear();
            deltaPos = Target.Position - Character.Position;
            deltaVel = Target.velocity - Character.velocity;
            deltaSpeed = deltaVel.sqrMagnitude;

            if (deltaSpeed == 0) return output;

            timeToClosest = (-Vector3.Dot(deltaPos, deltaVel)) / (deltaSpeed);

            if (timeToClosest > maxTimeLookAhead) return output;

            futureDeltaPos = deltaPos + deltaVel * timeToClosest;
            futureDistance = futureDeltaPos.sqrMagnitude;
            
            if (futureDistance > twiceCollisionRadius * twiceCollisionRadius) return output;

            if (futureDistance <= 0 || deltaPos.sqrMagnitude < twiceCollisionRadius * twiceCollisionRadius)
                output.linear = Character.Position - Target.Position;
            else
            {
                output.linear = futureDeltaPos * -1;
            }

            output.linear = output.linear.normalized * maxAcceleration;

            return output;
        }
    }
}
