using AnotherLib;
using AnotherLib.Utilities;
using Caster.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Caster.Entities.Projectiles
{
    public class BombRune : Projectile
    {
        public static Texture2D runeSymbolTexture;

        private readonly Point Size = new Point(24);
        private readonly Color RuneColor = Color.LightBlue;
        private int spawnTimer = 0;
        private int decayTimer = 180;
        private float baseRuneRotation;
        private bool droppedBomb = false;
        //private float 

        public override CollisionType collisionType => CollisionType.FriendlyProjectiles;
        public override CollisionType[] colliderTypes => new CollisionType[1] { CollisionType.Enemies };

        public static void NewBombRune(Vector2 position)
        {
            BombRune bombRune = new BombRune();
            bombRune.position = position;
            bombRune.Initialize();
            Main.activeProjectiles.Add(bombRune);
        }

        public override void Initialize()
        {
            hitbox = new Rectangle((int)position.X, (int)position.Y, Size.X, Size.Y);
        }

        public override void Update()
        {
            int amountOfSmoke = Main.random.Next(1, 3 + 1);
            for (int i = 0; i < amountOfSmoke; i++)
            {
                float scale = MathHelper.Clamp(spawnTimer / 60f, 0f, 1f);
                Vector2 smokePosition = position + (new Vector2(Main.random.Next(0, Size.X + 1), Main.random.Next(0, Size.Y + 1)) - ((Size.ToVector2() / 2f))) * scale;
                Vector2 smokeVelocity = new Vector2(Main.random.Next(-4, 4 + 1), Main.random.Next(-4, 4 + 1)) / 12f;
                Smoke.NewSmokeParticle(smokePosition, smokeVelocity, Color.White, Color.LightBlue, 15, 20, 10, foreground: true);
            }

            if (spawnTimer < 120)
                spawnTimer++;
            else
            {
                if (!droppedBomb)
                {
                    droppedBomb = true;
                    SpiritBomb.NewSpiritBomb(position, new Vector2(0f, 1f));
                }
                decayTimer--;
                if (decayTimer <= 0)
                    DestroyInstance();
            }


            baseRuneRotation += MathHelper.ToRadians(0.7f);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            float scale = MathHelper.Clamp((spawnTimer / 60f) * (decayTimer / 180f), 0f, 1f);
            spriteBatch.Draw(ElectricRune.baseRuneTexture, position, null, RuneColor * scale, baseRuneRotation, new Vector2(12), scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(ElectricRune.baseRuneInnerCircleTexture, position, null, RuneColor * scale, -baseRuneRotation * 0.5f, new Vector2(12), scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(runeSymbolTexture, position, null, RuneColor * scale, baseRuneRotation * 0.2f, new Vector2(12), scale, SpriteEffects.None, 0f);
        }
    }
}
