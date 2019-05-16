using Godot;
using StormTime.Player.Movement;
using StormTime.Utils;

public class PlayerShooting : Node
{
    private static readonly PackedScene BulletPrefab =
        ResourceLoader.Load<PackedScene>("res://Prefabs/Bullet.tscn");

    [Export] public NodePath _playerBulletHolderNodePath;

    private Node2D _playerBulletHolder;
    private PlayerMovement _playerRoot;

    public override void _Ready()
    {
        base._Ready();

        _playerBulletHolder = GetNode<Node2D>(_playerBulletHolderNodePath);
        _playerRoot = GetParent<PlayerMovement>();
    }

    public override void _Process(float delta)
    {
        base._Process(delta);

        if (Input.IsActionJustPressed(SceneControls.Shoot))
        {
            ShootBullet();
        }
    }

    private void ShootBullet()
    {
        Bullet bulletInstance = (Bullet)BulletPrefab.Instance();
        bulletInstance.SetPosition(_playerRoot.GetPosition());

        float rotation = _playerRoot.GetCurrentRotation() - 90;
        float xVelocity = Mathf.Cos(Mathf.Deg2Rad(rotation));
        float yVelocity = Mathf.Sin(Mathf.Deg2Rad(rotation));

        bulletInstance.LaunchBullet(new Vector2(xVelocity, yVelocity));

        _playerBulletHolder.AddChild(bulletInstance);
    }
}
