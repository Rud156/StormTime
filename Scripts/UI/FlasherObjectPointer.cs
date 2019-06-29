using Godot;

namespace StormTime.UI
{
    public class FlasherObjectPointer : TextureRect
    {
        [Export] public float fadeRate = 10;
        [Export] public int flashCount = 3;

        private bool _startFlashing;
        private bool isFadingIn;
        private float _currentAlpha;
        private int _flashedCount;

        public override void _Process(float delta)
        {
            if (!_startFlashing)
            {
                return;
            }


        }

        #region External Functions

        public void StartFlashing()
        {
            _startFlashing = true;

            _flashedCount = 0;

        }

        #endregion

        #region Singleton



        #endregion
    }
}