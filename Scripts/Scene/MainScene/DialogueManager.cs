using System.Collections.Generic;
using Godot;
using StormTime.Enemy.Groups;
using StormTime.UI;
using StormTime.Utils;

namespace StormTime.Scene.MainScene
{
    public class DialogueManager : Node
    {
        #region Data Holders

        private const string DIALOGUE_PATH = "";

        private struct DialogueAnswer
        {
            public string answer;
            public int nextInIndex;
            public bool isCorrectAnswer;
        }

        private struct DialogueStructure
        {
            public string question;
            public DialogueAnswer[] dialogueAnswers;
            public int endingDialogueIndex;
            public bool isEndingDialogue;
        }

        #endregion

        #region Exported Data

        [Export] public float dialogueChangeDelay;
        [Export] public float initialDialogueStartDelay;

        #endregion

        private Dictionary<int, DialogueStructure> _dialogues;

        // All Dialogues
        private List<int> _startDialogues;
        private List<int> _currentStartDialogues;
        private float _currentDialogueWaitTime = -1;

        // OnGoing Dialogue
        private bool _dialogueSequenceStarted;
        private DialogueStructure _onGoingDialogue;
        private EnemyGroup _enemyGroupInteractedWith;

        public override void _Ready()
        {
            _dialogues = new Dictionary<int, DialogueStructure>();
            _startDialogues = new List<int>();
            _currentStartDialogues = new List<int>();

            _dialogueSequenceStarted = false;

            File file = new File();
            file.Open(DIALOGUE_PATH, (int)File.ModeFlags.Read);

            string dialoguesContent = file.GetAsText();
            file.Close();

            JSONParseResult jsonParseResult = JSON.Parse(dialoguesContent);

            // TODO: Generate the actual JSON and put it in the dictionary
            _currentStartDialogues.AddRange(_startDialogues);
        }

        public void StartRandomDialogueInteraction(EnemyGroup enemyGroup)
        {
            int randomDialogueIndex = _currentStartDialogues[(int)GD.Randi() % _currentStartDialogues.Count];
            _currentStartDialogues.RemoveAt(randomDialogueIndex);

            _dialogueSequenceStarted = true;
            _enemyGroupInteractedWith = enemyGroup;

            DialogueUiManager.Instance.ClearDialogueStates();

            // TODO: Setup Dialogue States based on Player Reputation
            //DialogueUiManager.Instance.SetupDialogueStates();

            _onGoingDialogue = _dialogues[randomDialogueIndex];
            _currentDialogueWaitTime = initialDialogueStartDelay;
        }

        public override void _Process(float delta)
        {
            if (!_dialogueSequenceStarted)
            {
                return;
            }

            if (_currentDialogueWaitTime > 0)
            {
                _currentDialogueWaitTime -= delta;

                if (_currentDialogueWaitTime <= 0)
                {
                    ActivateNextDialogueSet(_onGoingDialogue);
                    _currentDialogueWaitTime = -1;
                }
            }

            int dialogueIndexPressed = -1;

            if (Input.IsActionJustPressed(SceneControls.DialogueControl_1))
            {
                dialogueIndexPressed = 0;
            }
            else if (Input.IsActionJustPressed(SceneControls.DialogueControl_2))
            {
                dialogueIndexPressed = 1;
            }
            else if (Input.IsActionJustPressed(SceneControls.DialogueControl_3))
            {
                dialogueIndexPressed = 2;
            }

            if (dialogueIndexPressed != -1)
            {
                DialogueAnswer selectedAnswer = _onGoingDialogue.dialogueAnswers[dialogueIndexPressed];
                bool dialogueInteractionOver;
                bool playerWon = false;

                if (selectedAnswer.isCorrectAnswer)
                {
                    dialogueInteractionOver = DialogueUiManager.Instance.IncrementPlayerWin();
                    if (dialogueInteractionOver)
                    {
                        playerWon = true;
                    }
                }
                else
                {
                    dialogueInteractionOver = DialogueUiManager.Instance.IncrementGroupWin();
                }

                if (dialogueInteractionOver)
                {
                    string endingDialogue = _dialogues[_onGoingDialogue.endingDialogueIndex].question;
                    DialogueUiManager.Instance.ShowEndingDialogue(endingDialogue);
                    _dialogueSequenceStarted = false;

                    if (playerWon)
                    {
                        // TODO: Give Reputation or something similar
                    }
                    else
                    {
                        // TODO: Clear Dialogue States
                        _enemyGroupInteractedWith.SetPlayerAsHostile();
                    }
                }
                else
                {
                    _onGoingDialogue = _dialogues[selectedAnswer.nextInIndex];
                    _currentDialogueWaitTime = dialogueChangeDelay;
                    // TODO: Clear Dialogue States
                }
            }
        }

        #region Utility Functions

        private void ActivateNextDialogueSet(DialogueStructure dialogue)
        {
            string question = dialogue.question;
            string[] answers = new string[dialogue.dialogueAnswers.Length];

            for (int i = 0; i < dialogue.dialogueAnswers.Length; i++)
            {
                answers[i] = dialogue.dialogueAnswers[i].answer;
            }

            DialogueUiManager.Instance.SetDialogueQuestionsAndOptions(question, answers);
        }

        #endregion
    }
}