using AnotherLib.Collision;
using AnotherLib.Utilities;
using Caster.Effects;
using Caster.Entities.Enemies;
using Caster.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Caster.Entities.Projectiles
{
    public class SpiritBomb : Projectile
    {
        public static Texture2D spiritBombTexture;

        private readonly Point Size = new Point(10, 19);
        private const float LifeTime = 8 * 60;
        private int spawnTimer = 0;
        private int lifeTimer = 0;
        private Vector2 originalVelocity;
        private float dropVelocity;

        public override CollisionType collisionType => CollisionType.FriendlyProjectiles;
        public override CollisionType[] colliderTypes => new CollisionType[1] { CollisionType.Enemies };
        public override bool CanBeThrownAround => true;

        public static void NewSpiritBomb(Vector2 position, Vector2 velocity)
        {
            SpiritBomb spiritBomb = new SpiritBomb();
            spiritBomb.position = position;
            spiritBomb.originalVelocity = velocity;
            spiritBomb.Initialize();
            Main.activeProjectiles.Add(spiritBomb);
        }

        public override void Initialize()
        {
            hitbox = new Rectangle((int)position.X, (int)position.Y, Size.X, Size.Y);
        }

        public override void Update()
        {
            if (spawnTimer < 10)
                spawnTimer++;

            UpdateThrowPhysics();
            if (throwVelocity.Y == 0f)
                dropVelocity += 0.14f;
            else
                dropVelocity = 0f;

            Vector2 velocity = originalVelocity + new Vector2(0f, dropVelocity) + throwVelocity;
            int amountOfSpiritBombSmoke = Main.random.Next(2, 6 + 1);
            for (int i = 0; i < amountOfSpiritBombSmoke; i++)
            {
                Vector2 smokePosition = position + new Vector2(Main.random.Next(0, Size.X + 1), Main.random.Next(0, Size.Y + 1));
                Vector2 smokeVelocity = velocity / 12f;
                Smoke.NewSmokeParticle(smokePosition, smokeVelocity, Color.White, Color.LightBlue, 30, 30, 20, foreground: true);
            }

            position += velocity;
            hitbox.X = (int)position.X;
            hitbox.Y = (int)position.Y;
            DetectCollisions(Main.activeEnemies);
            if (DetectTileCollisionsByCollisionStyle(position + velocity))
            {
                int amountOfSmoke = Main.random.Next(35, 62 + 1);
                for (int i = 0; i < amountOfSmoke; i++)
                {
                    Vector2 smokePosition = position + new Vector2(Main.random.Next(0, Size.X + 1), Main.random.Next(0, Size.Y + 1));
                    Vector2 smokeVelocity = velocity * (-Main.random.Next(4, 7 + 1) / 40f);
                    Smoke.NewSmokeParticle(smokePosition, smokeVelocity, Color.White, Color.LightBlue, 30, 30, 20, foreground: true);
                }
                int bombCircleSmoke = 60;
                for (int i = 0; i < bombCircleSmoke; i++)
                {
                    float angle = 360f * (i / 60f);
                    angle = MathHelper.ToRadians(angle);
                    Vector2 smokePosition = position + (new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * Main.random.Next(16, (2 * 16) + 1));
                    Vector2 smokeVelocity = smokePosition - position;
                    smokeVelocity.Normalize();
                    smokeVelocity *= (2f * 16f) - Vector2.Distance(smokePosition, position);
                    smokeVelocity *= Main.random.Next(6, 12 + 1) / 100f;
                    Smoke.NewSmokeParticle(smokePosition, smokeVelocity, Color.White, Color.LightBlue, 30, 30, 20, foreground: true);
                }

                CollisionBody[] enemies = Main.activeEnemies.ToArray();
                foreach (Enemy enemy in enemies)
                {
                    if (Vector2.Distance(enemy.position, position) < 4 * 16f)
                    {
                        float distanceScale = 1f - (Vector2.Distance(enemy.position, position) / (4f * 16f));
                        Vector2 knockBackVel = enemy.position - position;
                        knockBackVel.Normalize();
                        knockBackVel *= 8f * distanceScale;
                        enemy.ThrowEnemy(knockBackVel);
                        enemy.HurtEnemy((int)(32 * distanceScale));
                    }
                }
                float playerDistance = Vector2.Distance(Main.currentPlayer.playerCenter, position);
                if (playerDistance <= 12 * 16)
                {
                    Main.camera.ShakeCamera(3, 30);
                    if (playerDistance <= 4 * 16)
                        Main.currentPlayer.Hurt();
                }
                SoundPlayer.PlaySoundFromOtherSource(Sounds.SpiritBomb_Explosion, position, 24, 0.9f);
                DestroyInstance();
            }
        }

        public override void HandleCollisions(CollisionBody collider, CollisionType colliderType)
        {
            Enemy enemy = collider as Enemy;
            enemy.HurtEnemy(14);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(spiritBombTexture, position, null, Color.White * (spawnTimer / 10f), (originalVelocity + throwVelocity).GetRotation() - (float)(Math.PI / 2f), Size.ToVector2() / 2f, 1f, SpriteEffects.None, 0f);
        }
    }
}
