using Assets.Scripts.IAJ.Unity.Util;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    public class DynamicCohesion : DynamicArrive
    {
        public float FanAngle { get; set; }
        public float Radius { get; set; }
        private List<DynamicCharacter> flock;

        private Vector3 direction, characterPosition, boidPosition, massCenter = new Vector3();
        private uint closeBoids = 0;
        private KinematicData boid;
        private float angle = 0, angleDifference = 0;
        private MovementOutput output = new MovementOutput();

        public DynamicCohesion(List<DynamicCharacter> f)
        {
            FanAngle = 5f;
            Radius = 5f;
            flock = f;
            this.Target = new KinematicData();
        }

        public override string Name
        {
            get { return "Cohesion"; }
        }


        public override MovementOutput GetMovement()
        {
            massCenter.Set(0, 0, 0);
            closeBoids = 0;
            characterPosition = Character.Position;

            foreach (DynamicCharacter dynamicCharacter in flock)
            {
                boid = dynamicCharacter.KinematicData;
                boidPosition = boid.Position;
                direction = boidPosition - characterPosition;
                if (direction.sqrMagnitude <= Radius * Radius)
                {
                    angle = MathHelper.ConvertVectorToOrientation(direction);
                    angleDifference = MathHelper.ShortestAngleDifference(Character.Orientation, angle);
                    if (Mathf.Abs(angleDifference) <= FanAngle)
                    {
                        massCenter += boidPosition;
                        closeBoids++;
                    }
                }
            }

            if (closeBoids == 0) return output;

            massCenter /= closeBoids;
            Target.Position = massCenter;
            return base.GetMovement();
        }
    }
}