using Assets.Scripts.IAJ.Unity.Util;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    public class DynamicSeekMouse : DynamicArrive
    {
        private MovementOutput output = new MovementOutput();
        private Vector3 screenMousePosition, mousePosition;

        public DynamicSeekMouse()
        {
            this.Target = new KinematicData();
        }

        public override string Name
        {
            get { return "Seek Mouse"; }
        }

        public override MovementOutput GetMovement()
        {
            if (Input.GetMouseButton(0))
            {
                screenMousePosition = Input.mousePosition;
                screenMousePosition.z = Camera.main.transform.position.y;
                mousePosition = Camera.main.ScreenToWorldPoint(screenMousePosition);
                mousePosition.y = 0f;
                this.Target.Position = mousePosition;
            }
            else
            {
                return output;
            }
            return base.GetMovement();
        }
    }
}