using Godot;

namespace StormTime.Weapon
{
    public class ParticleActivator : Particles2D
    {
        public override void _Ready()
        {
            base._Ready();
            SetEmitting(true);
        }
    }
}
