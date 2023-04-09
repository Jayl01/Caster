using AnotherLib;
using AnotherLib.Collision;
using AnotherLib.Input;
using AnotherLib.Utilities;
using Caster.Effects;
using Caster.Entities.Enemies;
using Caster.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Caster.Entities.Projectiles
{
    public class GravityRune : Projectile
    {
        public static Texture2D runeSymbolTexture;

        private readonly Point Size = new Point(24);
        private readonly Color RuneColor = Color.Magenta;
        private const float RuneActiveTime = 8 * 60;
        private bool active = false;
        private float baseRuneRotation;
        private float runeStrength;
        private int activeTimer = 0;
        private float arrowRotation = 0f;
        private Vector2 throwVelocity;
        private int soundPlayCooldown = 0;

        public override CollisionType collisionType => CollisionType.FriendlyProjectiles;
        public override CollisionType[] colliderTypes => new CollisionType[1] { CollisionType.Enemies };

        public static void NewGravityRune(Vector2 position)
        {
            GravityRune gravityRune = new GravityRune();
            gravityRune.position = position;
            gravityRune.Initialize();
            Main.activeProjectiles.Add(gravityRune);
        }

        public override void Initialize()
        {
            active = false;
            hitbox = new Rectangle((int)position.X, (int)position.Y, Size.X, Size.Y);
        }

        public override void Update()
        {
            int amountOfSmoke = Main.random.Next(1, 3 + 1);
            for (int i = 0; i < amountOfSmoke; i++)
            {
                Vector2 smokePosition = position + (new Vector2(Main.random.Next(0, Size.X + 1), Main.random.Next(0, Size.Y + 1)) - ((Size.ToVector2() / 2f))) * (runeStrength / 2f);
                Vector2 smokeVelocity = new Vector2(Main.random.Next(-4, 4 + 1), Main.random.Next(-4, 4 + 1)) / 12f;
                Smoke.NewSmokeParticle(smokePosition, smokeVelocity, Color.Magenta, Color.Blue, 15, 20, 10, foreground: true);
            }

            if (soundPlayCooldown > 0)
                soundPlayCooldown--;

            if (active)
            {
                activeTimer++;
                if (activeTimer > RuneActiveTime)
                    DestroyInstance();

                if (Vector2.Distance(Main.currentPlayer.playerCenter, position) <= (runeStrength / 2f) * 16f)
                {
                    Main.currentPlayer.ThrowPlayer(throwVelocity);
                    if (soundPlayCooldown <= 0)
                    {
                        soundPlayCooldown = 2 * 60;
                        SoundPlayer.PlaySoundFromOtherSource(Sounds.GravityRune_Fling, position, 12, 0.8f);
                    }
                }

                CollisionBody[] enemies = Main.activeEnemies.ToArray();
                Projectile[] projectiles = Main.activeProjectiles.ToArray();
                foreach (Enemy enemy in enemies)
                {
                    if (Vector2.Distance(enemy.position, position) < (runeStrength / 2f) * 16f)
                    {
                        enemy.ThrowEnemy(throwVelocity);
                        if (soundPlayCooldown <= 0)
                        {
                            soundPlayCooldown = 2 * 60;
                            SoundPlayer.PlaySoundFromOtherSource(Sounds.GravityRune_Fling, position, 12, 0.8f);
                        }
                    }
                }
                foreach (Projectile projectile in projectiles)
                {
                    if (Vector2.Distance(projectile.position, position) < (runeStrength / 2f) * 16f)
                    {
                        projectile.ThrowProjectile(throwVelocity);
                        if (soundPlayCooldown <= 0)
                        {
                            soundPlayCooldown = 2 * 60;
                            SoundPlayer.PlaySoundFromOtherSource(Sounds.GravityRune_Fling, position, 12, 0.8f);
                        }
                    }
                }
                return;
            }

            if (!InputManager.IsMouseLeftHeld())
            {
                active = true;
                throwVelocity = new Vector2((float)Math.Cos(arrowRotation), (float)Math.Sin(arrowRotation));
                throwVelocity.Normalize();
                throwVelocity *= runeStrength * 1.5f;
                return;
            }

            arrowRotation = (position - GameData.MouseWorldPosition).GetRotation();
            runeStrength = Math.Clamp(Vector2.Distance(GameData.MouseWorldPosition, position) / 16f, 0f, 8f);
            baseRuneRotation += MathHelper.ToRadians(0.5f);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            float scale = runeStrength / 2f;
            spriteBatch.Draw(ElectricRune.baseRuneTexture, position, null, RuneColor, baseRuneRotation, new Vector2(12), scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(ElectricRune.baseRuneInnerCircleTexture, position, null, RuneColor, -baseRuneRotation * 0.5f, new Vector2(12), scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(runeSymbolTexture, position, null, RuneColor, arrowRotation + (float)(Math.PI / 2f), new Vector2(12), scale, SpriteEffects.None, 0f);
        }
    }
}
