using Godot;

namespace StormTime.UI
{
    public class Fader : TextureRect
    {
        [Export] public float faderRate;

        public delegate void FaderReady();
        public delegate void FadeInComplete();
        public delegate void FadeOutComplete();

        public static FaderReady faderReady;
        public FadeInComplete fadeInComplete;
        public FadeOutComplete fadeOutComplete;

        private bool _startFading;
        private bool _fadeIn;
        private Color _fadeColor;

        private float _currentAlpha;

        public override void _Ready()
        {
            if (instance == null)
            {
                instance = this;
            }

            faderReady?.Invoke();
        }

        public override void _Process(float delta)
        {
            if (!_startFading)
            {
                return;
            }

            if (_fadeIn)
            {
                _currentAlpha += faderRate * delta;

                if (_currentAlpha >= 1)
                {
                    fadeInComplete?.Invoke();
                    _startFading = false;
                }
            }
            else
            {
                _currentAlpha -= faderRate * delta;

                if (_currentAlpha <= 0)
                {
                    fadeOutComplete?.Invoke();
                    _startFading = false;
                }
            }

            _fadeColor.a = _currentAlpha;
            SetSelfModulate(_fadeColor);
        }


        #region External Functions

        public void StartFading(bool fadeIn, Color fadeColor)
        {
            _startFading = true;
            _fadeIn = fadeIn;
            _fadeColor = fadeColor;

            if (fadeIn)
            {
                _currentAlpha = 0;
            }
            else
            {
                _currentAlpha = 1;
            }
        }

        #endregion

        #region Singleton

        public static Fader instance;

        #endregion
    }
}