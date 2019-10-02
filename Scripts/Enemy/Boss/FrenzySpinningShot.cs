using Godot;

namespace StormTime.Enemy.Boss
{
    public class FrenzySpinningShot : BossBaseAttack
    {
        [Export] public int frenzyCircleShotCount;

        public override bool Update(float delta)
        {
            return base.Update(delta);
        }
    }
}