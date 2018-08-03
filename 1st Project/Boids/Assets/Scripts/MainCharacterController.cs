using Assets.Scripts.IAJ.Unity.Util;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.IAJ.Unity.Movement.DynamicMovement;
using Assets.Scripts.IAJ.Unity.Movement.Arbitration;
using System.Collections.Generic;

public class MainCharacterController : MonoBehaviour {

    public const float X_WORLD_SIZE = 55;
    public const float Z_WORLD_SIZE = 32.5f;
    private const float MAX_ACCELERATION = 40.0f;
    private const float MAX_SPEED = 20.0f;
    private const float DRAG = 0.1f;
    
    


    public KeyCode stopKey = KeyCode.S;
    public KeyCode priorityKey = KeyCode.P;
    public KeyCode blendedKey = KeyCode.B;

    public GameObject movementText;
    public DynamicCharacter character;

    public PriorityMovement priorityMovement;
    public BlendedMovement blendedMovement;

    private Text movementTextText;

    //early initialization
    void Awake()
    {
        this.character = new DynamicCharacter(this.gameObject);
        this.movementTextText = this.movementText.GetComponent<Text>();

        this.priorityMovement = new PriorityMovement
        {
            Character = this.character.KinematicData
        };

        this.blendedMovement = new BlendedMovement
        {
            Character = this.character.KinematicData
        };
    }

    // Use this for initialization
    void Start ()
    {
    }

    public void InitializeMovement(GameObject[] obstacles, List<DynamicCharacter> characters)
    {
        foreach (var obstacle in obstacles)
        {
            //TODO: add your AvoidObstacle movement here
            var avoid = new DynamicAvoidShort
            {
                Character = this.character.KinematicData,
                avoidDistance = 5.0f,
                lookAhead = 15.0f,
                collisionDetector = obstacle.GetComponent<Collider>(),
                MaxAcceleration = MAX_ACCELERATION,
                whiskerLength = 7.5f
            };

            this.blendedMovement.Movements.Add(new MovementWithWeight(avoid, 10.0f));
            this.priorityMovement.Movements.Add(avoid);
        }

        foreach (var otherCharacter in characters)
        {
            if (otherCharacter != this.character)
            {
                //TODO: add your AvoidCharacter movement here
                var avoidCharacter = new DynamicAvoidCharacter(otherCharacter.KinematicData)
                {
                    Character = this.character.KinematicData,
                    maxAcceleration = MAX_ACCELERATION,
                    collisionRadius = 3f,
                    maxTimeLookAhead = 8f
                };

                this.blendedMovement.Movements.Add(new MovementWithWeight(avoidCharacter, 10.0f));
                this.priorityMovement.Movements.Add(avoidCharacter);
            }
        }

        /* TODO: add your wander behaviour here! */
        var wander = new DynamicWander
        {
            Character = this.character.KinematicData,
            WanderOffset = 5,
            WanderRate = MathConstants.MATH_1_PI,
            MaxAcceleration = MAX_ACCELERATION
        };

        this.priorityMovement.Movements.Add(wander);
        
        this.blendedMovement.Movements.Add(new MovementWithWeight(wander,1));
        this.character.Movement = this.blendedMovement;
    }


    void Update()
    {
        if (Input.GetKeyDown(this.stopKey))
        {
            this.character.Movement = null;
        }
        else if (Input.GetKeyDown(this.blendedKey))
        {
            this.character.Movement = this.blendedMovement;
        }
        else if (Input.GetKeyDown(this.priorityKey))
        {
            this.character.Movement = this.priorityMovement;
        }

        this.UpdateMovingGameObject();
        this.UpdateMovementText();
    }

    void OnDrawGizmos()
    {
        if (this.character != null && this.character.Movement != null)
        {
            foreach (DynamicMovement movement in this.priorityMovement.Movements)
            {
                if (movement != null && movement is DynamicWander)
                {
                    DynamicWander wander = (DynamicWander)movement;
                    Gizmos.color = Color.blue;
                    Gizmos.DrawWireSphere(wander.CircleCenter, wander.WanderRadius);
                }
            }
        }
    }

    private void UpdateMovingGameObject()
    {
        if (this.character.Movement != null)
        {
            this.character.Update();
            this.character.KinematicData.ApplyWorldLimit(X_WORLD_SIZE, Z_WORLD_SIZE);
        }
    }

    private void UpdateMovementText()
    {
        if (this.character.Movement == null)
        {
            this.movementTextText.text = this.name + " Movement: Stationary";
        }
        else
        {
            this.movementTextText.text = this.name + " Movement: " + this.character.Movement.Name;
        }
    } 

}
