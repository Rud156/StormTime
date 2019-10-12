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

        // Scaling for a more dramatic effect
        [Export] public float initialScaleAmount = 0.1f;
        [Export] public float finalScaleAmount = 1;
        [Export] public float scaleRate = 7;
        [Export] public bool useBulletScaling;

        private bool _isBouncyBullet;
        private bool _canExplode;
        private bool _hasTrail;

        private bool _scalingActive;
        private float _currentScaleAmount;

        public override void _Process(float delta)
        {
            if (_hasTrail)
            {
                HandleTrailSpawnAndUpdate(delta);
            }

            if (_scalingActive)
            {
                UpdateBulletScaling(delta);
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

        public override void LaunchBullet(Vector2 forwardVectorNormalized)
        {
            base.LaunchBullet(forwardVectorNormalized);

            if (useBulletScaling)
            {
                SetGlobalScale(Vector2.One * initialScaleAmount);
                _scalingActive = true;
                _currentScaleAmount = initialScaleAmount;
            }
            else
            {
                SetGlobalScale(Vector2.One * finalScaleAmount);
            }
        }

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

        private void UpdateBulletScaling(float delta)
        {
            if (_currentScaleAmount >= finalScaleAmount)
            {
                _scalingActive = false;
            }

            SetGlobalScale(_currentScaleAmount * Vector2.One);
            _currentScaleAmount += scaleRate * delta;
        }

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
                    if (collidingBody is PlayerController playerController)
                    {
                        playerController.TakeExternalDamage(explosionDamageAmount);
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