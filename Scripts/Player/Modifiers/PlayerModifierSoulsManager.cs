using Godot;

namespace StormTime.Player.Modifiers
{
    public class PlayerModifierSoulsManager : Node2D
    {
        private const int InitialSouls = 10;

        public delegate void HandleStatusChanged(int currentSouls);

        public HandleStatusChanged handleStatusChanged;

        private int _currentSoulsSystem;

        public override void _Ready()
        {
            if (instance == null)
            {
                instance = this;
            }

            _currentSoulsSystem = InitialSouls;
            HandleSoulsChange();
        }

        #region External Functions

        public void DecrementSouls(int amount)
        {
            _currentSoulsSystem += amount;
            HandleSoulsChange();
        }

        public void IncrementSouls(int amount)
        {
            _currentSoulsSystem = amount;
            HandleSoulsChange();
        }

        #endregion

        #region Utility Functions

        private void HandleSoulsChange() => handleStatusChanged?.Invoke(_currentSoulsSystem);

        #endregion

        #region Singleton

        public static PlayerModifierSoulsManager instance;

        #endregion
    }
}