using System.Collections.Generic;
using Godot;

namespace StormTime.Extensions
{
    public class ParticlesCustomController : Node2D
    {
        [Export] public Godot.Collections.Array<NodePath> particleEffectsNodePaths;

        private List<Particles2D> _particleEffects;
        private bool _isEmitting;

        public override void _Ready()
        {
            _particleEffects = new List<Particles2D>();

            foreach (NodePath particleEffectNodePath in particleEffectsNodePaths)
            {
                _particleEffects.Add(GetNode<Particles2D>(particleEffectNodePath));
            }

            ActivateParticleEffects();
        }

        #region External Functions

        public void ActivateParticleEffects()
        {
            foreach (Particles2D particleEffect in _particleEffects)
            {
                particleEffect.SetEmitting(true);
            }

            _isEmitting = true;
        }

        public void DeActivateParticleEffects()
        {
            foreach (Particles2D particleEffect in _particleEffects)
            {
                particleEffect.SetEmitting(false);
            }

            _isEmitting = false;
        }

        public bool IsEmitting() => _isEmitting;

        #endregion
    }
}