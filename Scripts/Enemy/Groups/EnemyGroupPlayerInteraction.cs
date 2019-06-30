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
        private bool _isShop;

        private PlayerInteractionState _playerInteractionState;
        private bool _playerIsInside;
        private PlayerController _playerController;

        private List<PlayerModifierTypes.SacrificialItem> _sacrificialItems;
        private List<PlayerModifierTypes.ShopItem> _shopItems;

        public override void _Ready()
        {
            _sacrificialItems = new List<PlayerModifierTypes.SacrificialItem>();
            _shopItems = new List<PlayerModifierTypes.ShopItem>();

            _playerIsInside = false;
            _parentGroup = GetNode<EnemyGroup>(parentGroupNodePath);

            Connect("body_entered", this, nameof(HandlePlayerEntry));
            Connect("body_exited", this, nameof(HandlePlayerExit));
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
            if (_isShop)
            {
                HandleShopPlayerInteraction();
            }
            else
            {
                HandleSacrificialItemPlayerInteraction();
            }
        }

        private void HandlePlayerInteractionEnding(float delta)
        {
            SetPlayerInteractionState(PlayerInteractionState.Completed);
            ResetPlayerAndDialogues();
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

            if (_isShop)
            {
                ShowShopItemDialogues();
            }
            else
            {
                ShowSacrificialItemDialogues();
            }
        }

        private void ResetPlayerAndDialogues()
        {
            ClearDialogues();

            _playerController.SetPlayerState(PlayerController.PlayerState.PlayerInControlMovement);
            _playerController.ActivateShooting();
            _playerController.ResetSizeDefaults();
        }

        #region Shop Interactions

        private void ShowShopItemDialogues()
        {
            _shopItems.Clear();
            string[] shopItemDescriptions = new string[3];

            HashSet<PlayerModifierTypes.ShopItem> currentShopItems = new HashSet<PlayerModifierTypes.ShopItem>();

            for (int i = 0; i < shopItemDescriptions.Length; i++)
            {
                PlayerModifierTypes.ShopItem shopItem = PlayerModifierTypes.GetRandomShopItem();
                while (currentShopItems.Contains(shopItem))
                {
                    shopItem = PlayerModifierTypes.GetRandomShopItem();
                }

                _shopItems.Add(shopItem);
                currentShopItems.Add(shopItem);

                shopItemDescriptions[i] = PlayerModifierTypes.GetShopItemDescription(shopItem);
            }

            DialogueUiManager.instance.DisplayMultiDialogue(shopItemDescriptions);
        }

        private void HandleShopPlayerInteraction()
        {
            PlayerModifierTypes.ShopItemInfo? shopItemInfo = null;

            if (Input.IsActionJustPressed(SceneControls.DialogueControl_1))
            {
                shopItemInfo = PlayerModifierTypes.GetShopItem(_shopItems[0]);
            }
            else if (Input.IsActionJustPressed(SceneControls.DialogueControl_2))
            {
                shopItemInfo = PlayerModifierTypes.GetShopItem(_shopItems[1]);
            }
            else if (Input.IsActionJustPressed(SceneControls.DialogueControl_3))
            {
                shopItemInfo = PlayerModifierTypes.GetShopItem(_shopItems[2]);
            }
            else if (Input.IsActionJustPressed(SceneControls.Cancel))
            {
                ResetPlayerAndDialogues();
                SetPlayerInteractionState(PlayerInteractionState.NotActive);
                _playerIsInside = false;
            }

            if (shopItemInfo.HasValue)
            {
                int soulsCost = PlayerModifierTypes.GetShopItemCost(shopItemInfo.Value.shopItem);

                if (PlayerModifierSoulsManager.instance.GetSoulsCount() >= soulsCost)
                {
                    PlayerModifierSoulsManager.instance.DecrementSouls(soulsCost);
                    _playerController.HandleShopItemInfluence(shopItemInfo.Value);
                    _parentGroup.SetPlayerAsHostile();
                    SetPlayerInteractionState(PlayerInteractionState.Ending);
                }
                else
                {
                    ResetPlayerAndDialogues();
                    DialogueUiManager.instance.DisplaySingleStringTimed("Not Enough Souls Available", 3);

                    SetPlayerInteractionState(PlayerInteractionState.NotActive);
                    _playerIsInside = false;
                }

            }
        }

        #endregion

        #region Sacrificial Item Interactions

        private void ShowSacrificialItemDialogues()
        {
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

        private void HandleSacrificialItemPlayerInteraction()
        {
            PlayerModifierTypes.SacrificialItemInfo? itemInfo = null;

            if (Input.IsActionJustPressed(SceneControls.DialogueControl_1))
            {
                itemInfo = PlayerModifierTypes.GetSacrificialItemAffecter(_sacrificialItems[0]);
            }
            else if (Input.IsActionJustPressed(SceneControls.DialogueControl_2))
            {
                itemInfo = PlayerModifierTypes.GetSacrificialItemAffecter(_sacrificialItems[1]);
            }
            else if (Input.IsActionJustPressed(SceneControls.DialogueControl_3))
            {
                itemInfo = PlayerModifierTypes.GetSacrificialItemAffecter(_sacrificialItems[2]);
            }

            if (itemInfo.HasValue)
            {
                _playerController.HandleSacrificialItemInfluence(itemInfo.Value);
                _parentGroup.SetPlayerAsHostile();
                SetPlayerInteractionState(PlayerInteractionState.Ending);
            }
        }

        #endregion

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

        #region External Functions

        public void SetAsShop() => _isShop = true;

        #endregion
    }
}