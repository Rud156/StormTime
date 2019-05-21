using Godot;
using System;

namespace StormTime.Weapon
{
    public class EnemyBullet : Bullet
    {
        [Export] public NodePath bulletSpritePath;

        private Color _bulletColor;
        private Sprite _bulletSprite;

        public override void _Ready()
        {
            _bulletSprite = GetNode<Sprite>(bulletSpritePath);
            _bulletSprite.SelfModulate = _bulletColor;
        }

        public void SetBulletColor(Color color) => _bulletColor = color;
    }
}