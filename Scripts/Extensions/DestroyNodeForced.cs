using Godot;

namespace StormTime.Extensions
{
    public class DestroyNodeForced : Node2D
    {
        #region External Functions

        public void DestroyNode() => QueueFree();

        #endregion
    }
}