using Godot;

namespace StormTime.Utils
{
    public class GameConstants : Node2D
    {
        // Bullets
        public const string BulletPrefab = "res://Prefabs/Interactibles/Bullets/Bullet.tscn";
        public const string EnemyBulletPrefab = "res://Prefabs/Interactibles/Bullets/Enemy Bullet.tscn";

        public const string PlayerPrefab = "res://Prefabs/Interactibles/Player/Player.tscn";

        // Particle Effects
        public const string BulletExplosionPrefab = "res://Prefabs/Effects/Bullet Explosion.tscn";
        public const string BulletExplosionGBPrefab = "res://Prefabs/Effects/Bullet Explosion GB.tscn";
        public const string BulletTrailPrefab = "res://Prefabs/Effects/Bullet Trail.tscn";
        public const string BulletTrailGBPrefab = "res://Prefabs/Effects/Bullet Trail Green.tscn";
    }
}
