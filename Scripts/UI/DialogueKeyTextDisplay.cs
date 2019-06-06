using Godot;

namespace StormTime.UI
{
    public class DialogueKeyTextDisplay : Panel
    {
        [Export] public NodePath keyTextNodePath;
        [Export] public NodePath dialogueTextNodePath;

        private Label _keyTextLabel;
        private Label _dialogueLabel;

        public override void _Ready()
        {
            _keyTextLabel = GetNode<Label>(keyTextNodePath);
            _dialogueLabel = GetNode<Label>(dialogueTextNodePath);
        }

        public void SetAndDisplayDialogueAndKey(string key, string dialogue)
        {
            _keyTextLabel.SetText(key);
            _dialogueLabel.SetText(dialogue);

            DisplayDialogue();
        }

        public void ClearAndHideDialogueAndKey()
        {
            _keyTextLabel.SetText(string.Empty);
            _dialogueLabel.SetText(string.Empty);

            HideDialogue();
        }

        public void DisplayDialogue() => SetVisible(true);

        public void HideDialogue() => SetVisible(false);
    }
}