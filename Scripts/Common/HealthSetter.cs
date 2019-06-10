using Godot;

namespace StormTime.Common
{
    public class HealthSetter : Node2D
    {
        [Export] public float maxHealth;

        private float _maxHealth;
        private float _currentHealth;

        public override void _Ready()
        {
            _maxHealth = maxHealth;
            _currentHealth = maxHealth;
        }

        #region External Functions

        public void AddHealth(float amount)
        {
            _currentHealth = _currentHealth + amount > _maxHealth
                ? _maxHealth
                : _currentHealth + amount;

            HandleHealthChange();
        }

        public void SubtractHealth(float amount)
        {
            _currentHealth = _currentHealth - amount <= 0
                ? 0
                : _currentHealth
                  - amount;

            HandleHealthChange();
        }

        public float GetCurrentHealth() => _currentHealth;

        public float GetMaxHealth() => _maxHealth;

        public void SetMaxHealth(float amount) => _maxHealth = amount;

        #endregion

        #region Utility Functions

        public void HandleHealthChange()
        {

        }

        #endregion
    }
}