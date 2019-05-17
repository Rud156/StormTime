using Godot;
using StormTime.Player.Movement;
using StormTime.Utils;
using StormTime.Weapon;

public class PlayerShooting : Node
{
    private static readonly PackedScene BulletPrefab =
        ResourceLoader.Load<PackedScene>(GameConstants.BulletPrefab);

    [Export] public NodePath _playerBulletHolderNodePath;

    private Node2D _playerBulletHolder;
    private PlayerMovement _playerRoot;

    public override void _Ready()
    {
        base._Ready();

        GD.Print(_playerBulletHolderNodePath);
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
        bulletInstance.LaunchBullet(_playerRoot.GetTransform().x);

        _playerBulletHolder.AddChild(bulletInstance);
    }
}
