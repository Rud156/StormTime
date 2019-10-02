using Godot;

namespace StormTime.Enemy.Boss
{
    public abstract class BossBaseAttack : Node
    {
        [Export] public float attackTimer;

        public virtual bool Update(float delta)
        {
            attackTimer -= delta;

            if (attackTimer <= 0)
            {
                return true;
            }

            return false;
        }
    }
}