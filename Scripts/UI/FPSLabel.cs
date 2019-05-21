using Godot;

namespace StormTime.UI
{
    public class FPSLabel : Label
    {
        public override void _Process(float delta)
        {
            SetText($"FPS: {Engine.GetFramesPerSecond()}");
        }
    }
}