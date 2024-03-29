using Godot;

namespace StormTime.Effects
{
    public class ParticlesCleaner : Node2D
    {
        [Export] public float cleanupWaitTime;

        private float _currentWaitTime;

        public override void _Ready() => _currentWaitTime = cleanupWaitTime;

        public override void _Process(float delta)
        {
            _currentWaitTime -= delta;
            if (_currentWaitTime <= 0)
            {
                CleanUpParticles();
            }
        }

        private void CleanUpParticles() => QueueFree();
    }
}