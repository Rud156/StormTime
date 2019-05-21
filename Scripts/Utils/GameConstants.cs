using Godot;

namespace StormTime.Utils
{
    public class GameConstants : Node2D
    {
        public const string BulletPrefab = "res://Prefabs/Bullet.tscn";
        public const string PlayerPrefab = "res://Prefabs/Player.tscn";

        // Particle Effects
        public const string BulletExplosionPrefab = "res://Prefabs/Effects/Bullet Explosion.tscn";
        public const string BulletTrailPrefab = "res://Prefabs/Effects/Bullet Trail.tscn";
    }
}
