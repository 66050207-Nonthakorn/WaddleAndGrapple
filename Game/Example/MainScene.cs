using System.Collections.Generic;
using ComputerGameFinal.Engine;
using ComputerGameFinal.Engine.Components;
using ComputerGameFinal.Engine.Managers;
using ComputerGameFinal.Game;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GamePlayer = ComputerGameFinal.Game.Player;

namespace ComputerGameFinal.Game.Example;

class MainScene : Scene
{
    GamePlayer player;
    GameObject cameraObject;

    public override void Setup()
    {
        // Camera
        cameraObject = base.AddGameObject<GameObject>("camera");
        var camera = cameraObject.AddComponent<Camera2D>();
        camera.SetViewport(new Viewport(0, 0, 800, 600));
        camera.Zoom = 1f;
        camera.SmoothFollow = false;

        base.Camera = camera;

        // Player (Game.Player — ตัวเต็มพร้อม physics)
        player = base.AddGameObject<GamePlayer>("player");
        player.Position = new Vector2(200, 380);

        // วาดพื้นเป็นแถบสีสลับ 6 ช่อง ช่องละ 150×150 px
        var colors = new[] { new Color(60, 80, 120), new Color(40, 55, 90) };
        for (int i = 0; i < 6; i++)
        {
            var tile = base.AddGameObject<GameObject>($"floor_{i}");
            tile.Position = new Vector2(i * 150, 450); // top-left ของแต่ละช่อง
            tile.Scale    = new Vector2(150, 150);
            var sr        = tile.AddComponent<SpriteRenderer>();
            sr.Texture    = ResourceManager.Instance.GetTexture("pixel");
            sr.Tint       = colors[i % 2];
            sr.LayerDepth = 0.1f;
        }

        // Platforms (x, y, width, height) — y คือ top ของ platform
        var platforms = new (int x, int y, int w, int h)[]
        {
            (350, 300, 200, 30),  // platform กลาง
            (600, 180, 150, 30),  // platform สูง
        };

        var platformColors = new[] { new Color(80, 120, 80), new Color(60, 100, 60) };
        foreach (var (x, y, w, h) in platforms)
        {
            var p  = base.AddGameObject<GameObject>($"platform_{x}");
            p.Position = new Vector2(x, y);
            p.Scale    = new Vector2(w, h);
            var sr     = p.AddComponent<SpriteRenderer>();
            sr.Texture    = ResourceManager.Instance.GetTexture("pixel");
            sr.Tint       = platformColors[0];
            sr.LayerDepth = 0.1f;
        }

        // Solid rects สำหรับ collision
        var solids = new List<Microsoft.Xna.Framework.Rectangle>
        {
            new(0,   450, 900, 150),  // พื้น
            new(350, 300, 200,  30),  // platform กลาง
            new(600, 180, 150,  30),  // platform สูง
        };
        player.SetSolids(solids);

        camera.FollowTarget = player;

        // Laser 1: ค้างตลอด — แนวนอน ขวางช่วง x=450-650 ที่ระดับตัวผู้เล่น
        var laser1 = base.AddGameObject<LaserTrap>("laser_always");
        laser1.Position     = new Vector2(450, 406);
        laser1.BeamLength   = 200f;
        laser1.IsHorizontal = true;
        laser1.AlwaysOn     = true;
        laser1.Player       = player;

        // Laser 2: เปิด-ปิดตามเวลา (2s on / 1.5s off) — แนวตั้ง ขวางทางเดิน x=320
        var laser2 = base.AddGameObject<LaserTrap>("laser_timed");
        laser2.Position     = new Vector2(320, 300);
        laser2.BeamLength   = 150f;
        laser2.IsHorizontal = false;
        laser2.AlwaysOn     = false;
        laser2.OnDuration   = 2f;
        laser2.OffDuration  = 1.5f;
        laser2.Player       = player;

        // Saw trap บนพื้น — เดินซ้ายขวาระหว่าง x=100 ถึง x=300
        var saw1 = base.AddGameObject<SawTrap>("saw1");
        saw1.Position      = new Vector2(100, 420);
        saw1.MoveRange     = 200f;
        saw1.MoveSpeed     = 100f;
        saw1.MoveHorizontal = true;
        saw1.Player        = player;

        // Saw trap บน platform กลาง — เดินซ้ายขวาระหว่าง x=360 ถึง x=510
        var saw2 = base.AddGameObject<SawTrap>("saw2");
        saw2.Position      = new Vector2(360, 270);
        saw2.MoveRange     = 150f;
        saw2.MoveSpeed     = 120f;
        saw2.MoveHorizontal = true;
        saw2.Player        = player;
    }
}