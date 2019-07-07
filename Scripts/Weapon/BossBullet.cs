using Godot;

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

        public override void _Ready()
        {

        }

        public override void _Process(float delta)
        {

        }

        public override void _PhysicsProcess(float delta)
        {

        }

        #region External Functions

        public void SetAsBouncyBullet() => _isBouncyBullet = true;

        public void SetAsNormalBullet() => _isBouncyBullet = false;

        public void SetBulletAsExplosion() => _canExplode = true;

        public void SetBulletAsNonExplosion() => _canExplode = false;

        public void SetBulletHasTrail() => _hasTrail = true;

        public void SetBulletHasNoTrail() => _hasTrail = false;

        #endregion

        #region Overriden Parent

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
    }
}