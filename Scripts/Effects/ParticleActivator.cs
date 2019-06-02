using Godot;

namespace StormTime.Effects
{
    public class ParticleActivator : Particles2D
    {
        public override void _Ready() => SetEmitting(true);
    }
}
