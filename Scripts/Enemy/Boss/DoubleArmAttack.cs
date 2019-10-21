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
        private float _currentSingleAttackTimer;

        private int _previousArmIndex;

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
            bool attackComplete = base.UpdateAttack(delta);

            _currentSingleAttackTimer -= delta;
            if (_currentSingleAttackTimer <= 0)
            {
                LaunchRandomArmAttack();
                _currentSingleAttackTimer = _timeBetweenEachAttack;
            }

            if (attackComplete)
            {
                ClearEndAttack();
            }

            return attackComplete;
        }

        public override void LaunchAttack()
        {
            base.LaunchAttack();

            _previousArmIndex = -1;
            _currentSingleAttackTimer = _timeBetweenEachAttack;

            LaunchRandomArmAttack();
        }

        #endregion

        #region Utility Functions

        private void ClearEndAttack() => _bossArmControllers[_previousArmIndex].ClearDualArmAttack();

        private void LaunchRandomArmAttack()
        {
            int randomArmIndex = (int)(GD.Randi() % _bossArmControllers.Count);
            randomArmIndex = Mathf.Abs(randomArmIndex);

            while (true)
            {
                BossArmController bossArmController = _bossArmControllers[randomArmIndex];
                BossArmController.ArmStatus armStatus = bossArmController.GetArmStatus();
                if (armStatus.firstArmAlive && armStatus.secondArmAlive)
                {
                    bossArmController.LaunchDualArmAttack(_timeBetweenEachAttack);
                    break;
                }
                else
                {
                    randomArmIndex = (int)(GD.Randi() % _bossArmControllers.Count);
                    randomArmIndex = Mathf.Abs(randomArmIndex);
                }
            }

            if (_previousArmIndex != -1)
            {
                _bossArmControllers[_previousArmIndex].ClearDualArmAttack();
            }

            _previousArmIndex = randomArmIndex;
        }

        #endregion
    }
}