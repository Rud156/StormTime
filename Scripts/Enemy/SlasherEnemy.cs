using Godot;
using System;

namespace StormTime.Enemy
{
    public class SlasherEnemy : Enemy
    {
        [Export] public PackedScene _slasherEffect;

        protected override void UpdateAttacking(float delta)
        {
        }
    }
}