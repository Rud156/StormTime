using Godot;

namespace StormTime.Enemy.Boss
{
    public class BounceCircleShot : BossBaseAttack
    {
        [Export] public int bounceCircleShotCount;

        public override bool UpdateAttack(float delta)
        {
            return base.UpdateAttack(delta);
        }
    }
}