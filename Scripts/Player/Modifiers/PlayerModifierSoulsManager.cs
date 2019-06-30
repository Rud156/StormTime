using Godot;
using StormTime.Player.UIDisplay;

namespace StormTime.Player.Modifiers
{
    public class PlayerModifierSoulsManager : Node
    {
        #region Export Fields

        [Export] public int initialSoulsCount;
        [Export] public NodePath soulsLabelNodePath;
        [Export] public NodePath soulsAnimationControllerNodePath;

        #endregion

        public delegate void HandleStatusChanged(int currentSouls);

        public HandleStatusChanged handleStatusChanged;

        private Label _soulsLabel;
        private PlayerSoulsAnimationController _soulsAnimationController;

        private int _currentSoulsAmount;

        public override void _Ready()
        {
            if (instance == null)
            {
                instance = this;
            }

            _soulsLabel = GetNode<Label>(soulsLabelNodePath);
            _soulsAnimationController = GetNode<PlayerSoulsAnimationController>(soulsAnimationControllerNodePath);
            _currentSoulsAmount = initialSoulsCount;

            HandleSoulsChange();
        }

        #region External Functions

        public void DecrementSouls(int amount)
        {
            _currentSoulsAmount -= amount;

            if (_currentSoulsAmount < 0)
            {
                _currentSoulsAmount = 0;
            }

            HandleSoulsChange();
        }

        public void IncrementSouls(int amount)
        {
            _currentSoulsAmount += amount;
            HandleSoulsChange();
        }

        public bool HasSouls() => _currentSoulsAmount > 0;

        public int GetSoulsCount() => _currentSoulsAmount;

        #endregion

        #region Utility Functions

        private void HandleSoulsChange()
        {
            _soulsAnimationController.PlayActionAnimation();
            _soulsLabel.SetText($"X  {_currentSoulsAmount}");
            handleStatusChanged?.Invoke(_currentSoulsAmount);
        }

        #endregion

        #region Singleton

        public static PlayerModifierSoulsManager instance;

        #endregion
    }
}