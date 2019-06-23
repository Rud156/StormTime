using Godot;
using StormTime.Utils;

namespace StormTime.UI
{
    public class WarningManager : TextureRect
    {
        private const float MaxPossibleAlpha = 255;

        [Export] public float fadeRate;
        [Export] public float minFadeAlpha;

        private Node _objectRequester;
        private Color _fadeColor;

        private float _currentAlpha;
        private bool _isFadeActive;
        private bool _isAlphaIncreasing;

        public override void _Ready()
        {
            if (instance == null)
            {
                instance = this;
            }

            ResetFaderState();
        }

        public override void _Process(float delta)
        {
            if (!_isFadeActive)
            {
                return;
            }

            UpdateFadingState(delta);
        }

        #region External Functions

        public void StartForcedWarning(Node requester, Color color)
        {
            _objectRequester = requester;
            _fadeColor = color;
            _isFadeActive = true;
        }

        public void StartWarning(Node requester, Color color)
        {
            if (_objectRequester != null)
            {
                return;
            }

            StartForcedWarning(requester, color);
        }

        public void StopWarning(Node requester)
        {
            if (_objectRequester != requester)
            {
                return;
            }

            ResetFaderState();
        }

        #endregion

        #region Utility Functions

        private void UpdateFadingState(float delta)
        {
            if (_currentAlpha <= minFadeAlpha)
            {
                _isAlphaIncreasing = true;
            }
            else if (_currentAlpha >= MaxPossibleAlpha)
            {
                _isAlphaIncreasing = false;
            }

            if (_isAlphaIncreasing)
            {
                _currentAlpha += fadeRate * delta;
            }
            else
            {
                _currentAlpha -= fadeRate * delta;
            }

            _fadeColor.a = _currentAlpha / MaxPossibleAlpha;
            SelfModulate = _fadeColor;
        }

        private void ResetFaderState()
        {
            _objectRequester = null;
            _isFadeActive = false;
            _currentAlpha = 0;
            SelfModulate = new Color(0, 0, 0, 0);
        }

        #endregion

        #region Singleton

        public static WarningManager instance;

        #endregion
    }
}