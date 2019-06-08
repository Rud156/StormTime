using System;
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
            public int nextReplyIndex;
            public bool isCorrectAnswer;

            public int endingDialogueIndex; // Forced Ending Dialogue
            public int endType; // Can be Fetch Quests or Kill Quests or Something Else
        }

        private struct DialogueStructure
        {
            public string question;
            public DialogueAnswer[] dialogueAnswers; // This will be empty in the case this is an Ending Dialogue
            public bool isStartingDialogue; // Used to identify

            public int enemyInteractionCount; // Set Initial Count from Dialogues as they will decide properties
            public int playerInteractionCount; // Set Initial Count from Dialogues as they will decide properties
        }

        private enum NextActionDisplayState
        {
            ClearAll,
            DisplayQuestionsAndAnswers,
            ClearQuestionsAndAnswers,
            DisplayEnding
        }

        #endregion

        #region Exported Data

        [Export] public NodePath dialogueUiNodePath;
        [Export] public NodePath enemySpawnerNodePath;
        [Export] public float dialogueChangeDelay;
        [Export] public float initialDialogueStartDelay;

        #endregion

        private DialogueUiManager _dialogueUiManager;
        private Dictionary<int, DialogueStructure> _dialogues;
        private NextActionDisplayState _nextActionDisplayState;

        // All Dialogues
        private List<int> _startDialogues;
        private List<int> _currentStartDialogues;
        private float _currentDialogueWaitTime = -1;

        // OnGoing Dialogue
        private bool _dialogueSequenceStarted;
        private DialogueStructure _onGoingDialogue;
        private string _endingDialogue;
        private EnemyGroup _enemyGroupInteractedWith;

        public override void _Ready()
        {
            _dialogueUiManager = GetNode<DialogueUiManager>(dialogueUiNodePath);
            _dialogues = new Dictionary<int, DialogueStructure>();
            _startDialogues = new List<int>();
            _currentStartDialogues = new List<int>();

            _dialogueSequenceStarted = false;

            if (Instance == null)
            {
                Instance = this;
            }

            //LoadDialoguesFromFile();
        }

        private void LoadDialoguesFromFile()
        {
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
            StartDialogueInteraction(enemyGroup, randomDialogueIndex);
        }

        public void StartDialogueInteraction(EnemyGroup enemyGroup, int dialogueIndex)
        {
            _currentStartDialogues.RemoveAt(dialogueIndex);

            _dialogueSequenceStarted = true;
            _enemyGroupInteractedWith = enemyGroup;

            _dialogueUiManager.ClearAndHideDialoguePanel();

            // TODO: Setup Dialogue States based on Player Reputation
            //DialogueUiManager.Instance.SetupDialogueStates();

            _onGoingDialogue = _dialogues[dialogueIndex];
            _currentDialogueWaitTime = initialDialogueStartDelay;
            SetNextActionDisplayState(NextActionDisplayState.DisplayQuestionsAndAnswers);
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
                    HandleNextAction();
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
                    dialogueInteractionOver = _dialogueUiManager.IncrementPlayerWin();
                    if (dialogueInteractionOver)
                    {
                        playerWon = true;
                    }
                }
                else
                {
                    dialogueInteractionOver = _dialogueUiManager.IncrementGroupWin();
                }

                if (dialogueInteractionOver)
                {
                    _endingDialogue = _dialogues[_onGoingDialogue.dialogueAnswers[dialogueIndexPressed].endingDialogueIndex].question;
                    _currentDialogueWaitTime = dialogueChangeDelay;
                    SetNextActionDisplayState(NextActionDisplayState.DisplayEnding);
                    _dialogueSequenceStarted = false;

                    if (playerWon)
                    {
                        // TODO: Give Reputation or something similar
                    }
                    else
                    {
                        _enemyGroupInteractedWith.SetPlayerAsHostile();
                    }
                }
                else
                {
                    _onGoingDialogue = _dialogues[selectedAnswer.nextReplyIndex];
                    _currentDialogueWaitTime = dialogueChangeDelay;
                    SetNextActionDisplayState(NextActionDisplayState.DisplayQuestionsAndAnswers);
                }
            }
        }

        #region Actions Display

        private void HandleNextAction()
        {
            switch (_nextActionDisplayState)
            {
                case NextActionDisplayState.ClearAll:
                    _dialogueUiManager.ClearAndHideDialoguePanel();
                    break;

                case NextActionDisplayState.ClearQuestionsAndAnswers:
                    _dialogueUiManager.ClearDialogueQuestionAndOptions();
                    break;

                case NextActionDisplayState.DisplayEnding:
                    _dialogueUiManager.ClearAndShowSingleDelayed(_endingDialogue, dialogueChangeDelay);
                    break;

                case NextActionDisplayState.DisplayQuestionsAndAnswers:
                    ActivateNextDialogueSet(_onGoingDialogue);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ActivateNextDialogueSet(DialogueStructure dialogue)
        {
            string question = dialogue.question;
            string[] answers = new string[dialogue.dialogueAnswers.Length];

            for (int i = 0; i < dialogue.dialogueAnswers.Length; i++)
            {
                answers[i] = dialogue.dialogueAnswers[i].answer;
            }

            _dialogueUiManager.ClearAndShowDialogOptionsDelayed(question, answers, dialogueChangeDelay);
        }

        private void SetNextActionDisplayState(NextActionDisplayState nextActionDisplayState)
        {
            if (nextActionDisplayState == _nextActionDisplayState)
            {
                return;
            }

            _nextActionDisplayState = nextActionDisplayState;
        }

        #endregion

        #region Singleton

        public static DialogueManager Instance;

        #endregion
    }
}