using Godot;

namespace StormTime.Enemy.Boss
{
    public class FrenzySpinningShot : BossBaseAttack
    {
        [Export] public int frenzyCircleShotCount;

        public override bool UpdateAttack(float delta)
        {
            return base.UpdateAttack(delta);
        }
    }
}