using Godot;
using StormTime.UI;

namespace StormTime.Scene.BossScene
{
    public class BossSceneGameManager : Node
    {
        public override void _Ready()
        {
            Fader.FaderReady faderActivate = null;
            faderActivate = () =>
           {
               Fader.instance.StartFading(false, new Color(1, 1, 1));
               Fader.faderReady -= faderActivate;
           };

            Fader.faderReady += faderActivate;

            GD.Randomize();
        }
    }
}