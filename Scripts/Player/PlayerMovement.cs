using Godot;
using StormTime.Scene;
using StormTime.Utils;

namespace StormTime.Player
{
    public class PlayerMovement : KinematicBody2D
    {
        [Export] public float movementIncrementSpeed;
        [Export] public float maxMovementSpeed;
        [Export] public float turnSpeed;

        [Export] public NodePath backgroundManager;

        private Vector2 _movement;
        private float _rotation;
        private float _targetAngle;

        private MovingBackgroundManager _movingBackgroundManager;
        private Vector2 _maxMovementVector;

        public override void _Ready()
        {
            base._Ready();

            _movement = new Vector2();
            _maxMovementVector = new Vector2(maxMovementSpeed, maxMovementSpeed);
            _movingBackgroundManager = GetNode<MovingBackgroundManager>(backgroundManager);
        }

        public override void _Process(float delta)
        {
            base._Process(delta);
            RotatePlayer(delta);
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);
            MovePlayer(delta);
        }

        private void RotatePlayer(float delta)
        {
            Vector2 playerPosition = GetPosition();
            Vector2 mousePosition = GetViewport().GetMousePosition();

            float turnAngle =
                -Mathf.Rad2Deg(Mathf.Atan2(playerPosition.x - mousePosition.x, playerPosition.y - mousePosition.y));
            _targetAngle = ExtensionFunctions.To360Angle(turnAngle);
            _rotation = ExtensionFunctions.LerpAngleDeg(_rotation, _targetAngle, turnSpeed * delta);

            SetRotationDegrees(_rotation);
        }

        private void MovePlayer(float delta)
        {
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
            CheckMovementAndSetBackgroundRotation();
        }

        private void CheckMovementAndSetBackgroundRotation()
        {
            if (_movement.x == 0 && _movement.y == 0)
            {
                _movingBackgroundManager.DeActivateScrolling();
            }
            else
            {
                float movementAngle = -Mathf.Rad2Deg(Mathf.Atan2(-_movement.x, -_movement.y));
                movementAngle -= 90;
                PlayerVariables.PlayerStaticMovementRotation = movementAngle;
                _movingBackgroundManager.ActivateScrolling();
            }
        }
    }
}
