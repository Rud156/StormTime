using Godot;
using StormTime.Player.Modifiers;

namespace StormTime.Player.Controllers
{

    public class PlayerShieldController : RigidBody2D
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
        }

        #region External Functions

        public void BulletCollisionNotification(object bullet, bool isFreezingBullet)
        {
            // Don't do anything. Just destroy the bullet
            // Which it does automatically
        }

        public void ActivateShield()
        {
            if (!PlayerModifierSoulsManager.instance.HasNSouls(playerShieldSoulCount))
            {
                return;
            }

            PlayerModifierSoulsManager.instance.DecrementSouls(playerShieldSoulCount);
        }

        public void IncrementShieldTimer()
        {

        }

        #endregion

        #region Utility Functions

        private void DeActivateShield()
        {

        }

        #endregion
    }
}