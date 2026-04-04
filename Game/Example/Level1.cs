using WaddleAndGrapple.Engine;
using WaddleAndGrapple.Engine.Components;
using WaddleAndGrapple.Engine.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameGum;
using System;
using System.Collections.Generic;
using GamePlayer = WaddleAndGrapple.Game.Player;

namespace WaddleAndGrapple.Game.Example;

/// <summary>
/// Test Map v2 — 2400px wide, 4 zones, checkpoint ทุก 600px
///
///  Zone A (0–600):    Tutorial — platform ใหญ่ chain 2 ชั้น, saw ช้า
///                     Items: 3 coins บนพื้น, SpeedBoost บน plat_a2
///
///  Zone B (600–1200): Timing  — timed laser ขวางทาง, floor spike
///                     Items: 2 coins บนพื้น, 3 coins บน plat_b2, DoubleJump บน plat_b1
///
///  Zone C (1200–1800): Grapple — ช่องว่าง 220px, hook wall, ceiling spike
///                     Items: 2 coins ก่อนช่อง, 3 coins บน plat_c1, SlowTime บน plat_c2
///
///  Zone D (1800–2400): Gauntlet — wall spike pair, fast saw, always-on laser
///                     Items: 2 coins บนพื้นก่อน saw
///
/// กระโดดสูงสุด: ~126px (JumpForce 550 / Gravity 1200)
/// ยืนบนพื้น (floor y=450): player center y=420
/// </summary>
class Level1 : Scene
{
    GamePlayer player;
    GameObject cameraObject;

    static readonly Color ColFloorA   = new(50,  70, 110);
    static readonly Color ColFloorB   = new(35,  50,  85);
    static readonly Color ColPlatform = new(70, 110,  70);
    static readonly Color ColWall     = new(90,  70,  50);
    static readonly Color ColMarker   = new(60, 180, 120);
    static readonly Color ColCheckpt  = new(80,  80, 200);

