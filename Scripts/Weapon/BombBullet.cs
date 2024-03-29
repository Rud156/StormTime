using Godot;
using StormTime.Player.Controllers;

namespace StormTime.Weapon
{
    public class BombBullet : EnemyBullet
    {
        [Export] public float explosionRadius;
        [Export] public float explosionDamageAmount;
        [Export] public NodePath collisionShapeNodePath;
        [Export] public NodePath explosionCollisionAreaNodePath;

        private CollisionShape2D _collisionShape;
        private Area2D _explosionCollisionArea;

        private PlayerController _playerController;

        public override void _Ready()
        {
            _collisionShape = GetNode<CollisionShape2D>(collisionShapeNodePath);
            _explosionCollisionArea = GetNode<Area2D>(explosionCollisionAreaNodePath);

            _explosionCollisionArea.Connect("body_entered", this, nameof(HandleBodyEntered));
            _explosionCollisionArea.Connect("body_exited", this, nameof(HandleBodyExited));

            CircleShape2D collisionShape = (CircleShape2D)_collisionShape.GetShape();
            collisionShape.SetRadius(explosionRadius);
        }

        #region Overridden Parent

        protected override void RemoveBulletFromTree()
        {
            _explosionCollisionArea.Disconnect("body_entered", this, nameof(HandleBodyEntered));
            _explosionCollisionArea.Disconnect("body_exited", this, nameof(HandleBodyExited));

            Node2D explosionEffectInstance = (Node2D)bulletExplosionPrefab.Instance();
            GetParent().AddChild(explosionEffectInstance);

            explosionEffectInstance.SetGlobalPosition(GetGlobalPosition());

            _playerController?.TakeExternalDamage(explosionDamageAmount);

            base.RemoveBulletFromTree();
        }

        #endregion

        #region Utility Functions

        private void HandleBodyEntered(PhysicsBody2D other)
        {
            if (!(other is PlayerController))
            {
                return;
            }

            _playerController = (PlayerController)other;
        }

        private void HandleBodyExited(PhysicsBody2D other)
        {
            if (!(other is PlayerController))
            {
                return;
            }

            _playerController = null;
        }

        #endregion
    }
}