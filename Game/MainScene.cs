using ComputerGameFinal.Engine;
using Microsoft.Xna.Framework;

namespace ComputerGameFinal.Game;

class MainScene : Scene
{
    GameObject player;

    public override void Setup()
    {
        player = base.AddGameObject<Player>("player");
    }

    public override void Initialize()
    {
        player.Position = new Vector2(400, 200);
        player.Scale = new Vector2(0.1f, 0.1f);
    }
}