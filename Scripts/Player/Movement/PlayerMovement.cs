using Godot;
using StormTime.Player.Data;
using StormTime.Scene;
using StormTime.Utils;

namespace StormTime.Player.Movement
{
    public class PlayerMovement : KinematicBody2D
    {
        [Export] public float movementSpeed;

        private Vector2 _movement;

        public override void _Ready() => _movement = new Vector2();

        public override void _Process(float delta) => RotatePlayer(delta);

        public override void _PhysicsProcess(float delta) => MovePlayer(delta);

        private void RotatePlayer(float delta)
        {
            Vector2 mousePosition = GetGlobalMousePosition();
            LookAt(mousePosition);
        }

        private void MovePlayer(float delta)
        {
            // TODO: Remove this later on...
            if (Input.IsActionJustPressed(SceneControls.Interact))
            {
                GD.Print($"Player Position: {PlayerVariables.PlayerPosition}");
            }

            if (Input.IsActionPressed(SceneControls.Left))
                _movement.x = -movementSpeed;
            else if (Input.IsActionPressed(SceneControls.Right))
                _movement.x = movementSpeed;
            else
                _movement.x = 0;

            if (Input.IsActionPressed(SceneControls.Up))
                _movement.y = -movementSpeed;
            else if (Input.IsActionPressed(SceneControls.Down))
                _movement.y = movementSpeed;
            else
                _movement.y = 0;

            _movement = MoveAndSlide(_movement);
            PlayerVariables.PlayerPosition = GetPosition();
        }
    }
}
