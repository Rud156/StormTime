using Godot;

namespace StormTime.Common
{
    public class HealthDisplay : Node2D
    {
        [Export] public Color minHealthColor;
        [Export] public Color halfHealthColor;
        [Export] public Color maxHealthColor;
        [Export] public NodePath healthProgressNodePath;
        [Export] public NodePath healthLabelDisplayNodePath;
        [Export] public NodePath healthSetterNodePath;

        private TextureProgress _healthProgress;
        private Label _healthLabel;
        private HealthSetter _healthSetter; // TODO: Remove this later on...

        private float _maxHealth;
        private float _currentHealth;

        public override void _Ready()
        {
            if (healthLabelDisplayNodePath != null)
            {
                _healthLabel = GetNode<Label>(healthLabelDisplayNodePath);
            }

            _healthProgress = GetNode<TextureProgress>(healthProgressNodePath);
            _healthSetter = GetNode<HealthSetter>(healthSetterNodePath);

            _healthSetter.healthChanged += HandleHealthChanged;
            HandleHealthChanged(_healthSetter.GetCurrentHealth(), _healthSetter.GetMaxHealth());
        }

        public override void _ExitTree() => _healthSetter.healthChanged -= HandleHealthChanged;

        #region Utility Functions

        private void HandleHealthChanged(float currentHealth, float maxHealth)
        {
            _currentHealth = currentHealth;
            _maxHealth = maxHealth;

            _healthProgress.SetMax(_maxHealth);
            _healthProgress.SetValue(_currentHealth);

            _healthLabel?.SetText($"{currentHealth} / {maxHealth}");

            float healthRatio = _currentHealth / _maxHealth;
            _healthProgress.SetTintProgress(healthRatio <= 0.5f
                ? minHealthColor.LinearInterpolate(halfHealthColor, healthRatio * 2)
                : halfHealthColor.LinearInterpolate(maxHealthColor, (healthRatio - 0.5f) * 2));
        }

        #endregion
    }
}