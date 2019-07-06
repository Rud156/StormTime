using Godot;

namespace StormTime.Enemy.Boss
{
    public class BossController : Node2D
    {
        [Export] public NodePath bodyNodePath;
        [Export] public NodePath leftArmNodePath;
        [Export] public NodePath rightArmNodePath;
        [Export] public NodePath topArmNodePath;
        [Export] public NodePath bottomArmNodePath;

        private BossBodyController _bodyController;

        private BossArmController _leftArmController;
        private BossArmController _rightArmController;
        private BossArmController _topArmController;
        private BossArmController _bottomArmController;

        public override void _Ready()
        {
            _bodyController = GetNode<BossBodyController>(bodyNodePath);
            _bodyController.bodyStatusChanged += HandleBodyStatusChanged;

            _leftArmController = GetNode<BossArmController>(leftArmNodePath);
            _rightArmController = GetNode<BossArmController>(rightArmNodePath);
            _topArmController = GetNode<BossArmController>(topArmNodePath);
            _bottomArmController = GetNode<BossArmController>(bottomArmNodePath);

            _leftArmController.armStatusChanged += HandleLeftArmStatusChanged;
            _rightArmController.armStatusChanged += HandleRightArmStatusChanged;
            _topArmController.armStatusChanged += HandleTopArmStatusChanged;
            _bottomArmController.armStatusChanged += HandleBottomArmStatusChanged;
        }

        public override void _ExitTree()
        {
            _bodyController.bodyStatusChanged -= HandleBodyStatusChanged;

            _leftArmController.armStatusChanged -= HandleLeftArmStatusChanged;
            _rightArmController.armStatusChanged -= HandleRightArmStatusChanged;
            _topArmController.armStatusChanged -= HandleTopArmStatusChanged;
            _bottomArmController.armStatusChanged -= HandleBottomArmStatusChanged;
        }

        #region Utility Functions

        private void HandleLeftArmStatusChanged(BossArmController.ArmStatus armStatus)
        {

        }

        private void HandleRightArmStatusChanged(BossArmController.ArmStatus armStatus)
        {

        }

        private void HandleTopArmStatusChanged(BossArmController.ArmStatus armStatus)
        {

        }

        private void HandleBottomArmStatusChanged(BossArmController.ArmStatus armStatus)
        {

        }

        private void HandleBodyStatusChanged(BossBodyController.BodyStatus bodyStatus)
        {

        }

        #endregion
    }
}