using Godot;

namespace StormTime.Extensions
{
    public class Rotator : Node2D
    {
        [Export] public float rotationSpeed;

        public override void _Process(float delta) =>
            Rotate(Mathf.Deg2Rad(rotationSpeed * delta));
    }
}