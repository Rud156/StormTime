using System;
using Godot;
using StormTime.Player.Movement;
using StormTime.Utils;

namespace StormTime.Enemy.Groups
{
    public class EnemyGroupPlayerInteraction : Area2D
    {
        private enum PlayerInteractionState
        {
            NotActive,
            Active,
            Ending,
            Completed
        }

        private PlayerInteractionState _playerInteractionState;
        private bool _playerIsInside;
        private PlayerController _playerController;


        public override void _Ready()
        {
            _playerIsInside = false;

            base.Connect("body_entered", this, "HandlePlayerEntry");
            base.Connect("body_exited", this, "HandlePlayerExit");
        }

        public override void _Process(float delta)
        {
            if (!_playerIsInside)
            {
                return;
            }

            CheckPlayerInteraction(delta);
        }

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

        }

        private void HandlePlayerInteractionActive(float delta)
        {

        }

        private void HandlePlayerInteractionEnding(float delta)
        {

        }

        private void HandlePlayerInteractionComplete(float delta)
        {

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