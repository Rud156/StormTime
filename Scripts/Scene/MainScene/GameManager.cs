using Godot;

namespace StormTime.Scene.MainScene
{
    public class GameManager : Node
    {
        public static GameManager instance;

        public override void _Ready()
        {
            if (instance == null)
            {
                instance = this;
            }
        }
    }
}