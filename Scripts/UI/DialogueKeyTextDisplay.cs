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

        public void SetDialogueAndKey(string key, string dialogue)
        {
            _keyTextLabel.SetText(key);
            _dialogueLabel.SetText(dialogue);
        }
    }
}