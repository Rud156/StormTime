using Godot;

namespace StormTime.Camera
{
    public class CameraFollowTarget : Camera2D
    {
        [Export] public NodePath targetNode;
        [Export] public bool useCameraLocking = true;
        [Export] public Vector2 lockBottomLeftCorner;
        [Export] public Vector2 lockTopRightCorner;

        private Node2D _target;
        private Vector2 _cameraPosition;

        public override void _Ready()
        {
            _cameraPosition = new Vector2();
            _target = GetNode<Node2D>(targetNode);
        }

        public override void _Process(float delta)
        {
            _cameraPosition = _target.GetPosition();

            if (useCameraLocking)
            {
                if (_cameraPosition.x < lockBottomLeftCorner.x)
                    _cameraPosition.x = lockBottomLeftCorner.x;
                else if (_cameraPosition.x > lockTopRightCorner.x)
                    _cameraPosition.x = lockTopRightCorner.x;

                if (_cameraPosition.y > lockBottomLeftCorner.y)
                    _cameraPosition.y = lockBottomLeftCorner.y;
                else if (_cameraPosition.y < lockTopRightCorner.y)
                    _cameraPosition.y = lockTopRightCorner.y;
            }

            SetPosition(_cameraPosition);
        }
    }
}