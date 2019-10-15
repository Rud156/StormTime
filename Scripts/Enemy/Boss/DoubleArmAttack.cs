using System.Collections.Generic;
using Godot;

namespace StormTime.Enemy.Boss
{
    public class DoubleArmAttack : BossBaseAttack
    {
        [Export] public float dualArmAttackCount;
        [Export] public Godot.Collections.Array<NodePath> bossArmControllerNodePaths;

        private List<BossArmController> _bossArmControllers;
        private float _timeBetweenEachAttack;
        private float _currentAttackTimer;

        public override void _Ready()
        {
            _bossArmControllers = new List<BossArmController>();
            foreach (NodePath bossArmController in bossArmControllerNodePaths)
            {
                _bossArmControllers.Add(GetNode<BossArmController>(bossArmController));
            }

            _timeBetweenEachAttack = attackTimer / dualArmAttackCount;
        }

        #region Overridden Parent

        public override bool UpdateAttack(float delta)
        {
            bool updateAttack = base.UpdateAttack(delta);

            _currentAttackTimer -= delta;
            if (_currentAttackTimer <= 0)
            {
                LaunchRandomArmAttack();
                _currentAttackTimer = _timeBetweenEachAttack;
            }

            return updateAttack;
        }

        public override void LaunchAttack()
        {
            base.LaunchAttack();

            LaunchRandomArmAttack();
            _currentAttackTimer = _timeBetweenEachAttack;
        }

        #endregion

        #region Utility Functions

        private void LaunchRandomArmAttack()
        {
            int randomArmIndex = (int)(GD.Randi() % _bossArmControllers.Count);
            randomArmIndex = Mathf.Abs(randomArmIndex);
            _bossArmControllers[randomArmIndex].LaunchDualArmAttack(_timeBetweenEachAttack);
        }

        #endregion
    }
}