using Godot;
using StormTime.Player.Movement;

namespace StormTime.Enemy.Individuals
{
    public class SlasherEnemyCollision : Area2D
    {
        // Player Damage
        [Export] public float playerDamagerPerTime;
        [Export] public float playerDamageTimer;

        private float _currentTimer;
        private bool _playerIsInCollisionRange;
        private bool _isInAttackState;

        private PlayerController _playerController;

        public override void _Ready()
        {
            Connect("body_entered", this, nameof(HandleAreaEntered));
            Connect("body_exited", this, nameof(HandleAreaExited));
        }

        public override void _ExitTree()
        {
            Disconnect("body_entered", this, nameof(HandleAreaEntered));
            Disconnect("body_exited", this, nameof(HandleAreaExited));
        }

        public override void _Process(float delta)
        {
            if (!_playerIsInCollisionRange || !_isInAttackState)
            {
                return;
            }

            UpdatePlayerDamage(delta);
        }

        #region External Functions

        public void SetAttackingState(bool attackState) => _isInAttackState = attackState;

        #endregion

        #region Utility Functions

        private void HandleAreaEntered(PhysicsBody2D other)
        {
            if (!(other is PlayerController))
            {
                return;
            }

            _playerController = (PlayerController)other;
            _playerIsInCollisionRange = true;
        }

        private void HandleAreaExited(PhysicsBody2D other)
        {
            if (!(other is PlayerController))
            {
                return;
            }

            _playerController = null;
            _playerIsInCollisionRange = false;
        }

        private void UpdatePlayerDamage(float delta)
        {
            _currentTimer -= delta;
            if (_currentTimer <= 0)
            {
                _currentTimer = playerDamageTimer;
                _playerController.TakeExternalDamage(playerDamagerPerTime);
            }
        }

        #endregion
    }
}