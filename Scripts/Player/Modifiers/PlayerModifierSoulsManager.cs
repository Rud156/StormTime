using Godot;
using StormTime.Player.UIDisplay;

namespace StormTime.Player.Modifiers
{
    public class PlayerModifierSoulsManager : Node
    {
        private const int InitialSouls = 10;

        #region Export Fields

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
            _currentSoulsAmount = InitialSouls;

            HandleSoulsChange();
        }

        #region External Functions

        public void DecrementSouls(int amount)
        {
            _currentSoulsAmount -= amount;

            if (_currentSoulsAmount <= 0)
            {
                // TODO: Kill Player or Do Something
            }

            HandleSoulsChange();
        }

        public void IncrementSouls(int amount)
        {
            _currentSoulsAmount += amount;
            HandleSoulsChange();
        }

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