using Godot;

namespace StormTime.UI
{
    public class FlasherObjectPointer : TextureRect
    {
        [Export] public float fadeRate = 10;
        [Export] public int flashCount = 3;

        private bool _startFlashing;

        private bool _isFadingIn;
        private float _currentAlpha;
        private int _flashedCount;
        private Color _flashColor;

        private Node _objectRequester;

        public override void _Ready()
        {
            if (instance == null)
            {
                instance = this;
            }

            _currentAlpha = 0;
            SetSelfModulate(new Color(1, 1, 1, _currentAlpha));
        }

        public override void _Process(float delta)
        {
            if (!_startFlashing)
            {
                return;
            }

            if (_isFadingIn)
            {
                _currentAlpha += fadeRate * delta;
                if (_currentAlpha >= 1)
                {
                    _isFadingIn = false;
                }
            }
            else
            {
                _currentAlpha -= fadeRate * delta;
                if (_currentAlpha <= 0)
                {
                    _isFadingIn = true;
                    _flashedCount += 1;
                }
            }

            if (_flashedCount >= flashCount)
            {
                _startFlashing = false;
                _objectRequester = null;
            }

            _flashColor.a = _currentAlpha;
            SetSelfModulate(_flashColor);
        }

        #region External Functions

        public void StartFlashing(Color flashColor, Node objectRequester)
        {
            _startFlashing = true;

            _flashedCount = 0;
            _currentAlpha = 0;
            _isFadingIn = true;
            _flashColor = flashColor;

            _objectRequester = objectRequester;

            flashColor.a = _currentAlpha;
            SetSelfModulate(flashColor);
        }

        public void StartFlashing(Node objectRequester) =>
            StartFlashing(new Color(1, 1, 1), objectRequester);

        public void SetRotation(float rotation, Node objectRequester)
        {
            if (_objectRequester != objectRequester)
            {
                return;
            }

            SetRotationDegrees(rotation);
        }

        public bool IsFlashingActive(Node objectRequester)
        {
            if (_objectRequester != objectRequester)
            {
                return false;
            }

            return _startFlashing;
        }

        #endregion

        #region Singleton

        public static FlasherObjectPointer instance;

        #endregion
    }
}