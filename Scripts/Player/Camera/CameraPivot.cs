using Godot;
using System;
using StormTime.Player.Data;

namespace StormTime.Player.Camera
{
    public class CameraPivot : Position2D
    {
        public override void _PhysicsProcess(float delta) => 
            SetRotationDegrees(-PlayerVariables.PlayerStaticMovementRotation);
    }
}