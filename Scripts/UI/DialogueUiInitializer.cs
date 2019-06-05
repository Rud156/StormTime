using Godot;

namespace StormTime.UI
{
    public class DialogueUiInitializer : Panel
    {
        [Export] public NodePath leftContainerNodePath;
        [Export] public NodePath rightContainerNodePath;
        [Export] public PackedScene dialogueIntermediatePackedScene;
        [Export] public PackedScene dialogueMainPackedScene;

        public override void _Ready()
        {
            Control leftContainer = GetNode<Control>(leftContainerNodePath);
            Control rightContainer = GetNode<Control>(rightContainerNodePath);

            DialogueUiManager.Instance.Init(leftContainer, rightContainer,
                dialogueIntermediatePackedScene, dialogueMainPackedScene);
        }
    }
}