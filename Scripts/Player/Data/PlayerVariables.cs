using Godot;

namespace StormTime.Player.Data
{
    public class PlayerVariables : Node2D
    {
        public static Vector2 LastPlayerPosition = new Vector2();

        // Data used only on the Boss Scene
        public static float PlayerCurrentMaxHealth;
        public static float PlayerCurrentMovementSpeed;
        public static float PlayerCurrentShootingDamageDiff;

        public static void ClearPlayerData()
        {
            PlayerCurrentMaxHealth = 0;
            PlayerCurrentMovementSpeed = 0;
            PlayerCurrentShootingDamageDiff = 0;
        }
    }
}
