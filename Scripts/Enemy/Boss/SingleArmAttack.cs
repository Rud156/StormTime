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
        private float _currentAttackTimer;

        public override void _Ready()
        {
            _bossArmControllers = new List<BossArmController>();
            foreach (NodePath bossArmController in bossArmControllerNodePaths)
            {
                _bossArmControllers.Add(GetNode<BossArmController>(bossArmController));
            }

            _timeBetweenEachAttack = attackTimer / singleArmAttackCount;
        }

        public override bool UpdateAttack(float delta)
        {
            _currentAttackTimer -= delta;
            if (_currentAttackTimer <= 0)
            {
                LaunchRandomArmAttack();
                _currentAttackTimer = _timeBetweenEachAttack;
            }

            return base.UpdateAttack(delta);
        }

        public override void LaunchAttack()
        {
            base.LaunchAttack();

            LaunchRandomArmAttack();
            _currentAttackTimer = _timeBetweenEachAttack;
        }

        #region Utility Functions

        private void LaunchRandomArmAttack()
        {
            int randomArmIndex = (int)GD.Randi() % _bossArmControllers.Count;
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