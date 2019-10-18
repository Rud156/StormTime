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

        public static Vector2 MoveTowards(Vector2 current, Vector2 target, float maxDistanceDelta)
        {
            float toVector_x = target.x - current.x;
            float toVector_y = target.y - current.y;

            float sqDist = toVector_x * toVector_x + toVector_y * toVector_y;

            if (sqDist == 0 || (maxDistanceDelta >= 0 && sqDist <= maxDistanceDelta * maxDistanceDelta))
                return target;

            float dist = Mathf.Sqrt(sqDist);

            return new Vector2(current.x + toVector_x / dist * maxDistanceDelta,
                current.y + toVector_y / dist * maxDistanceDelta);
        }
    }
}