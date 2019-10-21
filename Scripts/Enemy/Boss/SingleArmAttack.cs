using System.Collections.Generic;
using Godot;

namespace StormTime.Enemy.Boss
{
    public class SingleArmAttack : BossBaseAttack
    {
        // Attack Count (Probably to be removed later on...)
        [Export] public float singleArmAttackCount;
        [Export] public Godot.Collections.Array<NodePath> bossArmControllerNodePaths;

        private List<BossArmController> _bossArmControllers;
        private float _timeBetweenEachAttack;
        private float _currentSingleAttackTimer;

        public override void _Ready()
        {
            _bossArmControllers = new List<BossArmController>();
            foreach (NodePath bossArmController in bossArmControllerNodePaths)
            {
                _bossArmControllers.Add(GetNode<BossArmController>(bossArmController));
            }

            _timeBetweenEachAttack = attackTimer / singleArmAttackCount;
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

            return attackComplete;
        }

        public override void LaunchAttack()
        {
            base.LaunchAttack();

            LaunchRandomArmAttack();
            _currentSingleAttackTimer = _timeBetweenEachAttack;
        }

        #endregion

        #region Utility Functions

        private void LaunchRandomArmAttack()
        {
            int randomArmIndex = (int)(GD.Randi() % _bossArmControllers.Count);
            randomArmIndex = Mathf.Abs(randomArmIndex);

            float randomNumber = (float)GD.Randf();

            if (randomNumber <= 0.5f)
            {
                _bossArmControllers[randomArmIndex].LaunchFirstArmAttack(_timeBetweenEachAttack);
            }
            else
            {
                _bossArmControllers[randomArmIndex].LaunchSecondArmAttack(_timeBetweenEachAttack);
            }
        }

        #endregion
    }
}