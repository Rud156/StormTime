using System.Collections.Generic;
using Godot;

namespace StormTime.UI
{
    public class DialogueUiManager : Panel
    {
        private PackedScene _dialogueIntermediatePrefab;
        private PackedScene _dialogueMainPrefab;

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

            if (Instance == null)
            {
                Instance = this;
            }
        }

        public void Init(Control leftContainer, Control rightContainer,
            PackedScene dialogueIntermediatePrefab, PackedScene dialogueMainPrefab)
        {
            _dialogueIntermediatePrefab = dialogueIntermediatePrefab;
            _dialogueMainPrefab = dialogueMainPrefab;

            _leftContainer = leftContainer;
            _rightContainer = rightContainer;
        }

        #region External Functions

        public void SetDialogueQuestionsAndOptions(string question, string[] possibleAnswers)
        {

        }

        public void ShowEndingDialogue(string dialogue)
        {

        }

        public void ClearDialogueQuestionAndOptions()
        {

        }

        public void SetupDialogueStates(int playerWinCount, int groupWinCount)
        {
            #region Left Container

            _leftCounter = 0;

            Control leftDialogueMainInstance = (Control)_dialogueMainPrefab.Instance();
            _leftContainer.AddChild(leftDialogueMainInstance);

            for (int i = 0; i < playerWinCount; i++)
            {
                DialogueIntermediate leftDialogueIntermediateInstance =
                    (DialogueIntermediate)_dialogueIntermediatePrefab.Instance();
                _leftContainerDialogueIntermediates.Add(leftDialogueIntermediateInstance);
                _leftContainer.AddChild(leftDialogueIntermediateInstance);
            }

            #endregion

            #region Right Container

            _rightCounter = groupWinCount - 1;

            for (int i = 0; i < groupWinCount; i++)
            {
                DialogueIntermediate rightDialogueIntermediateInstance =
                    (DialogueIntermediate)_dialogueIntermediatePrefab.Instance();
                _rightContainerDialogueIntermediates.Add(rightDialogueIntermediateInstance);
                _rightContainer.AddChild(rightDialogueIntermediateInstance);
            }

            Control rightDialogueMainInstance = (Control)_dialogueMainPrefab.Instance();
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

        public void DisplayDialoguePanel() => Visible = true;

        public void HideDialoguePanel() => Visible = false;

        #endregion

        #region Singleton

        public static DialogueUiManager Instance;

        #endregion
    }
}