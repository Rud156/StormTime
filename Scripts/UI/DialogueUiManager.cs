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
        [Export] public string[] multiDialogueKeys;

        public static DialogueUiManager instance;

        private Control _multiDialogueHolder;
        private List<TyperDialogue> _dialogues;

        private Control _singleDialogueHolder;
        private TextTyper _singleDialogue;

        private float _displayTimer;
        private bool _displayTimerActive;

        public override void _Ready()
        {
            _multiDialogueHolder = GetNode<Control>(multiDialogueHolderNodePath);
            _dialogues = new List<TyperDialogue>();
            for (var i = 0; i < dialogueNodePaths.Count; i++)
            {
                var dialogueNodePath = dialogueNodePaths[i];
                TyperDialogue typerDialogue = GetNode<TyperDialogue>(dialogueNodePath);

                _dialogues.Add(typerDialogue);
                typerDialogue.SetInteractionLabelString(multiDialogueKeys[i]);
            }

            _singleDialogueHolder = GetNode<Control>(singleLineHolderNodePath);
            _singleDialogue = GetNode<TextTyper>(singleLineLabelNodePath);

            ClearAll();

            if (instance == null)
            {
                instance = this;
            }
        }

        public override void _Process(float delta)
        {
            if (!_displayTimerActive)
            {
                return;
            }

            _displayTimer -= delta;
            if (_displayTimer <= 0)
            {
                _displayTimerActive = false;
                ClearAll();
            }
        }

        #region External Functions

        public void DisplaySingleStringTimed(string dialogue, float displayTime)
        {
            DisplaySingleString(dialogue);
            _displayTimerActive = true;
            _displayTimer = displayTime;
        }

        public void DisplaySingleString(string dialogue)
        {
            ClearAll();

            SetVisible(true);
            _singleDialogueHolder.SetVisible(true);
            _singleDialogue.DisplayString(dialogue);
        }

        public void ClearSingleDialogue()
        {
            _singleDialogue.ClearString();
            _singleDialogueHolder.SetVisible(false);
            SetVisible(false);
        }

        public void DisplayMultiDialogue(string[] dialogues)
        {
            ClearAll();

            SetVisible(true);
            _multiDialogueHolder.SetVisible(true);
            for (int i = 0; i < _dialogues.Count; i++)
            {
                _dialogues[i].GetTextTyper().DisplayString(dialogues[i]);
            }
        }

        public void ClearMultiDialogue()
        {
            foreach (TyperDialogue textTyper in _dialogues)
            {
                textTyper.GetTextTyper().ClearString();
            }
            _multiDialogueHolder.SetVisible(false);
            SetVisible(false);
        }

        #endregion

        #region Utility Functions

        private void ClearAll()
        {
            foreach (TyperDialogue textTyper in _dialogues)
            {
                textTyper.GetTextTyper().ClearString();
            }
            _multiDialogueHolder.SetVisible(false);

            _singleDialogue.ClearString();
            _singleDialogueHolder.SetVisible(false);
            SetVisible(false);
        }

        #endregion
    }
}