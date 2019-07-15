using Godot;
using StormTime.Common;
using StormTime.Player.Movement;

namespace StormTime.Weapon
{
    public class BossBullet : EnemyBullet
    {
        // Explosion Destruction on Death
        [Export] public float explosionRadius;
        [Export] public float explosionDamageAmount;

        private bool _isBouncyBullet;
        private bool _canExplode;
        private bool _hasTrail;

        public override void _Ready() => base._Ready();

        public override void _Process(float delta)
        {
            if (_hasTrail)
            {
                HandleTrailSpawnAndUpdate(delta);
            }
        }

        public override void _PhysicsProcess(float delta)
        {
            _collidingBodies = GetCollidingBodies();
            if (_collidingBodies.Count != 0)
            {
                if (!_isBouncyBullet)
                {
                    HandleBossBulletDestruction();
                }

                NotifyCollider((Object)_collidingBodies[0]);
            }
            else if (_currentBulletTimeLeft <= 0)
            {
                HandleBossBulletDestruction();
            }
        }

        #region External Functions

        public void SetAsBouncyBullet() => _isBouncyBullet = true;

        public void SetAsNormalBullet() => _isBouncyBullet = false;

        public void SetBulletAsExplosion() => _canExplode = true;

        public void SetBulletAsNonExplosion() => _canExplode = false;

        public void SetBulletHasTrail() => _hasTrail = true;

        public void SetBulletHasNoTrail() => _hasTrail = false;

        #endregion

        #region Overridden Parent

        protected override void SpawnBulletExplosion()
        {
            if (!_canExplode)
            {
                return;
            }
        }

        protected override void SpawnBulletTrail()
        {
            if (!_hasTrail)
            {
                return;
            }
        }

        #endregion

        #region Utility Functions

        private void HandleBossBulletDestruction()
        {
            if (_canExplode)
            {
                SpawnBulletExplosion();

                AreaCollisionChecker areaCollisionChecker = AreaCollisionManager.instance.GetAreaCollisionChecker();
                areaCollisionChecker.MoveShapeToPosition(GetGlobalPosition());
                areaCollisionChecker.SetCollisionRadius(explosionRadius);

                _collidingBodies = areaCollisionChecker.GetCollidingObjects();
                foreach (Object collidingBody in _collidingBodies)
                {
                    if (collidingBody is PlayerController)
                    {
                        ((PlayerController)collidingBody).TakeExternalDamage(explosionDamageAmount);
                        break;
                    }
                }

                AreaCollisionManager.instance.ReturnCollisionChecker(areaCollisionChecker);
            }

            RemoveBulletFromTree();
        }

        #endregion
    }
}