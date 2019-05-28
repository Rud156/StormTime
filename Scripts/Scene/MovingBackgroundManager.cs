using Godot;
using StormTime.Player.Data;

namespace StormTime.Scene
{
    public class MovingBackgroundManager : Node2D
    {
        // Image Paths
        [Export] public NodePath backgroundSprite_1;
        [Export] public NodePath backgroundSprite_2;
        [Export] public NodePath backgroundSprite_3;
        [Export] public NodePath backgroundSprite_4;

        // Movement Data
        [Export] public float maxMovementSpeed;
        [Export] public float speedIncrementRate;
        [Export] public Vector2 textureSize;
        [Export] public float cameraBufferSize;

        // Sprites
        private Sprite _background_1;
        private Sprite _background_2;
        private Sprite _background_3;
        private Sprite _background_4;

        // These are effectively 4 squares
        // arranged end to end
        // 2 1
        // 3 4
        private Vector2 b1, b2, b3, b4;

        private float _currentSpeed;
        private bool _isBackgroundScrollingActive;

        public override void _Ready()
        {
            base._Ready();

            _background_1 = GetNode<Sprite>(backgroundSprite_1);
            _background_2 = GetNode<Sprite>(backgroundSprite_2);
            _background_3 = GetNode<Sprite>(backgroundSprite_3);
            _background_4 = GetNode<Sprite>(backgroundSprite_4);

            b1 = _background_1.GetGlobalPosition();
            b2 = _background_2.GetGlobalPosition();
            b3 = _background_3.GetGlobalPosition();
            b4 = _background_4.GetGlobalPosition();
        }

        public override void _Process(float delta)
        {
            base._Process(delta);

            if (!_isBackgroundScrollingActive)
                return;

            if (_currentSpeed < maxMovementSpeed)
            {
                _currentSpeed += speedIncrementRate * delta;

                if (_currentSpeed > maxMovementSpeed)
                    _currentSpeed = maxMovementSpeed;
            }
            else if (_currentSpeed > maxMovementSpeed)
            {
                _currentSpeed -= speedIncrementRate * delta;

                if (_currentSpeed < maxMovementSpeed)
                    _currentSpeed = maxMovementSpeed;
            }

            float yDirection = Mathf.Sin(Mathf.Deg2Rad(PlayerVariables.PlayerStaticMovementRotation));
            float xDirection = Mathf.Cos(Mathf.Deg2Rad(PlayerVariables.PlayerStaticMovementRotation));

            b1.y -= _currentSpeed * delta * yDirection;
            b2.y -= _currentSpeed * delta * yDirection;
            b3.y -= _currentSpeed * delta * yDirection;
            b4.y -= _currentSpeed * delta * yDirection;

            b1.x -= _currentSpeed * delta * xDirection;
            b2.x -= _currentSpeed * delta * xDirection;
            b3.x -= _currentSpeed * delta * xDirection;
            b4.x -= _currentSpeed * delta * xDirection;

            SetLeftAndDownMovement();
            SetUpAndRightMovement();

            _background_1.SetGlobalPosition(b1);
            _background_2.SetGlobalPosition(b2);
            _background_3.SetGlobalPosition(b3);
            _background_4.SetGlobalPosition(b4);


        }

        #region External Functions

        public void ActivateScrolling()
        {
            _isBackgroundScrollingActive = true;
        }

        public void DeActivateScrolling()
        {
            _isBackgroundScrollingActive = false;
            _currentSpeed = 0;
        }

        #endregion

        #region Inner Functions

        private void SetLeftAndDownMovement()
        {
            float textureWidth = textureSize.x;
            float textureHeight = textureSize.y;

            if (b1.y + textureHeight <= -cameraBufferSize)
            {
                b1.y = b4.y + textureHeight;
            }

            if (b2.y + textureHeight <= -cameraBufferSize)
            {
                b2.y = b3.y + textureHeight;
            }

            if (b3.y + textureHeight <= -cameraBufferSize)
            {
                b3.y = b2.y + textureHeight;
            }

            if (b4.y + textureHeight <= -cameraBufferSize)
            {
                b4.y = b1.y + textureHeight;
            }

            if (b1.x + textureWidth <= -cameraBufferSize)
            {
                b1.x = b2.x + textureWidth;
            }

            if (b2.x + textureWidth <= -cameraBufferSize)
            {
                b2.x = b1.x + textureWidth;
            }

            if (b3.x + textureWidth <= -cameraBufferSize)
            {
                b3.x = b4.x + textureWidth;
            }

            if (b4.x + textureWidth <= -cameraBufferSize)
            {
                b4.x = b3.x + textureWidth;
            }

        }

        private void SetUpAndRightMovement()
        {
            float textureWidth = textureSize.x;
            float textureHeight = textureSize.y;

            if (b1.y > -cameraBufferSize + textureHeight)
            {
                b1.y = b4.y - textureHeight;
            }

            if (b2.y > -cameraBufferSize + textureHeight)
            {
                b2.y = b3.y - textureHeight;
            }

            if (b3.y > -cameraBufferSize + textureHeight)
            {
                b3.y = b2.y - textureHeight;
            }

            if (b4.y > -cameraBufferSize + textureHeight)
            {
                b4.y = b1.y - textureHeight;
            }

            if (b1.x > -cameraBufferSize + textureWidth)
            {
                b1.x = b2.x - textureWidth;
            }

            if (b2.x > -cameraBufferSize + textureWidth)
            {
                b2.x = b1.x - textureWidth;
            }

            if (b3.x > -cameraBufferSize + textureWidth)
            {
                b3.x = b4.x - textureWidth;
            }

            if (b4.x > -cameraBufferSize + textureWidth)
            {
                b4.x = b3.x - textureWidth;
            }
        }

        #endregion
    }
}
