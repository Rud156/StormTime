using System.Collections.Generic;
using Godot;

namespace StormTime.UI
{
    public class DialogueManager : Panel
    {
        [Export] public NodePath leftContainerNodePath;
        [Export] public NodePath rightContainerNodePath;
        [Export] public PackedScene dialogueIntermediatePrefab;
        [Export] public PackedScene dialogueMainPrefab;

        private Control _leftContainer;
        private Control _rightContainer;

        private List<DialogueIntermediate> _leftContainerDialogueIntermediates;
        private int _leftCounter;
        private List<DialogueIntermediate> _rightContainerDialogueIntermediates;
        private int _rightCounter;

        public override void _Ready()
        {
            _leftContainer = GetNode<Control>(leftContainerNodePath);
            _rightContainer = GetNode<Control>(rightContainerNodePath);

            _leftContainerDialogueIntermediates = new List<DialogueIntermediate>();
            _rightContainerDialogueIntermediates = new List<DialogueIntermediate>();
        }

        #region External Functions

        public void SetupDialogueStates(int playerWinCount, int groupWinCount)
        {
            #region Cleanup

            for (int i = 0; i < _leftContainer.GetChildCount(); i++)
            {
                _leftContainer.GetChild(i).QueueFree();
            }

            for (int i = 0; i < _rightContainer.GetChildCount(); i++)
            {
                _rightContainer.GetChild(i).QueueFree();
            }

            #endregion

            #region Left Container

            _leftContainerDialogueIntermediates.Clear();
            _leftCounter = 0;

            Control leftDialogueMainInstance = (Control)dialogueMainPrefab.Instance();
            _leftContainer.AddChild(leftDialogueMainInstance);

            for (int i = 0; i < playerWinCount; i++)
            {
                DialogueIntermediate leftDialogueIntermediateInstance =
                    (DialogueIntermediate)dialogueIntermediatePrefab.Instance();
                _leftContainerDialogueIntermediates.Add(leftDialogueIntermediateInstance);
                _leftContainer.AddChild(leftDialogueIntermediateInstance);
            }

            #endregion

            #region Right Container

            _rightContainerDialogueIntermediates.Clear();
            _rightCounter = groupWinCount - 1;

            for (int i = 0; i < groupWinCount; i++)
            {
                DialogueIntermediate rightDialogueIntermediateInstance =
                    (DialogueIntermediate)dialogueIntermediatePrefab.Instance();
                _rightContainerDialogueIntermediates.Add(rightDialogueIntermediateInstance);
                _rightContainer.AddChild(rightDialogueIntermediateInstance);
            }

            Control rightDialogueMainInstance = (Control)dialogueMainPrefab.Instance();
            _rightContainer.AddChild(rightDialogueMainInstance);

            #endregion
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
    }
}