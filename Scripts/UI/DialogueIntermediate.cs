using Godot;

namespace StormTime.UI
{
    public class DialogueIntermediate : TextureRect
    {
        [Export] public NodePath fillerNodePath;

        private TextureRect _fillerTextureRect;

        public override void _Ready() => _fillerTextureRect = GetNode<TextureRect>(fillerNodePath);

        public void DisplayFillerTexture() => _fillerTextureRect.Visible = true;

        public void HideFillerTexture() => _fillerTextureRect.Visible = false;
    }
}