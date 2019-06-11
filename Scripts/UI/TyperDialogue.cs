using Godot;

namespace StormTime.UI
{
    public class TyperDialogue : Control
    {
        [Export] public NodePath textTyperNodePath;
        [Export] public NodePath dialogueInteractionLabelNodePath;

        private TextTyper _textTyper;
        private Label _dialogueLabel;

        public override void _Ready()
        {
            _textTyper = GetNode<TextTyper>(textTyperNodePath);
            _dialogueLabel = GetNode<Label>(dialogueInteractionLabelNodePath);
        }

        #region External Functions

        public TextTyper GetTextTyper() => _textTyper;

        public void SetInteractionLabelString(string key) => _dialogueLabel.SetText(key);

        #endregion
    }
}