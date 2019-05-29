using Godot;
using StormTime.Player.Movement;
using StormTime.Utils;
using StormTime.Weapon;

public class PlayerShooting : Node
{
    [Export] public NodePath _playerBulletHolderNodePath;
    [Export] public PackedScene playerBulletPrefab;

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
        Bullet bulletInstance = (Bullet)playerBulletPrefab.Instance();
        _playerBulletHolder.AddChild(bulletInstance);

        bulletInstance.SetGlobalPosition(_playerRoot.GetGlobalPosition());
        bulletInstance.LaunchBullet(_playerRoot.GetTransform().x);
    }
}
