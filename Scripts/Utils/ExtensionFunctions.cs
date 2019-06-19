using Godot;

namespace StormTime.Utils
{
    public class ExtensionFunctions : Node2D
    {
        public static string Format2DecimalPlace(float value) => value.ToString("0.##");

        public static Color ConvertAndClampColor(float r = 0, float g = 0, float b = 0, float a = 0) =>
            new Color(Mathf.Clamp(r, 0, 255) / 255, Mathf.Clamp(g, 0, 255) / 255, Mathf.Clamp(b, 0, 255) / 255,
                Mathf.Clamp(a, 0, 255) / 255);

        public static float To360Angle(float angle)
        {
            while (angle < 0.0f)
                angle += 360.0f;
            while (angle >= 360.0f)
                angle -= 360.0f;

            return angle;
        }

        public static float LerpAngleDeg(float from, float to, float amount)
        {
            float delta = ((to - from + 360 + 180) % 360) - 180;
            return (from + delta * amount + 360) % 360;
        }

        public static float Map(float from, float fromMin, float fromMax, float toMin, float toMax)
        {
            var fromAbs = from - fromMin;
            var fromMaxAbs = fromMax - fromMin;

            var normal = fromAbs / fromMaxAbs;

            var toMaxAbs = toMax - toMin;
            var toAbs = toMaxAbs * normal;

            var to = toAbs + toMin;

            return to;
        }

        public static Vector2 VectorRandomUnit() =>
            VectorFromAngle((float)GD.Randf() * Mathf.Tau);

        public static Vector2 VectorFromAngle(float angle, float length = 1) =>
            new Vector2(length * Mathf.Cos(angle), length * Mathf.Sin(angle));
    }
}