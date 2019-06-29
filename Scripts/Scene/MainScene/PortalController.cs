using Godot;
using StormTime.Player.Movement;
using StormTime.UI;
using StormTime.Utils;

namespace StormTime.Scene.MainScene
{
    public class PortalController : Node2D
    {
        private bool _playerIsInside;
        private bool _playerInteracted;
        private PlayerController _playerController;

        public override void _Ready()
        {
            Connect("body_entered", this, nameof(HandlePlayerEntered));
            Connect("body_exited", this, nameof(HandlePlayerExited));
        }

        public override void _Process(float delta)
        {
            if (!_playerIsInside)
            {
                return;
            }

            if (Input.IsActionJustPressed(SceneControls.Interact))
            {
                DisablePlayerAndSwitchScene();
            }
        }

        #region Utility Functions

        private void DisablePlayerAndSwitchScene()
        {
            if (_playerInteracted)
            {
                return;
            }

            _playerInteracted = true;
            Fader.instance.StartFading(true, new Color(1, 1, 1));
            Fader.instance.fadeInComplete += HandleFadeInComplete;

            _playerController.DeActivateShooting();
            _playerController.SetPlayerState(PlayerController.PlayerState.PlayerDisabled);
        }

        private void HandleFadeInComplete()
        {
            Fader.instance.fadeInComplete -= HandleFadeInComplete;
            GameManager.instance.SwitchToBossScene();
        }

        private void HandlePlayerEntered(PhysicsBody2D other)
        {
            if (other.Name != TagManager.PlayerTag)
            {
                return;
            }

            _playerIsInside = true;
            _playerController = (PlayerController)other;
        }

        private void HandlePlayerExited(PhysicsBody2D other)
        {
            if (other.Name != TagManager.PlayerTag)
            {
                return;
            }

            _playerIsInside = false;
            _playerController = null;
        }

        #endregion
    }
}