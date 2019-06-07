using Godot;
using StormTime.Player.Movement;
using StormTime.Scene.MainScene;
using StormTime.Utils;

namespace StormTime.Enemy.Groups
{
    public class EnemyGroupPlayerInteraction : Area2D
    {
        [Export] public NodePath parentGroupNodePath;

        private enum PlayerInteractionState
        {
            NotActive,
            Active,
            Ending,
            Completed
        }

        private EnemyGroup _parentGroup;

        private PlayerInteractionState _playerInteractionState;
        private bool _playerIsInside;
        private PlayerController _playerController;

        public override void _Ready()
        {
            _playerIsInside = false;
            _parentGroup = GetNode<EnemyGroup>(parentGroupNodePath);

            base.Connect("body_entered", this, "HandlePlayerEntry");
            base.Connect("body_exited", this, "HandlePlayerExit");
        }

        public override void _Process(float delta) => CheckPlayerInteraction(delta);

        private void CheckPlayerInteraction(float delta)
        {
            switch (_playerInteractionState)
            {
                case PlayerInteractionState.NotActive:
                    HandlePlayerInteractionNotActive(delta);
                    break;

                case PlayerInteractionState.Active:
                    HandlePlayerInteractionActive(delta);
                    break;

                case PlayerInteractionState.Ending:
                    HandlePlayerInteractionEnding(delta);
                    break;

                case PlayerInteractionState.Completed:
                    HandlePlayerInteractionComplete(delta);
                    break;
            }
        }

        #region Interaction State Updates

        private void HandlePlayerInteractionNotActive(float delta)
        {
            if (!_playerIsInside)
            {
                return;
            }

            if (Input.IsActionJustPressed(SceneControls.Interact))
            {
                if (_parentGroup.IsPlayerHostile())
                {
                    return;
                }

                _playerController.SetLerpPosition(GetGlobalPosition());
                _playerController.SetPlayerState(PlayerController.PlayerState.PlayerFloatingMovement);
                _playerController.DeActivateShooting();

                DialogueManager.Instance.StartRandomDialogueInteraction(_parentGroup);

                SetPlayerInteractionState(PlayerInteractionState.Active);
            }
        }

        private void HandlePlayerInteractionActive(float delta)
        {
            // TODO: Implement Dialogue Interaction Logic Here...
        }

        private void HandlePlayerInteractionEnding(float delta)
        {
            SetPlayerInteractionState(PlayerInteractionState.Completed);
            _playerController.SetPlayerState(PlayerController.PlayerState.PlayerInControlMovement);
            _playerController.ActivateShooting();
        }

        private void HandlePlayerInteractionComplete(float delta)
        {
            // TODO: Make sure that the player cannot interact with the world anymore...
        }

        #endregion

        #region Collision

        public void HandlePlayerEntry(PhysicsBody2D other)
        {
            if (other.Name != TagManager.PlayerTag)
            {
                return;
            }

            _playerIsInside = true;
            _playerController = (PlayerController)other;
        }

        public void HandlePlayerExit(PhysicsBody2D other)
        {
            if (other.Name != TagManager.PlayerTag)
            {
                return;
            }

            _playerIsInside = false;
            _playerController = null;
        }

        #endregion

        #region Utility Functions

        private void SetPlayerInteractionState(PlayerInteractionState playerInteractionState)
        {
            if (_playerInteractionState == playerInteractionState)
            {
                return;
            }

            _playerInteractionState = playerInteractionState;
        }

        #endregion
    }
}