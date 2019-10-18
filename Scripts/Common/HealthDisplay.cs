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

        public override void _Ready()
        {
            _healthProgress = GetNode<TextureProgress>(healthProgressNodePath);
            _healthSetter = GetNode<HealthSetter>(healthSetterNodePath);

            _healthSetter.healthChanged += HandleHealthChanged;
            HandleHealthChanged(_healthSetter.GetCurrentHealth(), _healthSetter.GetMaxHealth());
        }

        public override void _ExitTree() => _healthSetter.healthChanged -= HandleHealthChanged;

        #region Utility Functions

        private void HandleHealthChanged(float currentHealth, float maxHealth)
        {
            _healthProgress.SetMax(maxHealth);
            _healthProgress.SetValue(currentHealth);

            float healthRatio = currentHealth / maxHealth;
            _healthProgress.SetTintProgress(healthRatio <= 0.5f
                ? minHealthColor.LinearInterpolate(halfHealthColor, healthRatio * 2)
                : halfHealthColor.LinearInterpolate(maxHealthColor, (healthRatio - 0.5f) * 2));
        }

        #endregion
    }
}