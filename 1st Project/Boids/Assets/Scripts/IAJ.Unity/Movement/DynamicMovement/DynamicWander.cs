using Assets.Scripts.IAJ.Unity.Util;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{

    public class DynamicWander : DynamicSeek
    {
        public float WanderOffset { get; set; }
        public float WanderRadius { get; set; }
        public float WanderRate { get; set; }

        public Vector3 CircleCenter { get; private set; }

        public GameObject DebugTarget { get; set; }

        protected float WanderOrientation { get; set; }


        public DynamicWander()
        {
            this.Target = new KinematicData();
            this.WanderOrientation = 0;
            this.WanderRadius = 5.0f;
        }

        public override string Name
        {
            get { return "Wander"; }
        }


        public override MovementOutput GetMovement()
        {
            WanderOrientation += RandomHelper.RandomBinomial() * WanderRate;
            Target.Orientation = WanderOrientation + Character.Orientation;
            CircleCenter = Character.Position + WanderOffset * MathHelper.ConvertOrientationToVector(Character.Orientation);
            Target.Position = CircleCenter + WanderRadius * MathHelper.ConvertOrientationToVector(Target.Orientation);
            return base.GetMovement();
        }
    }
}