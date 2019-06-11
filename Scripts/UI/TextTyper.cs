using System.Text;
using Godot;

namespace StormTime.UI
{
    public class TextTyper : Label
    {
        [Export] public float characterDelay;

        private string _displayString;

        private bool _startTyping;
        private StringBuilder _currentString;
        private float _currentTime;

        private int _currentStringIndexCounter = -1;

        public override void _Ready()
        {
            _startTyping = false;
            _currentString = new StringBuilder();
            _displayString = string.Empty;
        }

        public override void _Process(float delta)
        {
            if (!_startTyping)
            {
                return;
            }

            if (_currentTime > 0)
            {
                _currentTime -= delta;

                if (_currentTime <= 0)
                {
                    if (_currentStringIndexCounter >= _displayString.Length - 1)
                    {
                        _startTyping = false;
                    }
                    else
                    {
                        _currentStringIndexCounter += 1;
                        _currentTime = characterDelay;
                        _currentString.Append(_displayString[_currentStringIndexCounter]);
                        SetText(_currentString.ToString());
                    }
                }
            }
        }

        #region External Functions

        public void DisplayString(string displayString)
        {
            _displayString = displayString;
            _startTyping = true;

            _currentString.Clear();
            _currentTime = characterDelay;
            _currentStringIndexCounter = -1;
        }

        public void ClearString()
        {
            _startTyping = false;
            _currentString.Clear();
            _displayString = string.Empty;

            SetText(_currentString.ToString());
        }

        #endregion
    }
}