using System.Collections.Generic;
using Godot;

namespace StormTime.UI
{
    public class DialogueUiManager : Panel
    {
        [Export] public NodePath leftContainerNodePath;
        [Export] public NodePath rightContainerNodePath;

        // Dialogues
        [Export] public NodePath dialogueContainerNodePath;
        [Export] public NodePath dialogueQuestionNodePath;
        [Export] public Godot.Collections.Array<NodePath> dialogueKeyTextNodePaths;

        // Ending Dialogues
        [Export] public NodePath endingDialogueContainerNodePath;
        [Export] public NodePath endingDialogueLabelNodePath;

        // Prefabs
        [Export] public PackedScene dialogueIntermediatePackedScene;
        [Export] public PackedScene dialogueMainPackedScene;

        private Control _endingDialogueContainer;
        private Label _endingDialogueLabel;

        private Control _dialogueContainer;
        private Label _dialogueQuestion;
        private List<DialogueKeyTextDisplay> _dialogueKeyTexts;

        private Control _leftContainer;
        private Control _rightContainer;

        private List<DialogueIntermediate> _leftContainerDialogueIntermediates;
        private int _leftCounter;
        private List<DialogueIntermediate> _rightContainerDialogueIntermediates;
        private int _rightCounter;

        public override void _Ready()
        {
            _leftContainerDialogueIntermediates = new List<DialogueIntermediate>();
            _rightContainerDialogueIntermediates = new List<DialogueIntermediate>();
            _dialogueKeyTexts = new List<DialogueKeyTextDisplay>();

            _leftContainer = GetNode<Control>(leftContainerNodePath);
            _rightContainer = GetNode<Control>(rightContainerNodePath);

            _endingDialogueContainer = GetNode<Control>(endingDialogueContainerNodePath);
            _endingDialogueLabel = GetNode<Label>(endingDialogueLabelNodePath);

            _dialogueContainer = GetNode<Control>(dialogueContainerNodePath);
            _dialogueQuestion = GetNode<Label>(dialogueQuestionNodePath);
            foreach (var dialogueKeyText in dialogueKeyTextNodePaths)
            {
                _dialogueKeyTexts.Add(GetNode<DialogueKeyTextDisplay>(dialogueKeyText));
            }
        }

        #region External Functions

        #region Dialogue Questions

        public void SetDialogueQuestionsAndOptions(string question, string[] possibleAnswers)
        {
            _dialogueQuestion.SetText(question);
            _dialogueQuestion.SetVisible(true);

            for (int i = 0; i < _dialogueKeyTexts.Count; i++)
            {
                string key = "A";
                switch (i)
                {
                    case 0:
                        key = "A";
                        break;

                    case 1:
                        key = "W";
                        break;

                    case 2:
                        key = "D";
                        break;
                }

                _dialogueKeyTexts[i].SetAndDisplayDialogueAndKey(key, possibleAnswers[i]);
            }
        }

        public void ClearDialogueQuestionAndOptions()
        {
            _dialogueQuestion.SetText(string.Empty);
            _dialogueQuestion.SetVisible(false);

            foreach (DialogueKeyTextDisplay dialogueKeyTextDisplay in _dialogueKeyTexts)
            {
                dialogueKeyTextDisplay.ClearAndHideDialogueAndKey();
            }
        }

        #endregion

        #region Ending Dialogue

        public void ShowEndingDialogue(string dialogue)
        {
            ClearDialogueQuestionAndOptions();

            _endingDialogueContainer.SetVisible(true);
            _endingDialogueLabel.SetText(dialogue);
        }

        public void ClearEndingDialogue()
        {
            _endingDialogueLabel.SetText(string.Empty);
            _endingDialogueContainer.SetVisible(false);
        }

        #endregion

        #region Dialogue Range State

        public void SetupDialogueStates(int playerWinCount, int groupWinCount)
        {
            #region Left Container

            _leftCounter = 0;

            Control leftDialogueMainInstance = (Control)dialogueMainPackedScene.Instance();
            _leftContainer.AddChild(leftDialogueMainInstance);

            for (int i = 0; i < playerWinCount; i++)
            {
                DialogueIntermediate leftDialogueIntermediateInstance =
                    (DialogueIntermediate)dialogueIntermediatePackedScene.Instance();
                _leftContainerDialogueIntermediates.Add(leftDialogueIntermediateInstance);
                _leftContainer.AddChild(leftDialogueIntermediateInstance);
            }

            #endregion

            #region Right Container

            _rightCounter = groupWinCount - 1;

            for (int i = 0; i < groupWinCount; i++)
            {
                DialogueIntermediate rightDialogueIntermediateInstance =
                    (DialogueIntermediate)dialogueIntermediatePackedScene.Instance();
                _rightContainerDialogueIntermediates.Add(rightDialogueIntermediateInstance);
                _rightContainer.AddChild(rightDialogueIntermediateInstance);
            }

            Control rightDialogueMainInstance = (Control)dialogueMainPackedScene.Instance();
            _rightContainer.AddChild(rightDialogueMainInstance);

            #endregion
        }

        public void ClearDialogueStates()
        {
            _leftContainerDialogueIntermediates.Clear();
            _rightContainerDialogueIntermediates.Clear();

            for (int i = 0; i < _leftContainer.GetChildCount(); i++)
            {
                _leftContainer.GetChild(i).QueueFree();
            }

            for (int i = 0; i < _rightContainer.GetChildCount(); i++)
            {
                _rightContainer.GetChild(i).QueueFree();
            }
        }

        public bool IncrementPlayerWin()
        {
            _leftContainerDialogueIntermediates[_leftCounter].DisplayFillerTexture();
            _leftCounter += 1;

            return _leftCounter >= _leftContainerDialogueIntermediates.Count;
        }

        public bool IncrementGroupWin()
        {
            _rightContainerDialogueIntermediates[_rightCounter].DisplayFillerTexture();
            _rightCounter -= 1;

            return _rightCounter <= 0;
        }

        #endregion

        public void ClearAndHideDialoguePanel()
        {
            ClearDialogueQuestionAndOptions();
            ClearEndingDialogue();
            ClearDialogueStates();

            HideDialoguePanel();
        }

        public void DisplayDialoguePanel() => SetVisible(true);

        public void HideDialoguePanel() => SetVisible(false);

        #endregion
    }
}