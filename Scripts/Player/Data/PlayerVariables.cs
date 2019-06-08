using Godot;
using System;

namespace StormTime.Player.Data
{
    public class PlayerVariables : Node2D
    {
        public static Vector2 PlayerPosition = new Vector2();

        // Player World Status
        public static int GroupsDestroyed = 0;
        public static int EnemiesKilled = 0;
        public static int GroupsNonHostileInteraction = 0;
        public static int PlayerReputation = 0;
    }
}
