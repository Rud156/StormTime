using Godot;
using System;

namespace StormTime.Weapon
{
    public class EnemyBullet : Bullet
    {
        [Export] public NodePath bulletSpritePath;

        private Color _bulletColor = new Color(1, 1, 1);
        private Sprite _bulletSprite;

        public override void _Ready()
        {
            base._Ready();

            _bulletSprite = GetNode<Sprite>(bulletSpritePath);
            _bulletSprite.SetSelfModulate(_bulletColor);
        }

        public void SetBulletColor(Color color)
        {
            _bulletColor = color;
            _bulletSprite.SetSelfModulate(color);
        }
    }
}