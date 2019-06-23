using Godot;
using StormTime.Player.Modifiers;
using StormTime.Player.Movement;
using StormTime.Utils;
using StormTime.Weapon;

namespace StormTime.Player.Shooting
{
    public class PlayerShooting : Node2D
    {
        [Export] public NodePath playerBulletHolderNodePath;
        [Export] public PackedScene playerBulletPrefab;
        [Export] public float shootDelay;
        [Export] public int shootSoulDecrementCount = 1;

        private Node2D _playerBulletHolder;
        private PlayerController _playerRoot;

        private bool _shootingActive;
        private float _currentShootDelay;
        private float _currentShootTimeLeft;

        private float _currentDamageDiff; // Add this value to the original damage value of the bullet
        private float _currentDamageDiffPercent;

        public override void _Ready()
        {
            _shootingActive = true;
            _currentShootDelay = shootDelay;

            _playerBulletHolder = GetNode<Node2D>(playerBulletHolderNodePath);
            _playerRoot = GetParent<PlayerController>();
        }

        public override void _Process(float delta)
        {
            _currentShootTimeLeft -= delta;

            if (!_shootingActive)
            {
                return;
            }

            if (Input.IsActionJustPressed(SceneControls.Shoot) && _currentShootTimeLeft <= 0)
            {
                ShootBullet();
            }
        }

        private void ShootBullet()
        {
            Bullet bulletInstance = (Bullet)playerBulletPrefab.Instance();
            _playerBulletHolder.AddChild(bulletInstance);

            if (_currentDamageDiffPercent != 0)
            {
                float damageChange = bulletInstance.GetBulletDamage() * _currentDamageDiffPercent / 100;
                _currentDamageDiff += damageChange;
                _currentDamageDiffPercent = 0;
            }

            bulletInstance.SetBulletDamage(bulletInstance.GetBulletDamage() + _currentDamageDiff);
            bulletInstance.SetGlobalPosition(_playerRoot.GetGlobalPosition());
            bulletInstance.LaunchBullet(_playerRoot.GetTransform().x);

            _currentShootTimeLeft = _currentShootDelay;

            PlayerModifierSoulsManager.instance.DecrementSouls(shootSoulDecrementCount);
        }

        #region External Functions

        public void ActivateShooting() => _shootingActive = true;

        public void DeActivateShooting() => _shootingActive = false;

        public float GetCurrentShootingDelay() => _currentShootDelay;

        public void SetCurrentShootingDelay(float shootingDelay) => _currentShootDelay = shootingDelay;

        public void AddToDamageDifferencePercent(float percentIncrease) => _currentDamageDiffPercent += percentIncrease;

        #endregion
    }
}