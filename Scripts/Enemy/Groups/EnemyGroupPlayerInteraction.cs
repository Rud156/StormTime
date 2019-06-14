using System.Collections.Generic;
using Godot;
using StormTime.Player.Modifiers;
using StormTime.Player.Movement;
using StormTime.UI;
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

        private List<PlayerModifierTypes.SacrificialItem> _sacrificialItems;

        public override void _Ready()
        {
            _sacrificialItems = new List<PlayerModifierTypes.SacrificialItem>();

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

            if (_parentGroup.IsPlayerHostile())
            {
                return;
            }

            DisablePlayerAndShowDialogues();
            SetPlayerInteractionState(PlayerInteractionState.Active);
        }

        private void HandlePlayerInteractionActive(float delta)
        {
            PlayerModifierTypes.SacrificialItemInfo? itemInfo = null;
            int itemIndex = -1;

            if (Input.IsActionJustPressed(SceneControls.DialogueControl_1))
            {
                itemInfo = PlayerModifierTypes.GetSacrificialItemAffecter(_sacrificialItems[0]);
                itemIndex = 0;
            }
            else if (Input.IsActionJustPressed(SceneControls.DialogueControl_2))
            {
                itemInfo = PlayerModifierTypes.GetSacrificialItemAffecter(_sacrificialItems[1]);
                itemIndex = 1;
            }
            else if (Input.IsActionJustPressed(SceneControls.DialogueControl_3))
            {
                itemInfo = PlayerModifierTypes.GetSacrificialItemAffecter(_sacrificialItems[2]);
                itemIndex = 2;
            }

            if (itemInfo.HasValue)
            {
                _playerController.HandleSacrificialItemInfluence(itemInfo.Value);
                SetPlayerInteractionState(PlayerInteractionState.Ending);
            }
        }

        private void HandlePlayerInteractionEnding(float delta)
        {
            SetPlayerInteractionState(PlayerInteractionState.Completed);
            ClearDialogues();

            _playerController.SetPlayerState(PlayerController.PlayerState.PlayerInControlMovement);
            _playerController.ActivateShooting();
            _playerController.ResetSizeDefaults();

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

        private void DisablePlayerAndShowDialogues()
        {
            _playerController.SetLerpPosition(GetGlobalPosition());
            _playerController.SetPlayerState(PlayerController.PlayerState.PlayerFloatingMovement);
            _playerController.DeActivateShooting();

            _sacrificialItems.Clear();
            string[] sacrificialItemDescriptions = new string[3];

            HashSet<PlayerModifierTypes.SacrificialItem> currentSacrificialItems =
                new HashSet<PlayerModifierTypes.SacrificialItem>();

            for (int i = 0; i < sacrificialItemDescriptions.Length; i++)
            {
                PlayerModifierTypes.SacrificialItem sacrificialItem = PlayerModifierTypes.GetRandomSacrificialItem();
                while (currentSacrificialItems.Contains(sacrificialItem))
                {
                    sacrificialItem = PlayerModifierTypes.GetRandomSacrificialItem();
                }

                _sacrificialItems.Add(sacrificialItem);
                currentSacrificialItems.Add(sacrificialItem);

                sacrificialItemDescriptions[i] = PlayerModifierTypes.GetSacrificialItemDescription(_sacrificialItems[i]);
            }

            DialogueUiManager.instance.DisplayMultiDialogue(sacrificialItemDescriptions);
        }

        private void ClearDialogues() => DialogueUiManager.instance.ClearMultiDialogue();

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