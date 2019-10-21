using Godot;
using StormTime.Common;
using StormTime.Utils;
using StormTime.Weapon;

namespace StormTime.Enemy.Boss
{
    public class BossSingleArmController : RigidBody2D
    {
        [Export] public NodePath healthSetterNodePath;

        // Arm Destruction
        [Export] public float armAlphaChangeRate;
        [Export] public float armRotationRate;
        [Export] public float armDetachForce;

        private HealthSetter _healthSetter;

        // Arm Destruction
        private bool _armDestructionActive;
        private float _currentAlpha;

        public override void _Ready() => _healthSetter = GetNode<HealthSetter>(healthSetterNodePath);

        public override void _Process(float delta)
        {
            if (!_armDestructionActive)
            {
                return;
            }

            Rotate(Mathf.Deg2Rad(armRotationRate * delta));
            UpdateArmAlpha(delta);
        }

        #region External Functions

        // Event Function from Bullet Collision
        public void BulletCollisionNotification(object bullet, bool isFreezingBullet)
        {
            if (_armDestructionActive)
            {
                return;
            }

            bool isPLayerBullet = !(bullet is EnemyBullet);

            if (isPLayerBullet)
            {
                float damageAmount = ((Bullet)bullet).GetBulletDamage();
                _healthSetter.SubtractHealth(damageAmount);
            }
        }

        public void DestroyArm()
        {
            Node2D targetParent = (Node2D)GetParent().GetParent().GetParent();
            Vector2 initialPosition = GetGlobalPosition();

            GetParent().RemoveChild(this);
            targetParent.AddChild(this);

            SetGlobalPosition(initialPosition);

            Color currentColor = GetSelfModulate();
            _currentAlpha = currentColor.a;

            _armDestructionActive = true;

            SetMode(ModeEnum.Rigid);

            Vector2 randomVector = VectorHelpers.Random2D();
            AddForce(Vector2.Zero, randomVector * armDetachForce);
        }

        #endregion

        #region Utility Functions

        private void UpdateArmAlpha(float delta)
        {
            _currentAlpha -= armAlphaChangeRate * delta;

            Color currentColor = GetSelfModulate();
            currentColor.a = _currentAlpha;

            SetSelfModulate(currentColor);

            if (_currentAlpha <= 0)
            {
                QueueFree();
            }
        }

        #endregion
    }
}