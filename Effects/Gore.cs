using Caster.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Caster.Effects
{
    public class Gore : VisualEffect
    {
        public const int AmountOfGoreTextures = 4;
        public const byte Caster_1 = 0;
        public const byte Caster_2 = 1;

        public static Texture2D[] goreTextures;
        private readonly Vector2[] GoreSizes = new Vector2[2]
        {
            new Vector2(25, 9),
            new Vector2(27, 9),
        };

        private int goreType = 0;
        private Vector2 goreVelocity;
        private bool advancedDraw = false;
        private bool stoppedFalling = false;
        private Vector2 center;
        private SpriteEffects spriteEffect;

        public static Gore NewGore(int type, Vector2 position)
        {
            Gore newGore = new Gore();
            newGore.goreType = type;
            newGore.position = position;
            Main.activeBackgroundEffects.Add(newGore);
            return newGore;
        }

        public static Gore NewGore(int type, Vector2 position, Vector2 velocity)
        {
            Gore newGore = new Gore();
            newGore.goreType = type;
            newGore.position = position;
            newGore.goreVelocity = velocity;
            Main.activeBackgroundEffects.Add(newGore);
            return newGore;
        }

        public override void Update()
        {
            if (goreVelocity != Vector2.Zero)
            {
                position += goreVelocity;
                center = position + (GoreSizes[goreType] / 2f);
                goreVelocity.X *= 0.98f;
                if (!stoppedFalling && goreVelocity.Y < 6f)
                    goreVelocity.Y += 0.03f;
                if (goreVelocity.X > 0.1)
                {
                    advancedDraw = true;
                    spriteEffect = SpriteEffects.FlipHorizontally;
                }
                Vector2 searchVelocity = goreVelocity;
                searchVelocity.Normalize();
                searchVelocity *= GoreSizes[goreType] / 2f;
                if (DetectTileCollisionsByCollisionStyle(center + new Vector2(0f, searchVelocity.Y)))
                {
                    goreVelocity.Y = 0f;
                    stoppedFalling = true;
                }
                if (DetectTileCollisionsByCollisionStyle(center + new Vector2(searchVelocity.X, 0f)))
                    goreVelocity.X = 0f;

                //gore is snapped to the nearest pixel if the velocity is practially zero
                if (Math.Abs(goreVelocity.X) < 0.001 && Math.Abs(goreVelocity.Y) < 0.001)
                {
                    goreVelocity = Vector2.Zero;
                    position = Vector2.Round(position);
                }
            }
        }

        public bool DetectTileCollisionsByCollisionStyle(Vector2 position)
        {
            Point positionPoint = (position / 16).ToPoint();
            if (!ChunkLoader.CheckForSafeTileCoordinates(positionPoint))
                return false;

            if (WorldClass.activeWorldData.tiles[positionPoint.X, positionPoint.Y].isCollideable)
                return true;

            return false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!advancedDraw)
                spriteBatch.Draw(goreTextures[goreType], position, Color.White);
            else
                spriteBatch.Draw(goreTextures[goreType], position, null, Color.White, 0f, Vector2.Zero, 1f, spriteEffect, 0f);
        }
    }
}
