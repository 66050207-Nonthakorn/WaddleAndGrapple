using ComputerGameFinal.Engine;
using ComputerGameFinal.Engine.Components;
using ComputerGameFinal.Engine.Components.Tile;
using ComputerGameFinal.Engine.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ComputerGameFinal.Game.Example;

class MainScene : Scene
{
    GameObject player;
    GameObject cameraObject;
    GameObject tilemapObject;

    public override void Setup()
    {
        // Create tilemap first
        tilemapObject = base.AddGameObject<GameObject>("tilemap");
        
        // Create camera
        cameraObject = base.AddGameObject<GameObject>("camera");
        var camera = cameraObject.AddComponent<Camera2D>();
        camera.SetViewport(new Viewport(0, 0, 800, 600));
        camera.Zoom = 1f;
        camera.SmoothFollow = false;
        // camera.FollowSpeed = 2f;

        base.Camera = camera;
        
        player = base.AddGameObject<Player>("player");
        player.Position = new Vector2(100, 100);
        player.Scale = new Vector2(0.75f, 0.75f);

        camera.FollowTarget = player;

        // Setup tilemap
        SetupTilemap();
    }

    private void SetupTilemap()
    {
        // Load tileset texture
        var tileTexture = ResourceManager.Instance.GetTexture("Tiles/block");

        if (tileTexture != null)
        {
            // Create tilemap
            new TilemapBuilder(tilemapObject)
                .WithTileset(tileTexture, 64, 64, solidTiles: [0])
                .AddLayer("ground", 25, 19, CreateLevelData(), layerDepth: 0.1f)
                .Build();
            
            tilemapObject.Position = Vector2.Zero;
        }
    }

    private static int[] CreateLevelData()
    {
        const int width = 25;
        const int height = 19;
        
        // Use helper method to create ground layer
        int[] data = TilemapBuilder.CreateGroundLayer(width, height, groundTileId: 0, groundHeight: 3);
        
        // Add platforms using FillRect helper
        TilemapBuilder.FillRect(data, width, height, tileId: 0, startX: 5, startY: 12, rectWidth: 5, rectHeight: 1);
        TilemapBuilder.FillRect(data, width, height, tileId: 0, startX: 15, startY: 8, rectWidth: 4, rectHeight: 1);
        TilemapBuilder.FillRect(data, width, height, tileId: 0, startX: 10, startY: 10, rectWidth: 3, rectHeight: 1);

        return data;
    }
}
