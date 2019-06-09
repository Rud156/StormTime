using Godot;
using StormTime.Player.Movement;
using StormTime.Utils;
using StormTime.Weapon;

namespace StormTime.Player.Shooting
{
    public class PlayerShooting : Node2D
    {
        [Export] public NodePath _playerBulletHolderNodePath;
        [Export] public PackedScene playerBulletPrefab;

        private Node2D _playerBulletHolder;
        private PlayerController _playerRoot;

        private bool _shootingActive;

        public override void _Ready()
        {
            _shootingActive = true;
            _playerBulletHolder = GetNode<Node2D>(_playerBulletHolderNodePath);
            _playerRoot = GetParent<PlayerController>();
        }

        public override void _Process(float delta)
        {
            if (!_shootingActive)
            {
                return;
            }

            if (Input.IsActionJustPressed(SceneControls.Shoot))
            {
                ShootBullet();
            }
        }

        private void ShootBullet()
        {
            Bullet bulletInstance = (Bullet)playerBulletPrefab.Instance();
            _playerBulletHolder.AddChild(bulletInstance);

            bulletInstance.SetGlobalPosition(_playerRoot.GetGlobalPosition());
            bulletInstance.LaunchBullet(_playerRoot.GetTransform().x);
        }

        #region External Functions

        public void ActivateShooting() => _shootingActive = true;

        public void DeActivateShooting() => _shootingActive = false;

        #endregion
    }
}