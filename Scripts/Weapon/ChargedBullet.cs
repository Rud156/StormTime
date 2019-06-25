using Godot;
using StormTime.Utils;

namespace StormTime.Weapon
{
    /// <summary>
    /// This is a special bullet that has damage based on life
    /// and the number of items it collided with
    /// </summary>
    public class ChargedBullet : Bullet
    {
        [Export] public float minCollisionLifeDecrement;
        [Export] public float maxCollisionLifeDecrement;

        private bool _isStatic;

        private float _maxScalePossible;
        private float _currentScaleAmount;
        private float _collisionLifeDecrement;

        public override void _Process(float delta)
        {
            if (_isStatic)
            {
                return;
            }

            base._Process(delta);
        }

        public override void _PhysicsProcess(float delta)
        {
            if (_isStatic)
            {
                return;
            }

            KinematicCollision2D collision = MoveAndCollide(_launchVelocity * delta);
            if (collision != null || _currentBulletTimeLeft <= 0)
            {
                if (collision != null)
                {
                    SpawnBulletExplosion();
                    NotifyCollider(collision.Collider);

                    _currentBulletTimeLeft -= delta * maxCollisionLifeDecrement;
                }

                if (!IsColliderEnemyAffector(collision?.Collider) || _currentBulletTimeLeft <= 0)
                {
                    DestroyBullet();
                }
            }

            _currentBulletTimeLeft -= delta;
        }

        #region External Functions

        public void SetAsStaticBullet() => _isStatic = true;

        public void SetAsDynamicBullet() => _isStatic = false;

        public void SetMaxScale(float maxScale) => _maxScalePossible = maxScale;

        public void SetCurrentScale(float currentScale)
        {
            _currentScaleAmount = currentScale;
            _collisionLifeDecrement = ExtensionFunctions.Map(
                _currentScaleAmount, 0, _maxScalePossible,
                maxCollisionLifeDecrement, minCollisionLifeDecrement);
        }

        #endregion

        #region Utility Functions

        private bool IsColliderEnemyAffector(Object colliderObject)
        {
            if (colliderObject is Enemy.Individuals.Enemy ||
                colliderObject is Weapon.EnemyBullet)
            {
                return true;
            }

            return false;
        }

        #endregion
    }
}