using Godot;

namespace StormTime.Enemy.Boss
{
    public class CircleWorldFill : BossBaseAttack
    {
        // Attack Affectors
        [Export] public int burstCircleCount;
        [Export] public float timeBetweenBursts;

        // Display Objects
        [Export] public PackedScene projectilePackedScene;
        [Export] public NodePath launchPointNodePath;

        private Node2D _launchPoint;
        private float _currentTimeBetweenBurst;

        public override void _Ready()
        {
            _launchPoint = GetNode<Node2D>(launchPointNodePath);
        }

        #region Overridden Parent

        public override bool UpdateAttack(float delta)
        {
            return base.UpdateAttack(delta);
        }

        public override void LaunchAttack()
        {
            base.LaunchAttack();
        }

        #endregion

        #region Utility Functions



        #endregion
    }
}