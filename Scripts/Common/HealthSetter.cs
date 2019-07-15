using Godot;

namespace StormTime.Common
{
    public class HealthSetter : Node2D
    {
        [Export] public float maxHealth;

        private float _maxHealth;
        private float _currentHealth;
        private bool _zeroHealthEventFired;

        public delegate void HealthChanged(float currentHealth, float maxHealth);

        public delegate void ZeroHealth();

        public HealthChanged healthChanged;
        public ZeroHealth zeroHealth;

        public override void _Ready()
        {
            _maxHealth = maxHealth;
            _currentHealth = maxHealth;

            HandleHealthChange();
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
                : _currentHealth - amount;

            HandleHealthChange();
        }

        public void SetMaxHealth(float amount)
        {
            _maxHealth = amount;

            if (_currentHealth >= _maxHealth)
            {
                _currentHealth = _maxHealth;
            }

            HandleHealthChange();
        }

        public float GetCurrentHealth() => _currentHealth;

        public void ForceSetCurrentHealth(float amount)
        {
            _currentHealth = amount;
            HandleHealthChange();
        }

        public float GetMaxHealth() => _maxHealth;

        #endregion

        #region Utility Functions

        private void HandleHealthChange()
        {
            healthChanged?.Invoke(_currentHealth, _maxHealth);

            if (_currentHealth <= 0 && !_zeroHealthEventFired)
            {
                zeroHealth?.Invoke();
                _zeroHealthEventFired = true;
            }
        }

        #endregion
    }
}