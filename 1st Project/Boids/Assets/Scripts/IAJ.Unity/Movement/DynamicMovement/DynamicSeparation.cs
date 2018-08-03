using Assets.Scripts.IAJ.Unity.Util;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    public class DynamicSeparation : DynamicMovement
    {
        public float SeparationFactor { get; set; }
        public float Radius { get; set; }
        private List<DynamicCharacter> flock;

        private Vector3 direction, characterPosition;
        private float distance = 0, separationStrength = 0;
        private KinematicData boid;
        MovementOutput output = new MovementOutput();

        public DynamicSeparation(List<DynamicCharacter> f)
        {
            SeparationFactor = 5f;
            Radius = 5f;
            flock = f;
        }

        public override string Name
        {
            get { return "Separation"; }
        }


        public override MovementOutput GetMovement()
        {
            output.Clear();
            characterPosition = Character.Position;
            foreach (DynamicCharacter dynamicCharacter in flock)
            {
                boid = dynamicCharacter.KinematicData;
                direction = characterPosition - boid.Position;
                distance = direction.sqrMagnitude;
                if (distance < Radius * Radius)
                {
                    separationStrength = Mathf.Min(SeparationFactor / (distance), MaxAcceleration);
                    direction.Normalize();
                    output.linear += direction * separationStrength;
                }
            }

            if (output.linear.sqrMagnitude > MaxAcceleration * MaxAcceleration)
            {
                output.linear.Normalize();
                output.linear *= MaxAcceleration;
            }

            return output;
        }
    }
}