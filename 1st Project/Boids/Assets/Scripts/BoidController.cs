using Assets.Scripts.IAJ.Unity.Movement.Arbitration;
using Assets.Scripts.IAJ.Unity.Movement.DynamicMovement;
using Assets.Scripts.IAJ.Unity.Util;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoidController : MonoBehaviour {

    public const float X_WORLD_SIZE = 55;
    public const float Z_WORLD_SIZE = 32.5f;
    private const float MAX_ACCELERATION = 30.0f;
    private const float MAX_SPEED = 20.0f;
    private const float DRAG = 0.1f;

    private const float SEP_RADIUS = 8f;

    public DynamicCharacter character;
    private BlendedMovement blendedMovement;


    //early initialization
    void Awake()
    {
        this.character = new DynamicCharacter(this.gameObject);

        this.blendedMovement = new BlendedMovement
        {
            Character = this.character.KinematicData
        };

    }

    public void InitializeMovement(GameObject[] obstacles, List<DynamicCharacter> characters)
    {

        List<DynamicCharacter> otherCharacters = characters.Where(b => b != character).ToList();

        foreach (var obstacle in obstacles)
        {
            var avoid = new DynamicAvoidShort
            {
                Character = this.character.KinematicData,
                avoidDistance = 10.0f,
                lookAhead = 13.0f,
                collisionDetector = obstacle.GetComponent<Collider>(),
                MaxAcceleration = MAX_ACCELERATION,
                whiskerLength = 6.5f
            };

            this.blendedMovement.Movements.Add(new MovementWithWeight(avoid, 22.0f));
        }
        
        var cohesion = new DynamicCohesion(otherCharacters)
        {
            Character = this.character.KinematicData,
            MaxAcceleration = MAX_ACCELERATION,
            StopRadius = 5f,
            SlowRadius = 10f,
            MaxSpeed = this.character.MaxSpeed,
            Radius = 15f,
            FanAngle = MathConstants.MATH_PI_4
        };

        this.blendedMovement.Movements.Add(new MovementWithWeight(cohesion, 6.0f));

        var velocity = new FlockVelocityMatching(otherCharacters)
        {
            Character = this.character.KinematicData,
            MaxAcceleration = MAX_ACCELERATION,
            Radius = 20f,
            FanAngle = MathConstants.MATH_PI_4
        };

        this.blendedMovement.Movements.Add(new MovementWithWeight(velocity, 10.0f));

        var separation = new DynamicSeparation(otherCharacters)
        {
            Character = this.character.KinematicData,
            MaxAcceleration = MAX_ACCELERATION,
            Radius = SEP_RADIUS,
            SeparationFactor = MAX_ACCELERATION * 1.5f
        };

        this.blendedMovement.Movements.Add(new MovementWithWeight(separation, 12.0f));

        var wander = new DynamicStraightAhead
        {
            Character = this.character.KinematicData,
            MaxAcceleration = MAX_ACCELERATION
        };

        this.blendedMovement.Movements.Add(new MovementWithWeight(wander, 1.0f));

        var seekMouse = new DynamicSeekMouse
        {
            Character = this.character.KinematicData,
            MaxAcceleration = MAX_ACCELERATION,
            MaxSpeed = this.character.MaxSpeed,
            StopRadius = 1f,
            SlowRadius = 10f
        };

        this.blendedMovement.Movements.Add(new MovementWithWeight(seekMouse,4.0f));

        this.character.Movement = this.blendedMovement;
    }
    
    // Update is called once per frame
    void Update () {
        UpdateMovingGameObject();
	}

    private void UpdateMovingGameObject()
    {
        if (character.Movement != null)
        {
            character.Update();
            character.KinematicData.ApplyWorldLimit(X_WORLD_SIZE, Z_WORLD_SIZE);
        }
    }
}
