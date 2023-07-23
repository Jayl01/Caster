using Caster.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Caster.Entities.Projectiles
{
    public class SpiritWall : Projectile
    {
        public static Texture2D spiritWallTexture;

        private readonly Point Size = new Point(9, 26);
        private const float LifeTime = (3 * 60) + 30;
        private int spawnTimer = 0;
        private int lifeTimer = 0;
        private bool playedSlamSound = false;
        private int direction;
        private Vector2 originalPosition;
        private Vector2 expectedEndPosition;


        public override CollisionType collisionType => CollisionType.FriendlyProjectiles;
        public override CollisionType[] colliderTypes => new CollisionType[1] { CollisionType.Enemies };

        public static void NewSpiritWall(Vector2 position, int direction)
        {
            SpiritWall spiritWall = new SpiritWall();
            spiritWall.originalPosition = spiritWall.position = position;
            spiritWall.direction = direction;
            spiritWall.expectedEndPosition = position + new Vector2(8f * 16f * direction, 0f);
            spiritWall.Initialize();
            Main.activeProjectiles.Add(spiritWall);
        }

        public override void Initialize()
        {
            hitbox = new Rectangle((int)position.X, (int)position.Y, Size.X, Size.Y);
        }

        public override void Update()
        {
            if (spawnTimer < 60)
                spawnTimer++;
            lifeTimer++;
            if (lifeTimer >= LifeTime)
                DestroyInstance();

            int amountOfSpiritWallSmoke = Main.random.Next(2, 5 + 1);
            for (int i = 1; i < amountOfSpiritWallSmoke; i++)
            {
                Vector2 smokePosition = position + new Vector2(Main.random.Next(0, Size.X + 1), Main.random.Next(0, Size.Y + 1));
                Vector2 smokeVelocity = new Vector2(Main.random.Next(2, 3 + 1) / 10f * -direction, 0f) / 12f;
                Smoke.NewSmokeParticle(smokePosition, smokeVelocity, Color.White, Color.LightBlue, 30, 30, 20, foreground: true);
            }

            if (lifeTimer >= 120 && lifeTimer < 150)
            {
                position = Vector2.Lerp(originalPosition, expectedEndPosition, (float)Math.Sin(MathHelper.ToRadians(((lifeTimer - 120) / 30f) * 90f)));
                hitbox.X = (int)position.X;
                hitbox.Y = (int)position.Y;
            }
            if (lifeTimer >= 150 && !playedSlamSound)
            {
                playedSlamSound = true;
                //play slam sound
            }
            if (hitbox.Intersects(Main.currentPlayer.hitbox))
                Main.currentPlayer.Hurt();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (direction == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(spiritWallTexture, position, null, Color.White * (spawnTimer / 120f), 0f, Vector2.Zero, 1f, spriteEffects, 0f);
        }
    }
}
