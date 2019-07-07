using Godot;
using StormTime.Utils;

namespace StormTime.Extensions
{
    public class ColorLerper : Sprite
    {
        [Export] public float lerpDuration;
        [Export] public Color[] colors;

        private int _currentLerpIndex;
        private Color _currentlyDisplayedColor;
        private Color _nextDisplayedColor;

        private float _lerpTimeExpired;
        private float _lerpedAmount;

        public override void _Ready()
        {
            _currentLerpIndex = 1;
            _currentlyDisplayedColor = colors[0];
            _nextDisplayedColor = colors[_currentLerpIndex];
        }

        public override void _Process(float delta)
        {
            _lerpTimeExpired += delta;
            _lerpedAmount = _lerpTimeExpired / lerpDuration;

            SetSelfModulate(_currentlyDisplayedColor.LinearInterpolate(_nextDisplayedColor, _lerpedAmount));

            if (_lerpedAmount >= 1)
            {
                _lerpedAmount = 1;
                SwitchToNextLerp();
            }
        }

        #region Utility Functions

        private void SwitchToNextLerp()
        {
            _lerpTimeExpired = 0;
            _currentlyDisplayedColor = _nextDisplayedColor;

            _currentLerpIndex += 1;
            _currentLerpIndex %= colors.Length;
            _nextDisplayedColor = colors[_currentLerpIndex];
        }

        #endregion
    }
}