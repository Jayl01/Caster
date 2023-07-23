using AnotherLib;
using AnotherLib.Collision;
using AnotherLib.Input;
using AnotherLib.Utilities;
using Caster.Effects;
using Caster.Entities.Enemies;
using Caster.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Caster.Entities.Projectiles
{
    public class ElectricRune : Projectile
    {
        public static Texture2D runeSymbolTexture;
        public static Texture2D baseRuneTexture;
        public static Texture2D baseRuneInnerCircleTexture;

        private readonly Point Size = new Point(24);
        private readonly Color RuneColor = Color.Yellow;
        private int spawnTimer = 0;
        private bool active = true;
        private float baseRuneRotation;
        private List<ZapPosition> zapPositions = new List<ZapPosition>();
        private int randomZapTimer = 0;
        private int randomZapTime = 60;
        private int soundPlayTimer = 0;
        private int zapCooldownTimer = 0;

        private struct ZapPosition
        {
            public int lifeTime;
            public Vector2 position;

            public ZapPosition(int lifeTime, Vector2 zapPosition)
            {
                this.lifeTime = lifeTime;
                position = zapPosition;
            }
        }


        public override CollisionType collisionType => CollisionType.FriendlyProjectiles;
        public override CollisionType[] colliderTypes => new CollisionType[1] { CollisionType.Enemies };

        public static void NewElectricRune(Vector2 position)
        {
            ElectricRune electricRune = new ElectricRune();
            electricRune.position = position;
            electricRune.Initialize();
            Main.activeProjectiles.Add(electricRune);
        }

        public override void Initialize()
        {
            active = true;
            hitbox = new Rectangle((int)position.X, (int)position.Y, Size.X, Size.Y);
            soundPlayTimer = 42;
        }

        public override void Update()
        {
            if (!active)
            {
                spawnTimer -= 5;
                if (spawnTimer <= 0)
                    DestroyInstance();
                return;
            }

            if (!InputManager.IsMouseLeftHeld() || DetectTileCollisionsByCollisionStyle(GameData.MouseWorldPosition))
            {
                active = false;
                return;
            }

            if (spawnTimer < 60)
                spawnTimer++;
            if (zapCooldownTimer > 0)
                zapCooldownTimer--;
            if (soundPlayTimer > 0)
            {
                soundPlayTimer--;
                if (soundPlayTimer <= 0)
                {
                    soundPlayTimer += 42;
                    SoundPlayer.PlaySoundFromOtherSource(Sounds.ElectricRune_Active, this, 20, 1f);
                }
            }

            randomZapTimer++;
            if (randomZapTimer >= randomZapTime)
            {
                randomZapTimer = 0;
                randomZapTime = Main.random.Next(20, 80 + 1);
                Vector2 zapOffset = new Vector2(Main.random.Next(-8 * 16, (8 * 16) + 1), Main.random.Next(-8 * 16, (8 * 16) + 1));
                int amountOfZapSmoke = Main.random.Next(1, 3 + 1);
                for (int i = 0; i < amountOfZapSmoke; i++)
                {
                    Vector2 smokePosition = position + zapOffset + (new Vector2(Main.random.Next(0, Size.X + 1), Main.random.Next(0, Size.Y + 1)) - ((Size.ToVector2() / 2f))) * (spawnTimer / 60f);
                    Vector2 smokeVelocity = new Vector2(Main.random.Next(-4, 4 + 1), Main.random.Next(-4, 4 + 1)) / 12f;
                    Smoke.NewSmokeParticle(smokePosition, smokeVelocity, Color.Yellow, Color.Orange, 15, 20, 10, foreground: true);
                }
                zapPositions.Add(new ZapPosition(5, position + zapOffset));
                SoundPlayer.PlaySoundFromOtherSource(Sounds.ElectricRune_Shock, position + zapOffset, 16, soundPitch: Main.random.Next(-3, 3 + 1) / 10f);
            }

            Vector2 velocity = Vector2.Zero;
            if (Vector2.Distance(GameData.MouseWorldPosition, position) > 4f)
            {
                velocity = GameData.MouseWorldPosition - position;
                velocity.Normalize();
                velocity *= 0.8f;
            }

            position += velocity;
            hitbox.X = (int)position.X;
            hitbox.Y = (int)position.Y;
            baseRuneRotation += MathHelper.ToRadians(0.5f);

            int amountOfSmoke = Main.random.Next(1, 3 + 1);
            for (int i = 0; i < amountOfSmoke; i++)
            {
                Vector2 smokePosition = position + (new Vector2(Main.random.Next(0, Size.X + 1), Main.random.Next(0, Size.Y + 1)) - ((Size.ToVector2() / 2f))) * (spawnTimer / 60f);
                Vector2 smokeVelocity = new Vector2(Main.random.Next(-4, 4 + 1), Main.random.Next(-4, 4 + 1)) / 12f;
                Smoke.NewSmokeParticle(smokePosition, smokeVelocity, Color.Yellow, Color.Orange, 15, 20, 10, foreground: true);
            }

            if (spawnTimer >= 60 && zapCooldownTimer <= 0)
            {
                int currentZaps = 0;
                CollisionBody[] enemies = Main.activeEnemies.ToArray();
                foreach (Enemy enemy in enemies)
                {
                    if (currentZaps < 3 && Vector2.Distance(enemy.position, position) < 8f * 16f)
                    {
                        int amountOfZapSmoke = Main.random.Next(3, 5 + 1);
                        for (int i = 0; i < amountOfZapSmoke; i++)
                        {
                            Vector2 smokePosition = enemy.position + (new Vector2(Main.random.Next(0, Size.X + 1), Main.random.Next(0, Size.Y + 1)) - ((Size.ToVector2() / 2f))) * (spawnTimer / 60f);
                            Vector2 smokeVelocity = new Vector2(Main.random.Next(-2, 2 + 1), Main.random.Next(-2, 2 + 1)) / 12f;
                            Smoke.NewSmokeParticle(smokePosition, smokeVelocity, Color.Yellow, Color.Orange, 15, 20, 10, foreground: true);
                        }
                        Vector2 zapPosition = enemy.position + new Vector2(Main.random.Next(0, enemy.EnemyWidth), Main.random.Next(0, enemy.EnemyHeight));
                        zapPositions.Add(new ZapPosition(5, zapPosition));
                        SoundPlayer.PlaySoundFromOtherSource(Sounds.ElectricRune_Shock, enemy.position, 16, soundPitch: Main.random.Next(-3, 3 + 1) / 10f);
                        enemy.HurtEnemy(Main.random.Next(3, 6 + 1) + Enemy.EnemiesKilled);
                        enemy.ThrowEnemy(new Vector2(Main.random.Next(-2, 2 + 1), Main.random.Next(-2, 2 + 1)) / 10f);
                        currentZaps++;
                    }
                }
                if (currentZaps > 0)
                {
                    randomZapTimer = 0;
                    zapCooldownTimer += 8;
                }
            }

            if (zapPositions.Count != 0)
            {
                for (int i = 0; i < zapPositions.Count; i++)
                {
                    ZapPosition zapPosition = zapPositions[i];
                    if (zapPosition.lifeTime - 1 <= 0)
                    {
                        zapPositions.RemoveAt(i);
                        i -= 1;
                    }
                    else
                    {
                        zapPosition.lifeTime -= 1;
                        zapPositions[i] = zapPosition;
                        if (Vector2.Distance(Main.currentPlayer.playerCenter, zapPosition.position) <= 4 * 16)
                            Main.camera.ShakeCamera(2, 1);
                    }
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (zapPositions.Count != 0)
            {
                for (int i = 0; i < zapPositions.Count; i++)
                {
                    int lineSegments = Main.random.Next(2, 5 + 1);
                    Vector2 lastSegmentPosition = position;
                    for (int j = 0; j < lineSegments; j++)
                    {
                        Vector2 segmentPosition = Vector2.Lerp(lastSegmentPosition, zapPositions[i].position, 1f / (float)lineSegments) + new Vector2(Main.random.Next(-4, 4 + 1), Main.random.Next(-4, 4 + 1));
                        if (j == lineSegments - 1)
                            segmentPosition = zapPositions[i].position;
                        DrawLine(spriteBatch, lastSegmentPosition, segmentPosition, Color.Lerp(Color.Yellow, Color.Orange, Main.random.Next(0, 100 + 1) / 100f));
                        lastSegmentPosition = segmentPosition;
                    }
                }
            }

            spriteBatch.Draw(baseRuneTexture, position, null, RuneColor * (spawnTimer / 60f), baseRuneRotation, new Vector2(12), (spawnTimer / 60f), SpriteEffects.None, 0f);
            spriteBatch.Draw(baseRuneInnerCircleTexture, position, null, RuneColor * (spawnTimer / 60f), -baseRuneRotation * 0.5f, new Vector2(12), (spawnTimer / 60f), SpriteEffects.None, 0f);
            spriteBatch.Draw(runeSymbolTexture, position, null, RuneColor * (spawnTimer / 60f), 0f, new Vector2(12), (spawnTimer / 60f), SpriteEffects.None, 0f);
        }

        public static void DrawLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color)
        {
            Vector2 lineVector = end - start;
            Vector2 drawScale = new Vector2(lineVector.Length(), 1f) * 0.5f;
            spriteBatch.Draw(Smoke.smokePixelTextures[0], start, null, color, lineVector.GetRotation(), Vector2.Zero, drawScale, SpriteEffects.None, 0f);
        }
    }
}
