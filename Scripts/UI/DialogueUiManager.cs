using System.Collections.Generic;
using Godot;

namespace StormTime.UI
{
    public class DialogueUiManager : Panel
    {
        [Export] public NodePath singleLineLabelNodePath;
        [Export] public NodePath singleLineHolderNodePath;
        [Export] public NodePath multiDialogueHolderNodePath;
        [Export] public Godot.Collections.Array<NodePath> dialogueNodePaths;

        public static DialogueUiManager instance;

        private Control _multiDialogueHolder;
        private List<TextTyper> _dialogues;

        private Control _singleDialogueHolder;
        private TextTyper _singleDialogue;

        public override void _Ready()
        {
            _multiDialogueHolder = GetNode<Control>(multiDialogueHolderNodePath);
            _dialogues = new List<TextTyper>();
            foreach (var dialogueNodePath in dialogueNodePaths)
            {
                _dialogues.Add(GetNode<TextTyper>(dialogueNodePath));
            }

            _singleDialogueHolder = GetNode<Control>(singleLineHolderNodePath);
            _singleDialogue = GetNode<TextTyper>(singleLineLabelNodePath);

            ClearAll();

            if (instance == null)
            {
                instance = this;
            }
        }

        #region External Functions

        public void DisplaySingleString(string dialogue)
        {
            ClearAll();

            SetVisible(true);
            _singleDialogueHolder.SetVisible(true);
            _singleDialogue.DisplayString(dialogue);
        }

        public void DisplayMultiDialogue(string[] dialogues)
        {
            ClearAll();

            SetVisible(true);
            for (int i = 0; i < _dialogues.Count; i++)
            {
                _dialogues[i].DisplayString(dialogues[i]);
            }
        }

        public void ClearMultiDialogue()
        {
            foreach (TextTyper textTyper in _dialogues)
            {
                textTyper.ClearString();
            }
            _multiDialogueHolder.SetVisible(false);
            SetVisible(false);
        }

        public void ClearSingleDialogue()
        {
            _singleDialogue.ClearString();
            _singleDialogueHolder.SetVisible(false);
            SetVisible(false);
        }

        #endregion

        #region Utility Functions

        private void ClearAll()
        {
            foreach (TextTyper textTyper in _dialogues)
            {
                textTyper.ClearString();
            }
            _multiDialogueHolder.SetVisible(false);

            _singleDialogue.ClearString();
            _singleDialogueHolder.SetVisible(false);
            SetVisible(false);
        }

        #endregion
    }
}