using Caster.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Caster.Effects
{
    public class Gore : VisualEffect
    {
        public const int AmountOfGoreTextures = 4;
        public const byte Turret_1 = 0;
        public const byte Turret_2 = 1;
        public const byte Turret_3 = 2;
        public const byte Goon = 3;

        public static Texture2D[] goreTextures;
        private readonly Vector2[] GoreSizes = new Vector2[4]
        {
            new Vector2(10, 7),
            new Vector2(6, 7),
            new Vector2(8, 9),
            new Vector2(24, 24)
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
                if (DetectTileCollisionsByCollisionStyle(center + (goreVelocity * (GoreSizes[goreType].Y / 2f))))
                {
                    goreVelocity.Y = 0f;
                    stoppedFalling = true;
                }

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
