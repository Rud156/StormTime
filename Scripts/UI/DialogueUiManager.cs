using System;
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

        private enum DisplayState
        {
            None,
            ClearAndDisplayQA,
            ClearAndDisplaySingle,
            DisplayQA,
            DisplaySingle,
        }

        private DisplayState _displayState;
        private float _delayTimer;
        private float _originalDelayTimer;
        private string _question;
        private string[] _possibleAnswers;

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

        public override void _Process(float delta)
        {
            if (_delayTimer > 0)
            {
                _delayTimer -= delta;

                if (_delayTimer <= 0)
                {
                    switch (_displayState)
                    {
                        case DisplayState.None:
                            // Do nothing in this case
                            // Only used for Idle State
                            break;

                        case DisplayState.ClearAndDisplayQA:
                            ClearDialogueQuestionAndOptions();
                            _delayTimer = _originalDelayTimer;
                            SetDisplayState(DisplayState.DisplayQA);
                            break;

                        case DisplayState.ClearAndDisplaySingle:
                            ClearSingleDialogue();
                            _delayTimer = _originalDelayTimer;
                            SetDisplayState(DisplayState.DisplaySingle);
                            break;

                        case DisplayState.DisplayQA:
                            ShowDialogueQuestionsAndOptions(_question, _possibleAnswers);
                            SetDisplayState(DisplayState.None);
                            break;

                        case DisplayState.DisplaySingle:
                            ShowSingleDialogue(_question);
                            SetDisplayState(DisplayState.None);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }

        #region External Functions

        #region Dialogue Questions

        public void ShowDialogueQuestionsAndOptions(string question, string[] possibleAnswers)
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

        public void ClearAndShowDialogOptionsDelayed(string question, string[] possibleAnswers, float delayTime)
        {
            _delayTimer = delayTime;
            _originalDelayTimer = delayTime;

            _question = question;
            _possibleAnswers = possibleAnswers;

            SetDisplayState(DisplayState.ClearAndDisplayQA);
        }

        #endregion

        #region Single Dialogue

        public void ShowSingleDialogue(string dialogue)
        {
            ClearDialogueQuestionAndOptions();

            _endingDialogueContainer.SetVisible(true);
            _endingDialogueLabel.SetText(dialogue);
        }

        public void ClearSingleDialogue()
        {
            _endingDialogueLabel.SetText(string.Empty);
            _endingDialogueContainer.SetVisible(false);
        }

        public void ClearAndShowSingleDelayed(string dialogue, float delayTimer)
        {
            _delayTimer = delayTimer;
            _originalDelayTimer = delayTimer;

            _question = dialogue;
            _possibleAnswers = new string[0];

            SetDisplayState(DisplayState.ClearAndDisplaySingle);
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
            ClearSingleDialogue();
            ClearDialogueStates();

            HideDialoguePanel();
        }

        public void DisplayDialoguePanel() => SetVisible(true);

        public void HideDialoguePanel() => SetVisible(false);

        #endregion

        #region State Management

        private void SetDisplayState(DisplayState displayState)
        {
            if (displayState == _displayState)
            {
                return;
            }

            _displayState = displayState;
        }

        #endregion
    }
}