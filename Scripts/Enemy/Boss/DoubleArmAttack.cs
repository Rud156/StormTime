using Godot;

namespace StormTime.Enemy.Boss
{
    public class DoubleArmAttack : BossBaseAttack
    {
        [Export] public float dualArmAttackCount;

        public override bool UpdateAttack(float delta)
        {
            return base.UpdateAttack(delta);
        }
    }
}