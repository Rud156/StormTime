using Godot;

namespace StormTime.UI
{
    public class ScaleBlinker : Control
    {
        [Export] public int scaleChangeCount;
        [Export] public float minScaleAmount;
        [Export] public float scaleChangeRate;

        private bool _isScalingActive;
        private int _currentScaleCount;
        private float _currentScaleAmount;
        private bool _isScaleIncreasing;

        public override void _Process(float delta)
        {
            if (!_isScalingActive)
            {
                return;
            }

            if (_isScaleIncreasing)
            {
                _currentScaleAmount += scaleChangeRate * delta;

                if (_currentScaleAmount >= 1)
                {
                    _isScaleIncreasing = false;
                    _currentScaleCount -= 1;
                }
            }
            else
            {
                _currentScaleAmount -= scaleChangeRate * delta;

                if (_currentScaleAmount <= minScaleAmount)
                {
                    _isScaleIncreasing = true;
                }
            }

            SetScale(Vector2.One * _currentScaleAmount);

            if (_currentScaleCount <= 0)
            {
                StopScaleBlinking();
            }
        }

        #region External Functions

        public void StartScaleBlinking()
        {
            if (_isScalingActive)
            {
                return;
            }

            _isScalingActive = true;
            _currentScaleCount = scaleChangeCount;
            _currentScaleAmount = 1;
            _isScaleIncreasing = false;
        }

        #endregion

        #region Utility Functions

        public void StopScaleBlinking()
        {
            _isScalingActive = false;
            SetScale(Vector2.One);
        }

        #endregion
    }
}