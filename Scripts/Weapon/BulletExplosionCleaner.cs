using Godot;
using System;

namespace StormTime.Weapon
{
    public class BulletExplosionCleaner : Timer
    {
        public override void _Ready()
        {
            base._Ready();
            this.Connect("timeout", this, "OnTimeOut");
        }

        public void OnTimeOut()
        {
            GetParent().GetParent().RemoveChild(this);
        }
    }
}
