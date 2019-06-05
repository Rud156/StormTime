using System.Collections.Generic;
using Godot;

namespace StormTime.Scene.MainScene
{
    public class DialogueManager : Node2D
    {
        private const string DIALOGUE_PATH = "";

        private struct DialogueStructure
        {
            public string question;
            public string[] possibleAnswers;
            public int nextInSequence;
            public bool isEnd;
        }

        private Dictionary<int, DialogueStructure> dialogues;

        private List<int> startDialogues;
        private List<int> currentStartDialogues;

        public override void _Ready()
        {
            dialogues = new Dictionary<int, DialogueStructure>();
            startDialogues = new List<int>();
            currentStartDialogues = new List<int>();

            File file = new File();
            file.Open(DIALOGUE_PATH, (int)File.ModeFlags.Read);

            string dialoguesContent = file.GetAsText();
            file.Close();

            JSONParseResult jsonParseResult = JSON.Parse(dialoguesContent);

            // TODO: Generate the actual JSON and put it in the dictionary
            // TODO: Also put the starting dialogue and generate and copy which to actually use
        }

        public void StartRandomDialogueInteraction()
        {

        }
    }
}