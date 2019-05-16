using Godot;
using System;

public class Bullet : KinematicBody2D
{
    [Export] public float bulletSpeed;

    private Vector2 _launchVelocity;
    private Vector2 _position;

    public override void _Ready()
    {
        _launchVelocity = Vector2.Zero;
    }

    public override void _PhysicsProcess(float delta)
    {
        KinematicCollision2D collision = MoveAndCollide(_launchVelocity * delta);
        if (collision != null)
        {
            // TODO: Destroy the Object and Spawn and Effect
        }
    }
}
