using Godot;
using StormTime.Common;
using StormTime.Player.Data;
using StormTime.Player.Movement;
using StormTime.Player.Shooting;
using StormTime.Utils;

namespace StormTime.Scene.MainScene
{
    public class GameManager : Node
    {
        private const string BossScenePath = "res://Scenes/Boss.tscn";

        [Export] public NodePath playerControllerNodePath;
        [Export] public NodePath playerShooterNodePath;
        [Export] public NodePath playerHealthSetterNodePath;

        private PlayerController _playerController;
        private PlayerShooting _playerShooting;
        private HealthSetter _playerHealthSetter;

        public override void _Ready()
        {
            if (instance == null)
            {
                instance = this;
            }

            _playerController = GetNode<PlayerController>(playerControllerNodePath);
            _playerShooting = GetNode<PlayerShooting>(playerShooterNodePath);
            _playerHealthSetter = GetNode<HealthSetter>(playerHealthSetterNodePath);
        }

        #region External Functions

        public void SwitchToBossScene() 
        {
            float movementSpeed = _playerController.GetPlayerMovementSpeed();
            float currentMaxHealth = _playerHealthSetter.GetMaxHealth();
            float currentDamageDiff = _playerShooting.GetShootingDamageDiff();

            PlayerVariables.PlayerCurrentMovementSpeed = movementSpeed;
            PlayerVariables.PlayerCurrentMaxHealth = currentMaxHealth;
            PlayerVariables.PlayerCurrentShootingDamageDiff = currentDamageDiff;

            GetTree().ChangeScene(BossScenePath);
        }

        #endregion

        #region Singleton

        public static GameManager instance;

        #endregion
    }
}