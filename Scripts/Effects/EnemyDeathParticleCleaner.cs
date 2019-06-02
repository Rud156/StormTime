using Godot;

namespace StormTime.Effects
{
    public class EnemyDeathParticleCleaner : ParticlesCleaner
    {
        [Export] public NodePath _deathEffect_1_NodePath;
        [Export] public NodePath _deathEffect_2_NodePath;

        private Particles2D _deathEffect_1;
        private Particles2D _deathEffect_2;

        public override void _Ready()
        {
            base._Ready();

            _deathEffect_1 = GetNode<Particles2D>(_deathEffect_1_NodePath);
            _deathEffect_2 = GetNode<Particles2D>(_deathEffect_2_NodePath);
        }

        public void SetEffectGradient(GradientTexture effectGradientTexture)
        {
            _deathEffect_1.ProcessMaterial.Set("color_ramp", effectGradientTexture);
            _deathEffect_2.ProcessMaterial.Set("color_ramp", effectGradientTexture);
        }
    }
}