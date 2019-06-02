using Godot;
using StormTime.Player.Data;
using StormTime.Utils;

namespace StormTime.Player.Movement
{
    public class PlayerController : KinematicBody2D
    {
        [Export] public float movementSpeed;
        [Export] public float defaultScaleAmount;

        // Float Effects
        [Export] public float minScaleAmount;
        [Export] public float maxScaleAmount;
        [Export] public float scaleChangeFrequency;
        [Export] public float rotationRate;
        [Export] public float lerpVelocity;

        public enum PlayerState
        {
            PlayerInControlMovement,
            PlayerFloatingMovement
        }

        private PlayerState _playerState;
        private Vector2 _movement;

        private Vector2 _targetScale;
        private Vector2 _lerpPosition;

        private float _playerTime;

        public override void _Ready()
        {
            _movement = new Vector2();
            _targetScale = Vector2.One * defaultScaleAmount;
            _lerpPosition = new Vector2();

            SetPlayerState(PlayerState.PlayerInControlMovement);
        }

        public override void _Process(float delta) => _playerTime += delta;

        public override void _PhysicsProcess(float delta)
        {
            SetScale(GetScale().LinearInterpolate(_targetScale, lerpVelocity * delta));

            switch (_playerState)
            {
                case PlayerState.PlayerInControlMovement:
                    HandlePlayerControlMovement(delta);
                    break;

                case PlayerState.PlayerFloatingMovement:
                    HandlePlayerFloatingMovement(delta);
                    break;
            }
        }

        #region Player Controls

        private void HandlePlayerControlMovement(float delta)
        {
            RotatePlayer(delta);
            MovePlayer(delta);
        }

        private void HandlePlayerFloatingMovement(float delta)
        {
            ConstantRotatePlayer(delta);
            FloatingScaleChange(delta);
            LerpPlayerToPosition(delta);
        }

        #region Player Control Movement

        private void RotatePlayer(float delta)
        {
            Vector2 mousePosition = GetGlobalMousePosition();
            LookAt(mousePosition);
        }

        private void MovePlayer(float delta)
        {
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
            PlayerVariables.PlayerPosition = GetGlobalPosition();
        }

        #endregion

        #region Player Floating Movement

        private void ConstantRotatePlayer(float delta) => Rotate(Mathf.Deg2Rad(rotationRate * delta));

        private void FloatingScaleChange(float delta)
        {
            float scaleMultiplier = Mathf.Sin(scaleChangeFrequency * _playerTime);
            float scaleAmount = ExtensionFunctions.Map(scaleMultiplier, -1, 1,
                minScaleAmount, maxScaleAmount);

            _targetScale = Vector2.One * scaleAmount;
        }

        private void LerpPlayerToPosition(float delta) => SetGlobalPosition(GetGlobalPosition().LinearInterpolate(_lerpPosition, lerpVelocity * delta));

        #endregion

        #endregion

        #region External Functions

        public void ResetSizeDefaults() => _targetScale = Vector2.One * defaultScaleAmount;

        public void SetLerpPosition(Vector2 position) => _lerpPosition = position;

        public void SetPlayerState(PlayerState playerState)
        {
            if (_playerState == playerState)
            {
                return;
            }

            _playerState = playerState;
        }

        #endregion
    }
}
