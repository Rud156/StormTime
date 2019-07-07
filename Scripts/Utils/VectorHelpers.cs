using Godot;

namespace StormTime.Utils
{
    public class VectorHelpers : Node2D
    {
        public static Vector2 Random2D()
        {
            float randomValue = (float)GD.RandRange(0, 1);
            return Vector2FromAngle(randomValue * Mathf.Tau);
        }

        public static Vector2 Vector2FromAngle(float angle, float length = 1)
        {
            Vector2 vector = new Vector2
            {
                x = length * Mathf.Cos(angle),
                y = length * Mathf.Sin(angle)
            };

            return vector;
        }
    }
}