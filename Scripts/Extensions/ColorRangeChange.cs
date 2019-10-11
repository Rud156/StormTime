using Godot;
using StormTime.Utils;

namespace StormTime.Extensions
{
    public class ColorRangeChange : Sprite
    {
        // Colors
        [Export] public Color startColor;
        [Export] public Color endColor;

        // Positions
        [Export] public float minYPosition;
        [Export] public float maxYPosition;

        private Color _currentColor;

        public override void _Ready() => _currentColor = startColor;

        public override void _Process(float delta)
        {
            float currentYPosition = GetGlobalPosition().y;
            float clampedYPosition = Mathf.Clamp(currentYPosition, minYPosition, maxYPosition);

            SetSelfModulate(_currentColor.LinearInterpolate(endColor,
                ExtensionFunctions.Map(clampedYPosition, minYPosition, maxYPosition, 0, 1)));
        }
    }
}