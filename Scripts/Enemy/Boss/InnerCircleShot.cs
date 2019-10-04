using Godot;

namespace StormTime.Enemy.Boss
{
    public class InnerCircleShot : BossBaseAttack
    {
        [Export] public int innerCircleShotCount;

        public override bool UpdateAttack(float delta)
        {
            return base.UpdateAttack(delta);
        }
    }
}