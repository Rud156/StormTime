using Godot;

namespace StormTime.Utils
{
    public class MovingBackgroundManager : Node2D
    {
        // Image Paths
        [Export] public NodePath backgroundSprite_1;
        [Export] public NodePath backgroundSprite_2;
        [Export] public NodePath backgroundSprite_3;
        [Export] public NodePath backgroundSprite_4;

        // Movement Data
        [Export] public float maxMovementSpeed;
        [Export] public float speedIncrementRate;

        // Sprites
        private Sprite _background_1;
        private Sprite _background_2;
        private Sprite _background_3;
        private Sprite _background_4;

        // These are effectively 4 squares
        // arranged end to end
        // 2 1
        // 3 4
        private float _y1, _y2, _y3, _y4;
        private float _x1, _x2, _x3, _x4;

        private float _currentSpeed;
        private bool _isBackgroundScrollingActive;

        public override void _Ready()
        {
            base._Ready();

            _background_1 = GetNode<Sprite>(backgroundSprite_1);
            _background_2 = GetNode<Sprite>(backgroundSprite_2);
            _background_3 = GetNode<Sprite>(backgroundSprite_3);
            _background_4 = GetNode<Sprite>(backgroundSprite_4);
        }
    }
}
