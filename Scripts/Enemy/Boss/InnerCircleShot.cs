using Godot;

namespace StormTime.Enemy.Boss
{
    public class InnerCircleShot : BossBaseAttack
    {
        [Export] public int innerCircleShotCount;

        public override bool Update(float delta)
        {
            return base.Update(delta);
        }
    }
}