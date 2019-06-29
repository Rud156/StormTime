using Godot;
using StormTime.Player.Modifiers;
using StormTime.Player.Movement;
using StormTime.UI;
using StormTime.Utils;

namespace StormTime.Scene.MainScene
{
    public class PortalController : Node2D
    {
        [Export] public int soulsRequirement;
        [Export] public float minYPosition;
        [Export] public float maxYPosition;

        private bool _playerIsInside;
        private bool _playerInteracted;
        private PlayerController _playerController;

        public override void _Ready()
        {
            Connect("body_entered", this, nameof(HandlePlayerEntered));
            Connect("body_exited", this, nameof(HandlePlayerExited));

            PlaceAtRandomYPosition();
        }

        public override void _Process(float delta)
        {
            if (!_playerIsInside)
            {
                return;
            }

            if (Input.IsActionJustPressed(SceneControls.Interact))
            {
                CheckAndAllowPlayerEntry();
            }
        }

        #region Utility Functions

        private void PlaceAtRandomYPosition()
        {
            float randomYPosition = (float)GD.RandRange(minYPosition, maxYPosition);
            Vector2 currentPosition = GetGlobalPosition();
            SetGlobalPosition(new Vector2(currentPosition.x, randomYPosition));
        }

        private void CheckAndAllowPlayerEntry()
        {
            int currentSouls = PlayerModifierSoulsManager.instance.GetSoulsCount();
            if (currentSouls < soulsRequirement)
            {
                DialogueUiManager.instance.DisplaySingleStringTimed("Not Enough Souls to enter the doorway", 3);
            }
            else
            {
                DisablePlayerAndSwitchScene();
            }
        }

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