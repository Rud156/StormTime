using Godot;
using System;

namespace StormTime.Enemy
{
    public class SpinnerEnemy : Enemy
    {
        [Export] public float rotationRate;
        [Export] public float rotationSwitchTime;

        private bool _isForwardRotation;
        private float _currentRotationTime;

        protected override void UpdateAttacking(float delta)
        {
            base.UpdateAttacking(delta);
            RotateAndAttack(delta);
        }

        private void RotateAndAttack(float delta)
        {
            _currentRotationTime -= delta;

            if (_currentRotationTime <= 0)
            {
                _isForwardRotation = !_isForwardRotation;
                _currentRotationTime = rotationSwitchTime;
            }

            if (_isForwardRotation)
            {
                Rotate(Mathf.Deg2Rad(rotationRate * delta));
            }
            else
            {
                Rotate(Mathf.Deg2Rad(-rotationRate * delta));
            }
        }
    }
}