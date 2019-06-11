using Godot;

namespace StormTime.Common
{
    public class HealthDisplay : Node2D
    {
        [Export] public Color minHealthColor;
        [Export] public Color halfHealthColor;
        [Export] public Color maxHealthColor;
        [Export] public NodePath healthProgressNodePath;
        [Export] public NodePath healthSetterNodePath;

        private TextureProgress _healthProgress;
        private HealthSetter _healthSetter;

        private float _maxHealth;
        private float _currentHealth;

        public override void _Ready()
        {
            _healthProgress = GetNode<TextureProgress>(healthProgressNodePath);
            _healthSetter = GetNode<HealthSetter>(healthSetterNodePath);

            _healthSetter.healthChanged += HandleHealthChanged;
        }

        #region Utility Functions

        private void HandleHealthChanged(float currentHealth, float maxHealth)
        {
            _currentHealth = currentHealth;
            _maxHealth = maxHealth;

            _healthProgress.SetMax(_maxHealth);
            _healthProgress.SetValue(_currentHealth);

            float healthRatio = _currentHealth / _maxHealth;
            _healthProgress.SetTintProgress(healthRatio <= 0.5f
                ? minHealthColor.LinearInterpolate(halfHealthColor, healthRatio * 2)
                : halfHealthColor.LinearInterpolate(maxHealthColor, (healthRatio - 0.5f) * 2));
        }

        #endregion
    }
}