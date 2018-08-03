using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    public class DynamicArrive : DynamicVelocityMatch
    {
        public override string Name
        {
            get { return "Arrive"; }
        }

        public float StopRadius { get; set; }
        public float SlowRadius { get; set; }
        public float MaxSpeed { get; set; }

        private float distance = 0, targetSpeed = 0;

        public DynamicArrive()
        {
            this.Output = new MovementOutput();
        }

        public override MovementOutput GetMovement()
        {
            this.Output.linear = this.Target.Position - this.Character.Position;
            distance = this.Output.linear.magnitude;

            if (distance < StopRadius)
                targetSpeed = 0f;
            else if (distance > SlowRadius)
                targetSpeed = MaxSpeed;
            else
                targetSpeed = MaxSpeed * (distance / SlowRadius);

            Target.velocity = this.Output.linear.normalized * targetSpeed;
            return base.GetMovement();
        }
    }
}