    public override void Setup()
    {
        // ── Camera ────────────────────────────────────────────────────────────
        cameraObject = base.AddGameObject<GameObject>("camera");
        var camera   = cameraObject.AddComponent<Camera2D>();
        camera.SetViewport(new Viewport(0, 0,
            ScreenManager.Instance.nativeWidth,
            ScreenManager.Instance.nativeHeight));
        camera.Zoom         = 1f;
        camera.SmoothFollow = false;
        base.Camera         = camera;

        // ── Parallax Background ───────────────────────────────────────────────
        var bgObj = base.AddGameObject<GameObject>("background");
        var bg    = bgObj.AddComponent<ParallaxBackground>();
        bg.AddLayer("background", scrollFactor: 0.3f, layerDepth: 0.00f);

        // ── Player ────────────────────────────────────────────────────────────
        player = base.AddGameObject<GamePlayer>("player");
        var startSpawn = new Vector2(80, 380);
        player.Position = startSpawn;
        player.SetSpawnPoint(startSpawn);

        // ══════════════════════════════════════════════════════════════════════
        // SOLIDS
        // ══════════════════════════════════════════════════════════════════════
        var solids = new List<Microsoft.Xna.Framework.Rectangle>
        {
            // ── พื้น (ช่องว่าง Zone C: x=1300–1520) ─────────────────────────
            new(   0, 450, 1300, 150),   // Zone A + B + ต้น C
            new(1520, 450,  880, 150),   // ปลาย C + Zone D

            // ── Zone A ────────────────────────────────────────────────────────
            // plat_a1: platform ต่ำ กระโดดจากพื้นง่าย (rise 50px)
            new( 150, 370, 300, 20),
            // plat_a2: สูงขึ้นอีกชั้น chain จาก a1 (rise 70px จาก center=340)
            new( 440, 295, 260, 20),

            // ── Zone B ────────────────────────────────────────────────────────
            new( 640, 370, 300, 20),     // plat_b1
            new( 990, 295, 270, 20),     // plat_b2 (chain จาก b1, rise 70px)

            // ── Zone C ────────────────────────────────────────────────────────
            new(1320, 258,  20, 197),    // hook_wall — grapple ข้ามช่องว่าง
            new(1540, 360, 300, 20),     // plat_c1 — ลงจอดหลังข้ามช่อง
            new(1810, 280, 250, 20),     // plat_c2 — chain จาก c1 (rise 80px)

            // ── Zone D ────────────────────────────────────────────────────────
            new(1865, 360, 300, 20),     // plat_d1
            new(2195, 285, 265, 20),     // plat_d2 — chain จาก d1 (rise 75px)
        };
        player.SetSolids(solids);

        // ══════════════════════════════════════════════════════════════════════
        // VISUALS — พื้น (tile 150px)
        // ══════════════════════════════════════════════════════════════════════
        for (int i = 0; i < 9; i++)   // x = 0–1299
            AddBlock($"fl_{i}", i * 150, 450, 150, 150,
                     i % 2 == 0 ? ColFloorA : ColFloorB);
        for (int i = 0; i < 6; i++)   // x = 1520–2399
            AddBlock($"fl_{10+i}", 1520 + i * 150, 450, 150, 150,
                     i % 2 == 0 ? ColFloorA : ColFloorB);

        // ── Platform visuals ─────────────────────────────────────────────────
        AddBlock("plat_a1",   150, 370, 300, 20, ColPlatform);
        AddBlock("plat_a2",   440, 295, 260, 20, ColPlatform);
        AddBlock("plat_b1",   640, 370, 300, 20, ColPlatform);
        AddBlock("plat_b2",   990, 295, 270, 20, ColPlatform);
        AddBlock("hook_wall",1320, 258,  20, 197, ColWall);
        AddBlock("plat_c1",  1540, 360, 300, 20, ColPlatform);
        AddBlock("plat_c2",  1810, 280, 250, 20, ColPlatform);
        AddBlock("plat_d1",  1865, 360, 300, 20, ColPlatform);
        AddBlock("plat_d2",  2195, 285, 265, 20, ColPlatform);

        // ══════════════════════════════════════════════════════════════════════
        // CHECKPOINTS / SECTIONS
        // ══════════════════════════════════════════════════════════════════════
        CheckpointManager.Instance.Reset();
        CheckpointManager.Instance.RegisterSections(new[]
        {
            new Section { Id=0, LeftBound=   0, RightBound= 600,
                LeftSpawnPoint = startSpawn,
                RightSpawnPoint = new Vector2(560, 380) },
            new Section { Id=1, LeftBound= 600, RightBound=1200,
                LeftSpawnPoint  = new Vector2(620, 380),
                RightSpawnPoint = new Vector2(1170, 380) },
            new Section { Id=2, LeftBound=1200, RightBound=1800,
                LeftSpawnPoint  = new Vector2(1230, 380),
                RightSpawnPoint = new Vector2(1760, 380) },
            new Section { Id=3, LeftBound=1800, RightBound=2400,
                LeftSpawnPoint  = new Vector2(1830, 380),
                RightSpawnPoint = new Vector2(2360, 380) },
        });

        AddBlock("spawn_marker", (int)startSpawn.X, (int)startSpawn.Y - 40, 8, 50, ColMarker);
        foreach (var (nm, x) in new[] { ("cp1",600), ("cp2",1200), ("cp3",1800) })
            AddBlock(nm, x, 375, 10, 75, ColCheckpt);

        camera.FollowTarget = player;

        // ═══════════════════════════════════════════════════════════════════
        // ZONE 1 — Saw Traps
        // ═══════════════════════════════════════════════════════════════════
        var saw1 = base.AddGameObject<SawTrap>("saw1");
        saw1.Position          = new Vector2(320, 450);  // Y = floor surface (FloorMounted)
        saw1.MoveRange         = 200f;
        saw1.MoveSpeed         = 100f;
        saw1.MoveHorizontal    = true;
        saw1.Size              = SawSize.Large;
        saw1.SpriteTextureName = "Traps/Saw/LargeSaw";
        saw1.SpriteTint        = Color.White;
        saw1.Placement         = SawPlacement.FloorMounted;
        saw1.Player            = player;

        var saw2 = base.AddGameObject<SawTrap>("saw2");
        saw2.Position          = new Vector2(440, 270);
        saw2.MoveRange         = 150f;
        saw2.MoveSpeed         = 120f;
        saw2.MoveHorizontal    = true;
        saw2.Size              = SawSize.Small;
        saw2.SpriteTextureName = "Traps/Saw/SmallSaw";
        saw2.SpriteTint        = Color.White;
        saw2.Placement         = SawPlacement.Full;
        saw2.Player            = player;

        // ═══════════════════════════════════════════════════════════════════
        // ZONE 2 — Laser Traps
        // ═══════════════════════════════════════════════════════════════════
        var laser1 = base.AddGameObject<LaserTrap>("laser_always");
        laser1.Position     = new Vector2(450, 406);
        laser1.BeamLength   = 200f;
        laser1.IsHorizontal = true;
        laser1.Style        = LaserStyle.WallMounted;
        laser1.EndpointScale = 2.0f;
        laser1.BeamThicknessScale = 1.35f;
        laser1.AlwaysOn     = true;
        laser1.Player       = player;

        var laser2 = base.AddGameObject<LaserTrap>("laser_timed");
        laser2.Position     = new Vector2(320, 300);
        laser2.BeamLength   = 150f;
        laser2.IsHorizontal = false;
        laser2.Style        = LaserStyle.Floating;
        laser2.EndpointScale = 2.0f;
        laser2.BeamThicknessScale = 1.35f;
        laser2.AlwaysOn     = false;
        laser2.OnDuration   = 2.4f;
        laser2.OffDuration  = 1.8f;
        laser2.Player       = player;

        

        // ═══════════════════════════════════════════════════════════════════
        // ZONE 3 — Wall Spikes
        // ═══════════════════════════════════════════════════════════════════
        var spikeWallL = base.AddGameObject<SpikeTrap>("spike_wall_left");
        spikeWallL.Position          = new Vector2(1540, 405);
        spikeWallL.RotationAngle     = MathF.PI / 2f;
        spikeWallL.SpikeTiles        = 3;
        spikeWallL.PhaseOffset       = 0f;
        spikeWallL.SpriteTextureName = "Traps/Spike/Spike";
        spikeWallL.SpriteTint        = Color.White;
        spikeWallL.Player            = player;

        var spikeWallR = base.AddGameObject<SpikeTrap>("spike_wall_right");
        spikeWallR.Position          = new Vector2(1680, 435);
        spikeWallR.RotationAngle     = -(MathF.PI / 2f);
        spikeWallR.SpikeTiles        = 3;
        spikeWallR.PhaseOffset       = 0f;
        spikeWallR.SpriteTextureName = "Traps/Spike/Spike";
        spikeWallR.SpriteTint        = Color.White;
        spikeWallR.Player            = player;

        // ═══════════════════════════════════════════════════════════════════
        // ZONE 4 — Final Floor Spikes
        // ═══════════════════════════════════════════════════════════════════
        var spikeFinalA = base.AddGameObject<SpikeTrap>("spike_final_a");
        spikeFinalA.Position          = new Vector2(1740, 450);
        spikeFinalA.RotationAngle     = 0f;
        spikeFinalA.SpikeTiles        = 3;
        spikeFinalA.PhaseOffset       = 0f;
        spikeFinalA.SpriteTextureName = "Traps/Spike/Spike";
        spikeFinalA.SpriteTint        = Color.White;
        spikeFinalA.Player            = player;

        var spikeFinalB = base.AddGameObject<SpikeTrap>("spike_final_b");
        spikeFinalB.Position          = new Vector2(1775, 450);
        spikeFinalB.RotationAngle     = 0f;
        spikeFinalB.SpikeTiles        = 3;
        spikeFinalB.PhaseOffset       = 0.7f;
        spikeFinalB.SpriteTextureName = "Traps/Spike/Spike";
        spikeFinalB.SpriteTint        = Color.White;
        spikeFinalB.Player            = player;

        // ═══════════════════════════════════════════════════════════════════
        // ZONE 5 — Mini Demo Map
        // ═══════════════════════════════════════════════════════════════════
        var saw3 = base.AddGameObject<SawTrap>("saw_mini_1");
        saw3.Position          = new Vector2(2030, 230);
        saw3.MoveRange         = 120f;
        saw3.MoveSpeed         = 110f;
        saw3.MoveHorizontal    = true;
        saw3.Size              = SawSize.Medium;
        saw3.SpriteTextureName = "Traps/Saw/MediumSaw";
        saw3.SpriteTint        = Color.White;
        saw3.Placement         = SawPlacement.Full;
        saw3.AnimationColumns  = 4;
        saw3.Player            = player;

        var laser3 = base.AddGameObject<LaserTrap>("laser_mini_gate");
        laser3.Position     = new Vector2(2120, 420);
        laser3.BeamLength   = 160f;
        laser3.IsHorizontal = true;
        laser3.EndpointScale = 2.0f;
        laser3.BeamThicknessScale = 1.35f;
        laser3.AlwaysOn     = false;
        laser3.OnDuration   = 1.8f;
        laser3.OffDuration  = 1.3f;
        laser3.Player       = player;

        int[]   miniFloorSpikeX = { 2060, 2100, 2140 };
        float[] miniFloorPhase  = { 0f, 0.45f, 0.9f };
        for (int i = 0; i < miniFloorSpikeX.Length; i++)
        {
            var s = base.AddGameObject<SpikeTrap>($"spike_mini_floor_{i}");
            s.Position          = new Vector2(miniFloorSpikeX[i], 450);
            s.RotationAngle     = 0f;
            s.SpikeTiles        = 3;
            s.PhaseOffset       = miniFloorPhase[i];
            s.SpriteTextureName = "Traps/Spike/Spike";
            s.SpriteTint        = Color.White;
            s.Player            = player;
        }

        // ══════════════════════════════════════════════════════════════════════
        // ITEMS — Coins & Power-Ups
        //
        // Coin  (สีทอง)   : เพิ่ม CoinCount — secondary score
        // SpeedBoost (M)  : MoveSpeed ×1.5 เป็น 10 วิ
        // DoubleJump (M)  : กระโดดได้อีกครั้งในอากาศ (one-time)
        // SlowTime   (M)  : ชะลอ world ทั้งหมด 8 วิ (timer ยังเดินปกติ)
        // ══════════════════════════════════════════════════════════════════════

        // Zone A — 3 coins บนพื้น (ก่อน saw), SpeedBoost บน plat_a2
        AddCoins("cA", new[] { 90f, 130f, 170f }, y: 415f);
        AddItem<SpeedBoostPowerUp>("sboost", 555f, 258f);   // plat_a2 center

        // Zone B — 2 coins บนพื้นก่อน laser, 3 coins บน plat_b2, DoubleJump บน plat_b1
        AddCoins("cBf", new[] { 665f, 705f }, y: 415f);
        AddCoins("cBp", new[] { 1015f, 1065f, 1110f }, y: 258f);
        AddItem<DoubleJumpPowerUp>("djump", 790f, 333f);    // plat_b1 center

        // Zone C — 2 coins ก่อนช่อง, 3 coins บน plat_c1, SlowTime บน plat_c2
        AddCoins("cCf", new[] { 1230f, 1270f }, y: 415f);
        AddCoins("cCp", new[] { 1570f, 1620f, 1670f }, y: 323f);
        AddItem<SlowTimePowerUp>("slow", 1935f, 243f);      // plat_c2 center

        // Zone D — 2 coins บนพื้นก่อน wall spike
        AddCoins("cDf", new[] { 1890f, 1930f }, y: 415f);

        // ── Goal Flag ── ปลายด่าน ──────────────────────────────────────────
        var goal = base.AddGameObject<GoalFlag>("goal");
        goal.Position = new Vector2(2340, 285);  // บน plat_d2 (x=2195,w=265,y=285)
        goal.Player   = player;
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private void AddBlock(string name, int x, int y, int w, int h, Color color)
    {
        var go      = base.AddGameObject<GameObject>(name);
        go.Position = new Vector2(x, y);
        go.Scale    = new Vector2(w, h);
        var sr      = go.AddComponent<SpriteRenderer>();
        sr.Texture    = ResourceManager.Instance.GetTexture("pixel");
        sr.Tint       = color;
        sr.LayerDepth = 0.1f;
    }

    private void AddCoins(string prefix, float[] xs, float y)
    {
        for (int i = 0; i < xs.Length; i++)
        {
            var c = base.AddGameObject<Coin>($"{prefix}_{i}");
            c.Position = new Vector2(xs[i], y);
            c.SetPlayer(player);
        }
    }

    private void AddItem<T>(string name, float x, float y) where T : PowerUp, new()
    {
        var item = base.AddGameObject<T>(name);
        item.Position = new Vector2(x, y);
        item.SetPlayer(player);
    }
}
