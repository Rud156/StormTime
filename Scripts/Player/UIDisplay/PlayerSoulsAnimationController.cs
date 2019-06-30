using Godot;

namespace StormTime.Player.UIDisplay
{
    public class PlayerSoulsAnimationController : AnimationTree
    {
        private const string PlaybackPath = "parameters/playback";
        private const string IdleAnimationPath = "Idle";
        private const string ActionAnimationPath = "Action";

        private AnimationNodeStateMachinePlayback _stateMachine;

        public override void _Ready() =>
            _stateMachine = (AnimationNodeStateMachinePlayback)Get(PlaybackPath);

        #region External Functions

        public void PlayIdleAnimation() => _stateMachine?.Travel(IdleAnimationPath);

        public void PlayActionAnimation() => _stateMachine?.Travel(ActionAnimationPath);

        #endregion
    }
}