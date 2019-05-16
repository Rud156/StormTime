using Godot;
using System;

public class Bullet : KinematicBody2D
{
    [Export] public float bulletSpeed;
    [Export] public float bulletLifeTime;

    private Vector2 _launchVelocity;
    private Vector2 _position;

    private float _currentBulletTimeLeft;

    public override void _PhysicsProcess(float delta)
    {
        KinematicCollision2D collision = MoveAndCollide(_launchVelocity * delta);
        if (collision != null || _currentBulletTimeLeft <= 0)
        {
            GD.Print("Bullet Collided");
            DestroyBullet();
        }

        _currentBulletTimeLeft -= delta;
    }

    public void LaunchBullet(Vector2 playerForwardVector)
    {
        _launchVelocity = playerForwardVector * bulletSpeed;
        _currentBulletTimeLeft = bulletLifeTime;
    }

    public void DestroyBullet()
    {
        GetParent().RemoveChild(this);
    }
}
