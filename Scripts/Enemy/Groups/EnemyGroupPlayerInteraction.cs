using Godot;
using StormTime.Utils;

namespace StormTime.Enemy.Groups
{
    public class EnemyGroupPlayerInteraction : Area2D
    {
        private bool _playerIsInside;  

        public override void _Ready()
        {
            _playerIsInside = false;

            base.Connect("body_entered", this,"HandlePlayerEntry");
            base.Connect("body_exited", this, "HandlePlayerExit");
        }

        public override void _Process(float delta)
        {
            if (!_playerIsInside)
            {
                return;
            }
        }

        public void HandlePlayerEntry(PhysicsBody2D other)
        {
            if (other.Name != TagManager.PlayerTag)
            {
                return;
            }
            
            _playerIsInside = true;
        }

        public void HandlePlayerExit(PhysicsBody2D other)
        {
            if (other.Name != TagManager.PlayerTag)
            {
                return;
            }

            _playerIsInside = false;
        }
    }
}