using Godot;
using StormTime.Player.Data;

namespace StormTime.Enemy
{
    public class SlasherEnemy : Enemy
    {
        // Player Targeting and Attack
        [Export] public float attackingPlayerMovementSpeed;
        [Export] public float attackingWaitTime;
        [Export] public float rotationRate;
        [Export] public NodePath slasherEffectNode;

        private Particles2D _slasherEffect;
        private float _currentRotationRate;

        public override void _Ready()
        {
            base._Ready();
            _slasherEffect = GetNode<Particles2D>(slasherEffectNode);
        }

        protected override void UpdateAttacking(float delta)
        {
            base.UpdateAttacking(delta);

            if (_enemyTimer > attackingWaitTime)
            {
                _targetPosition = PlayerVariables.PlayerPosition;
                MoveToTowardsTarget(_targetPosition, attackingPlayerMovementSpeed);
            }
            else
            {
                _currentRotationRate -= delta * attackingWaitTime * rotationRate;
                _currentRotationRate = _currentRotationRate <= 0 ? 0 : _currentRotationRate;
                _slasherEffect.SetEmitting(false);
            }

            Rotate(Mathf.Deg2Rad(_currentRotationRate * delta));
        }

        protected override void LaunchAttack()
        {
            base.LaunchAttack();

            _currentRotationRate = rotationRate;
            _slasherEffect.SetEmitting(true);
        }
    }
}