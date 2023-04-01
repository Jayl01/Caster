using Caster.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Caster.World
{
    public class Tile
    {
        public static Dictionary<TileType, Texture2D> tileTextures;

        public Texture2D tileTexture;
        public Vector2 tilePosition;
        public Vector2 tileCenter;
        public CollisionStyle tileCollisionStyle;
        public Rectangle tileCollisionRect;
        public TileType tileType;
        public Color tileColor;
        public bool canUpdate = false;
        public bool isCollideable = false;
        public bool hidden = false;
        public int hiddenRoomIndex;
        public byte customLightBlockerTexture = 0;
        public bool borderTile = false;
        public bool bordersUp = false;
        public bool bordersDown = false;
        public bool bordersLeft = false;
        public bool bordersRight = false;

        private int amountOfFrames = 0;
        private int frame = 0;
        private int frameCounter = 0;
        private int frameCounterLimit = 0;
        private Rectangle animRect;
        private readonly Vector2 centerOffset = new Vector2(8f, 8f);

        public enum TileType
        {
            None,
            Grass,
            LeftGrass,
            RightGrass,
            Dirt,
            DirtToUndergroundDirt,
            UndergroundDirt
        }

        public enum CollisionStyle
        {
            None,
            Solid
        }

        public static Tile GetTileInfo(TileType tileType, Vector2 position, bool backgroundTile = false)
        {
            Tile tile = new Tile();
            if (tileType == TileType.None)
                return tile;

            switch (tileType)
            {
                case TileType.Dirt:
                    tile.tileCollisionStyle = CollisionStyle.Solid;
                    break;

                case TileType.Grass:
                    tile.tileCollisionStyle = CollisionStyle.Solid;
                    break;

                case TileType.LeftGrass:
                    tile.tileCollisionStyle = CollisionStyle.Solid;
                    break;

                case TileType.RightGrass:
                    tile.tileCollisionStyle = CollisionStyle.Solid;
                    break;

                case TileType.DirtToUndergroundDirt:
                    tile.tileCollisionStyle = CollisionStyle.Solid;
                    break;

                case TileType.UndergroundDirt:
                    tile.tileCollisionStyle = CollisionStyle.Solid;
                    break;
            }
            tile.tileType = tileType;
            tile.tileTexture = tileTextures[tileType];
            tile.tilePosition = position;
            tile.tileCenter = position + tile.centerOffset;
            tile.animRect = new Rectangle(0, 0, 16, 16);
            if (tile.tileCollisionStyle != CollisionStyle.None)
            {
                tile.isCollideable = true;
                tile.tileCollisionRect = new Rectangle((int)position.X, (int)position.Y, 16, 16);
            }
            tile.tileColor = Color.White;
            if (backgroundTile)
                tile.tileColor = Color.White * 0.5f;

            return tile;
        }

        public void ManuallyReinitializeCollisionRects()
        {
            if (tileCollisionStyle != CollisionStyle.None)
            {
                isCollideable = true;
                tileCenter = tilePosition + centerOffset;
                tileCollisionRect = new Rectangle((int)tilePosition.X, (int)tilePosition.Y, 16, 16);
            }
        }

        public void Update()
        {
            if (!canUpdate)
                return;

            if (amountOfFrames != 0)
            {
                frameCounter++;
                if (frameCounter >= frameCounterLimit)
                {
                    frame += 1;
                    frameCounter = 0;
                    if (frame >= amountOfFrames)
                        frame = 0;

                    animRect.Y = 16 * frame;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (tileTexture == null)
                return;

            spriteBatch.Draw(tileTexture, tilePosition, animRect, tileColor);
        }
    }
}