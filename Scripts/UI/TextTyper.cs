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
                    if (_currentString.Length >= _displayString.Length)
                    {
                        _startTyping = false;
                    }
                    else
                    {
                        _currentTime = characterDelay;
                        _currentString.Append(_displayString[_currentString.Length + 1]);
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