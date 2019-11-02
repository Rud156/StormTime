using Godot;

namespace StormTime.Weapon
{
    /// <summary>
    /// This is a special bullet that has damage based on life
    /// and the number of items it collided with
    /// </summary>
    public class ChargedBullet : Bullet
    {
        [Export] public float collisionLifeDecrement;

        private bool _isStatic;

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

            _collidingBodies = GetCollidingBodies();
            if (_collidingBodies.Count != 0 || _currentBulletTimeLeft <= 0)
            {
                if (_collidingBodies.Count != 0)
                {
                    NotifyCollider((Object)_collidingBodies[0]);

                    _currentBulletTimeLeft -= delta * collisionLifeDecrement;
                }

                if (_currentBulletTimeLeft <= 0 || !IsColliderEnemyAffector((Object)_collidingBodies[0]))
                {
                    SpawnBulletExplosion();
                    RemoveBulletFromTree();
                }
            }

            _currentBulletTimeLeft -= delta;
        }

        #region External Functions

        public void SetAsStaticBullet() => _isStatic = true;

        public void SetAsDynamicBullet() => _isStatic = false;

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