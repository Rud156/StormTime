using Godot;
using System;

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

        public override void _Ready()
        {
            _leftContainer = GetNode<Control>(leftContainerNodePath);
            _rightContainer = GetNode<Control>(rightContainerNodePath);
        }
    }
}