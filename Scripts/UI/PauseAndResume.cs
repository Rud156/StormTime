using Godot;
using StormTime.Scene.MainScene;

namespace StormTime.UI
{
    public class PauseAndResume : Panel
    {
        [Export] public NodePath resumeButtonNodePath;
        [Export] public NodePath mainMenuButtonNodePath;

        private TextureButton _resumeButton;
        private TextureButton _mainMenuButton;

        public override void _Ready()
        {
            _resumeButton = GetNode<TextureButton>(resumeButtonNodePath);
            _mainMenuButton = GetNode<TextureButton>(mainMenuButtonNodePath);

            _resumeButton.Connect("pressed", this, nameof(HidePauseMenu));
            _mainMenuButton.Connect("pressed", this, nameof(HandleMainMenuClicked));
        }

        #region External Functions

        public void ShowPauseMenu()
        {
            Show();
            GetTree().SetPause(true);

            GameManager.instance.PauseMenuOpened();
        }

        public void HidePauseMenu()
        {
            Hide();
            GetTree().SetPause(false);

            GameManager.instance.PauseMenuClosed();
        }

        #endregion

        #region Utility Functions

        private void HandleMainMenuClicked()
        {
            // TODO: Redirect to menu screen
        }

        #endregion
    }
}