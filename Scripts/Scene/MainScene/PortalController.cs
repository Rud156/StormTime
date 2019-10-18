using Godot;
using StormTime.Player.Data;
using StormTime.Player.Modifiers;
using StormTime.Player.Movement;
using StormTime.UI;
using StormTime.Utils;

namespace StormTime.Scene.MainScene
{
    public class PortalController : Node2D
    {
        [Export] public int maxRevealCount;
        [Export] public NodePath portalCountDisplayLabelNodePath;
        [Export] public int soulsRequirement;
        [Export] public float minYPosition;
        [Export] public float maxYPosition;
        [Export] public string portalEnteredText;

        private bool _playerIsInside;
        private bool _playerInteracted;
        private PlayerController _playerController;

        private int _currentRevealCount;
        private Label _portalCountDisplayLabel;

        public override void _Ready()
        {
            Connect("body_entered", this, nameof(HandlePlayerEntered));
            Connect("body_exited", this, nameof(HandlePlayerExited));

            _currentRevealCount = maxRevealCount;

            _portalCountDisplayLabel = GetNode<Label>(portalCountDisplayLabelNodePath);
            _portalCountDisplayLabel.SetText($"X  {_currentRevealCount}");

            PlaceAtRandomYPosition();
        }

        public override void _Process(float delta)
        {
            CheckAndUpdateFlasherStatus();

            if (Input.IsActionJustPressed(SceneControls.Interact))
            {
                if (!_playerIsInside)
                {
                    MarkPositionInWorld();
                }
                else
                {
                    CheckAndAllowPlayerEntry();
                }
            }
        }

        private void CheckAndUpdateFlasherStatus()
        {
            bool isFlasherActive = FlasherObjectPointer.instance.IsFlashingActive(this);
            if (isFlasherActive)
            {
                Vector2 playerPosition = PlayerVariables.LastPlayerPosition;
                Vector2 currentPosition = GetGlobalPosition();
                float rotationAngle = -Mathf.Rad2Deg(Mathf.Atan2(currentPosition.x - playerPosition.x,
                    currentPosition.y - playerPosition.y)) + 90;
                FlasherObjectPointer.instance.SetRotation(rotationAngle, this);
            }
        }

        #region Utility Functions

        private void PlaceAtRandomYPosition()
        {
            float randomYPosition = (float)GD.RandRange(minYPosition, maxYPosition);
            Vector2 currentPosition = GetGlobalPosition();
            SetGlobalPosition(new Vector2(currentPosition.x, randomYPosition));
        }

        private void MarkPositionInWorld()
        {
            if (_currentRevealCount <= 0)
            {
                DialogueUiManager.instance.DisplaySingleStringTimed("No more reveals left", 3);
                return;
            }

            FlasherObjectPointer.instance.StartFlashing(this);
            _currentRevealCount -= 1;

            _portalCountDisplayLabel.SetText($"X  {_currentRevealCount}");
        }

        private void CheckAndAllowPlayerEntry()
        {
            int currentSouls = PlayerModifierSoulsManager.instance.GetSoulsCount();
            if (currentSouls < soulsRequirement)
            {
                DialogueUiManager.instance.DisplaySingleStringTimed(
                    $"Not Enough Souls to enter the doorway. (soulsRequirement)", 3);
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

            DialogueUiManager.instance.DisplaySingleString(portalEnteredText);
        }

        private void HandlePlayerExited(PhysicsBody2D other)
        {
            if (other.Name != TagManager.PlayerTag)
            {
                return;
            }

            DialogueUiManager.instance.ClearSingleDialogue();

            _playerIsInside = false;
            _playerController = null;
        }

        #endregion
    }
}