using Godot;
using StormTime.Player.Modifiers;
using StormTime.Utils;
using StormTime.Weapon;

namespace StormTime.Player.Controllers
{

    public class PlayerShieldController : Area2D
    {
        // Prefabs
        [Export] public NodePath playerShieldNodePath;

        // Timer and Count
        [Export] public float playerShieldActiveTimer;
        [Export] public int playerShieldSoulCount;

        // Collision
        [Export] public int[] activeCollisionLayers;
        [Export] public int[] activeCollisionMasks;
        [Export] public int[] inActiveCollisionLayers;
        [Export] public int[] inActiveCollisionMasks;

        private Sprite _playerShield;

        private float _currentShieldActiveTimer;
        private float _shieldActiveTimeLeft;
        private bool _isShieldActive;

        public override void _Ready()
        {
            _playerShield = GetNode<Sprite>(playerShieldNodePath);
            _currentShieldActiveTimer = playerShieldActiveTimer;

            DeActivateShield();

            Connect("body_entered", this, nameof(HandlePhysicsObjectEntered));
        }

        public override void _ExitTree() => Disconnect("body_entered", this, nameof(HandlePhysicsObjectEntered));

        public override void _Process(float delta)
        {
            if (!_isShieldActive)
            {
                if (Input.IsActionJustPressed(SceneControls.Shield))
                {
                    CheckAndActivateShield();
                }
            }
            else
            {
                _shieldActiveTimeLeft -= delta;
                if (_shieldActiveTimeLeft <= 0)
                {
                    DeActivateShield();
                }
            }
        }

        #region External Functions

        public void BulletCollisionNotification(object bullet, bool isFreezingBullet)
        {
            // Don't do anything. Just destroy the bullet
            // Which it does automatically
        }

        public void CheckAndActivateShield()
        {
            if (!PlayerModifierSoulsManager.instance.HasNSouls(playerShieldSoulCount))
            {
                return;
            }

            _isShieldActive = true;
            _shieldActiveTimeLeft = _currentShieldActiveTimer;

            _playerShield.SetVisible(true);

            foreach (int collisionLayerBit in inActiveCollisionLayers)
            {
                SetCollisionLayerBit(collisionLayerBit, false);
            }
            foreach (int collisionMaskBit in inActiveCollisionMasks)
            {
                SetCollisionMaskBit(collisionMaskBit, false);
            }

            foreach (int collisionLayerBit in activeCollisionLayers)
            {
                SetCollisionLayerBit(collisionLayerBit, true);
            }
            foreach (int collisionMaskBit in activeCollisionMasks)
            {
                SetCollisionMaskBit(collisionMaskBit, true);
            }

            PlayerModifierSoulsManager.instance.DecrementSouls(playerShieldSoulCount);
        }

        public void IncrementShieldTimerByPercent(float percentAmount)
        {
            float shieldTimerIncreaseAmount = _currentShieldActiveTimer * percentAmount / 100;
            _currentShieldActiveTimer += shieldTimerIncreaseAmount;
        }

        public void DecrementShieldTimerByPercent(float percentAmount)
        {
            float shieldTimerDecreaseAmount = _currentShieldActiveTimer * percentAmount / 100;
            _currentShieldActiveTimer -= shieldTimerDecreaseAmount;
        }

        public bool IsShieldActive() => _isShieldActive;

        #endregion

        #region Utility Functions

        private void HandlePhysicsObjectEntered(PhysicsBody2D other)
        {
            if (!(other is EnemyBullet))
            {
                return;
            }

            EnemyBullet enemyBullet = (EnemyBullet)other;
            enemyBullet.ForceDestroyBullet();
        }

        private void DeActivateShield()
        {

            foreach (int collisionLayerBit in inActiveCollisionLayers)
            {
                SetCollisionLayerBit(collisionLayerBit, true);
            }
            foreach (int collisionMaskBit in inActiveCollisionMasks)
            {
                SetCollisionMaskBit(collisionMaskBit, true);
            }

            foreach (int collisionLayerBit in activeCollisionLayers)
            {
                SetCollisionLayerBit(collisionLayerBit, false);
            }
            foreach (int collisionMaskBit in activeCollisionMasks)
            {
                SetCollisionMaskBit(collisionMaskBit, false);
            }

            _isShieldActive = false;
            _playerShield.SetVisible(false);
        }

        #endregion
    }
}