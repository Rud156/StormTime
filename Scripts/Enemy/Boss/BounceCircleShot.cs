using Godot;

namespace StormTime.Enemy.Boss
{
    public class BounceCircleShot : BossBaseAttack
    {
        [Export] public int bounceCircleShotCount;

        public override bool Update(float delta)
        {
            return base.Update(delta);
        }
    }
}