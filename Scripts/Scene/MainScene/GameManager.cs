using System;
using Godot;
using StormTime.Common;
using StormTime.Player.Data;
using StormTime.Player.Movement;
using StormTime.Player.Shooting;
using StormTime.UI;
using StormTime.Utils;

namespace StormTime.Scene.MainScene
{
    public class GameManager : Node
    {
        private const string BossScenePath = "res://Scenes/Boss.tscn";

        // Player Data Controls
        [Export] public NodePath playerControllerNodePath;
        [Export] public NodePath playerShooterNodePath;
        [Export] public NodePath playerHealthSetterNodePath;

        // Pause and Resume
        [Export] public NodePath pauseResumeControllerNodePath;

        private PlayerController _playerController;
        private PlayerShooting _playerShooting;
        private HealthSetter _playerHealthSetter;

        private PauseAndResume _pauseResumeController;

        private bool _playerInteractingWithShop;
        private bool _pauseMenuOpen;

        public override void _Ready()
        {
            GD.Randomize();

            if (instance == null)
            {
                instance = this;
            }

            _playerController = GetNode<PlayerController>(playerControllerNodePath);
            _playerShooting = GetNode<PlayerShooting>(playerShooterNodePath);
            _playerHealthSetter = GetNode<HealthSetter>(playerHealthSetterNodePath);

            _pauseResumeController = GetNode<PauseAndResume>(pauseResumeControllerNodePath);

            Fader.FaderReady faderActivate = null;
            faderActivate = () =>
           {
               Fader.instance.StartFading(false, new Color(1, 1, 1));
               Fader.faderReady -= faderActivate;
           };

            Fader.faderReady += faderActivate;
        }

        public override void _Process(float delta)
        {
            if (!Input.IsActionJustPressed(SceneControls.Cancel))
            {
                return;
            }

            if (_playerInteractingWithShop)
            {
                return;
            }

            if (_pauseMenuOpen)
            {
                _pauseResumeController.HidePauseMenu();
            }
            else
            {
                _pauseResumeController.ShowPauseMenu();
            }
        }

        #region External Functions

        public void PauseMenuOpened() => _pauseMenuOpen = true;

        public void PauseMenuClosed() => _pauseMenuOpen = false;

        public void PlayerEnemyShopInteractionStarted() => _playerInteractingWithShop = true;

        public void PlayerEnemyShopInteractionEnded() => _playerInteractingWithShop = false;

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