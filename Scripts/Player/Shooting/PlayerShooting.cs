using Godot;
using StormTime.Utils;

public class PlayerShooting : Node
{
    private static readonly PackedScene BulletPrefab =
        ResourceLoader.Load<PackedScene>("res://Prefabs/Bullet.tscn");

    [Export] public NodePath _playerBulletHolderNodePath;

    private Node2D _playerBulletHolder;
    private KinematicBody2D _playerRoot;

    public override void _Ready()
    {
        base._Ready();

        _playerBulletHolder = GetNode<Node2D>(_playerBulletHolderNodePath);
        _playerRoot = GetParent<KinematicBody2D>();
    }

    public override void _Process(float delta)
    {
        base._Process(delta);

        if (Input.IsActionJustPressed(SceneControls.Shoot))
        {
            Bullet bulletInstance = (Bullet)BulletPrefab.Instance();
            bulletInstance.SetPosition(_playerRoot.GetPosition());
            bulletInstance.LaunchBullet(_playerRoot.GetTransform().x);
            GD.Print(_playerRoot.GetTransform().x);
            _playerBulletHolder.AddChild(bulletInstance);
        }
    }
}
