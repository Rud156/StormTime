using Godot;
using System;

namespace StormTime.Player
{
    public class CameraPivot : Position2D
    {
        public override void _PhysicsProcess(float delta)
        {
            SetRotationDegrees(-PlayerVariables.PlayerStaticMovementRotation);
        }
    }
}