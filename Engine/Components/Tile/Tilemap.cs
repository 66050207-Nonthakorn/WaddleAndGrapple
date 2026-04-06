using System;
using System.Collections.Generic;
using WaddleAndGrapple.Engine.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WaddleAndGrapple.Engine.Components.Tile;

public class TilemapTileset
{
    public int FirstGid { get; set; }
    public Texture2D Texture { get; set; }
}

public class Tilemap : Component
{
    public Texture2D Tileset { get; set; } // Legacy fallback
    public List<TilemapTileset> Tilesets { get; set; } = new();

    public int SourceTileSize { get; set; }
    public int DestinationTileSize { get; set; }
    
    public float Layer { get; set; } = 0f;
    public int[,] MapData { get; set; }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (MapData == null) return;
        if (Tilesets.Count == 0 && Tileset == null) return;

        int mapRows = MapData.GetLength(0);
        int mapCols = MapData.GetLength(1);

        // Compute the world-space visible area from the active camera
        var camera = SceneManager.Instance.CurrentScene?.Camera;
        int startX = 0, startY = 0, endX = mapCols - 1, endY = mapRows - 1;

        if (camera != null)
        {
            float scaledTile = DestinationTileSize * GameObject.Scale.X;
            Rectangle visible = camera.GetVisibleArea();

            // Convert world visible rect to tile indices (relative to this tilemap's position)
            startX = Math.Max(0, (int)Math.Floor((visible.Left   - GameObject.Position.X) / scaledTile));
            startY = Math.Max(0, (int)Math.Floor((visible.Top    - GameObject.Position.Y) / scaledTile));
            endX   = Math.Min(mapCols - 1, (int)Math.Ceiling((visible.Right  - GameObject.Position.X) / scaledTile));
            endY   = Math.Min(mapRows - 1, (int)Math.Ceiling((visible.Bottom - GameObject.Position.Y) / scaledTile));
        }

        Vector2 tileScale = new Vector2((float)DestinationTileSize / SourceTileSize) * GameObject.Scale;

        for (int y = startY; y <= endY; y++)
        {
            for (int x = startX; x <= endX; x++)
            {
                int gid = MapData[y, x];
                if (gid <= 0 && gid == -1) continue; // Legacy empty
                if (gid == 0) continue; // New empty

                Texture2D tex = null;
                int localId = gid;

                if (Tilesets.Count > 0)
                {
                    TilemapTileset matched = null;
                    for (int i = Tilesets.Count - 1; i >= 0; i--)
                    {
                        if (gid >= Tilesets[i].FirstGid)
                        {
                            matched = Tilesets[i];
                            break;
                        }
                    }
                    if (matched != null)
                    {
                        tex = matched.Texture;
                        localId = gid - matched.FirstGid;
                    }
                }
                else
                {
                    tex = Tileset;
                    // Legacy maps assume gid was already converted to 0-based
                    localId = gid;
                }

                if (tex == null) continue;

                int tilesetColumns = tex.Width / SourceTileSize;
                if (tilesetColumns <= 0) continue;

                int tileX = localId % tilesetColumns;
                int tileY = localId / tilesetColumns;

                Rectangle sourceRect = new Rectangle(tileX * SourceTileSize, tileY * SourceTileSize, SourceTileSize, SourceTileSize);
                Vector2 position = new Vector2(x * DestinationTileSize, y * DestinationTileSize) * GameObject.Scale;
                Vector2 drawPosition = position + GameObject.Position;
                drawPosition = new Vector2(
                    (float)Math.Round(drawPosition.X),
                    (float)Math.Round(drawPosition.Y));

                spriteBatch.Draw(tex, drawPosition, sourceRect, Color.White,
                    GameObject.Rotation.Z, Vector2.Zero, tileScale, SpriteEffects.None, Layer);
            }
        }
    }
}
