using Godot;

namespace StormTime.Common
{
    public class AreaCollisionChecker : Area2D
    {
        [Export] public NodePath circleCollisionShapeNodePath;

        private CollisionShape2D _circleCollisionShape;
        private bool _collisionCheckerInUse;

        public override void _Ready() =>
            _circleCollisionShape = GetNode<CollisionShape2D>(circleCollisionShapeNodePath);

        #region External Functions

        public void MoveShapeToPosition(Vector2 position) => SetGlobalPosition(position);

        public void SetCollisionRadius(float collisionRadius)
        {
            CircleShape2D collsionShape = (CircleShape2D)_circleCollisionShape.GetShape();
            collsionShape.SetRadius(collisionRadius);
        }

        public Godot.Collections.Array GetCollidingObjects() => GetOverlappingBodies();

        public void SetInUse() => _collisionCheckerInUse = true;

        public void SetFree() => _collisionCheckerInUse = false;

        public bool IsCollisionCheckerInUse() => _collisionCheckerInUse;

        #endregion
    }
}