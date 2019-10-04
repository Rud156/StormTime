using Godot;

namespace StormTime.Enemy.Boss
{
    public class CircleWorldFill : BossBaseAttack
    {
        [Export] public int circleWorldFillCount;

        public override bool UpdateAttack(float delta)
        {
            return base.UpdateAttack(delta);
        }
    }
}