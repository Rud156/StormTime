using Godot;
using StormTime.Player.Modifiers;
using StormTime.Utils;

namespace StormTime.Effects
{
    public class SoulsController : Node2D
    {
        [Export] public NodePath soulSpriteNodePath;
        [Export] public NodePath soulParticleNodePath;
        [Export] public NodePath soulAreaCollisionNodePath;

        private Sprite _soulSprite;
        private Particles2D _soulParticles;
        private Area2D _soulAreaCollision;

        public override void _Ready()
        {
            _soulSprite = GetNode<Sprite>(soulSpriteNodePath);
            _soulParticles = GetNode<Particles2D>(soulParticleNodePath);
            _soulAreaCollision = GetNode<Area2D>(soulAreaCollisionNodePath);

            _soulAreaCollision.Connect("body_entered", this, nameof(HandleBodyEntered));
        }

        #region Utility Functions

        public void HandleBodyEntered(PhysicsBody2D other)
        {
            if (other.Name != TagManager.PlayerTag)
            {
                return;
            }

            CallDeferred(nameof(RemoveSoulFromWorld));
        }

        public void RemoveSoulFromWorld()
        {
            PlayerModifierSoulsManager.instance.IncrementSouls(1);
            QueueFree();
        }

        #endregion

        #region External Functions

        public void SetSoulsColor(Color color)
        {
            _soulSprite.SetSelfModulate(color);
            _soulParticles.ProcessMaterial.Set("color", color);
        }

        #endregion
    }
}