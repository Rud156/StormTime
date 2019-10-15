using Godot;

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

        private bool _canExplode;
        private bool _hasTrail;
        private bool _isStatic;

        private bool _scalingActive;
        private float _currentScaleAmount;

        public override void _Process(float delta)
        {
            if (_isStatic)
            {
                return;
            }

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
            if (_isStatic)
            {
                return;
            }

            _collidingBodies = GetCollidingBodies();
            if (_collidingBodies.Count != 0)
            {
                HandleBossBulletDestruction();
                NotifyCollider((Object)_collidingBodies[0]);
            }
            else if (_currentBulletTimeLeft <= 0)
            {
                HandleBossBulletDestruction();
            }
        }

        #region External Functions

        public void SetBulletAsExplosion() => _canExplode = true;

        public void SetBulletAsNonExplosion() => _canExplode = false;

        public void SetBulletHasTrail() => _hasTrail = true;

        public void SetBulletHasNoTrail() => _hasTrail = false;

        public void SetAsStaticBullet() => _isStatic = true;

        public void SetAsDynamicBullet() => _isStatic = false;

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
            }

            RemoveBulletFromTree();
        }

        #endregion
    }
}