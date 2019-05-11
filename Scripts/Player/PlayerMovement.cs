using Godot;
using StormTime.Utils;

namespace StormTime.Player
{
    public class PlayerMovement : KinematicBody2D
    {
        [Export] public float movementIncrementSpeed;
        [Export] public float maxMovementSpeed;
        [Export] public float turnSpeed;

        private Vector2 _movement;
        private float _rotation;
        private float _targetAngle;

        public override void _Ready()
        {
            base._Ready();

            _movement = new Vector2();
        }

        public override void _Process(float delta)
        {
            base._Process(delta);

            Vector2 playerPosition = GetPosition();
            Vector2 mousePosition = GetViewport().GetMousePosition();

            float turnAngle =
                -Mathf.Rad2Deg(Mathf.Atan2(playerPosition.x - mousePosition.x, playerPosition.y - mousePosition.y));
            _targetAngle = ExtensionFunctions.To360Angle(turnAngle);
            _rotation = ExtensionFunctions.LerpAngleDeg(_rotation, _targetAngle, turnSpeed * delta);
            
            SetRotationDegrees(_rotation);
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            if (Input.IsActionPressed(SceneControls.Left))
                _movement.x -= movementIncrementSpeed * delta;
            else if (Input.IsActionPressed(SceneControls.Right))
                _movement.x += movementIncrementSpeed * delta;
            else
                _movement.x = 0;

            if (Mathf.Abs(_movement.x) > maxMovementSpeed)
                _movement.x = Mathf.Sign(_movement.x) * maxMovementSpeed;

            if (Input.IsActionPressed(SceneControls.Up))
                _movement.y -= movementIncrementSpeed * delta;
            else if (Input.IsActionPressed(SceneControls.Down))
                _movement.y += movementIncrementSpeed * delta;
            else
                _movement.y = 0;

            if (Mathf.Abs(_movement.y) > maxMovementSpeed)
                _movement.y = Mathf.Sign(_movement.y) * maxMovementSpeed;

            MoveAndSlide(_movement);
        }
    }
}
