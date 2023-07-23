using AnotherLib.Utilities;
using Caster.Effects;
using Caster.Entities.Projectiles;
using Caster.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Caster.Entities.Enemies
{
    public class EvilCaster : Enemy
    {
        public static Texture2D evilCasterWalkSpritesheet;
        public static Texture2D evilCasterAttackSpritesheet;
        private Texture2D currentTexture;

        private const float DetectionDistance = 16f * 16f;
        private const float ShootRange = 12f * 16f;
        private const float MoveSpeed = 0.9f;
        private const float GravityStrength = 0.18f;
        private const float MaxFallSpeed = 18f;
        private const float JumpStrength = 3.6f;

        private Vector2 center;
        private bool targetFound;
        private bool targetInRange = false;
        private int direction = 1;
        private int frame = 0;
        private int frameCounter = 0;
        private AnimState animState;
        private AnimState oldAnimState;
        private Rectangle animRect;
        private int shootTimer = 0;
        private float currentYVelocity = 0f;
        private Vector2 deathVelocity;
        private Vector2 idleWalkVelocity;
        private int sinTimer = 0;
        private float shootDistanceOffset;
        private int attackCooldown = 0;
        private readonly Vector2 LeftArmOffset = new Vector2(2, 2);
        private readonly Vector2 RightArmOffset = new Vector2(14, 3);

        public override int EnemyWidth => 19;
        public override int EnemyHeight => 27;
        public override int EnemyHealth => 100;
        public override CollisionType collisionType => CollisionType.Enemies;
        public override CollisionType[] colliderTypes => new CollisionType[2] { CollisionType.Player, CollisionType.FriendlyProjectiles };

        private enum AnimState
        {
            Walk,
            Attacking
        }

        public static void NewEvilCaster(Vector2 position)
        {
            EvilCaster newEvilCaster = new EvilCaster();
            newEvilCaster.position = position;
            newEvilCaster.Initialize();
            Main.activeEnemies.Add(newEvilCaster);
        }

        public override void Initialize()
        {
            health = EnemyHealth;
            currentTexture = evilCasterWalkSpritesheet;
            hitbox = new Rectangle((int)position.X, (int)position.Y, EnemyWidth, EnemyHeight);
            animRect = new Rectangle(0, 0, EnemyWidth, EnemyHeight);
            idleWalkVelocity = new Vector2(-2f, 0f);
            shootDistanceOffset = Main.random.Next(-2 * 16, (2 * 16) + 1);
            drawColor = Color.White;
            if (Main.currentPlayer.playerCenter.X > position.X)
                idleWalkVelocity = new Vector2(2f, 0f);
        }

        public override void Update()
        {
            sinTimer++;
            if (sinTimer >= 360)
                sinTimer = 0;

            if (hurtTimer > 0)
            {
                hurtTimer--;
                if (hurtTimer <= 0)
                    drawColor = Color.White;
            }

            if (attackCooldown > 0)
                attackCooldown--;

            Vector2 velocity = throwVelocity;
            if (!targetFound)
            {
                animState = AnimState.Walk;
                if (DetectTileCollisionsByCollisionStyle(center + (velocity * 18f)))
                    currentYVelocity = -JumpStrength;
                if (Vector2.Distance(Main.currentPlayer.playerCenter, center) <= DetectionDistance)
                {
                    targetFound = true;
                    if (Main.currentPlayer.playerCenter.X > position.X)
                        direction = 1;
                    else
                        direction = -1;
                }
                else
                    velocity.X += idleWalkVelocity.X;
            }
            else
            {
                targetInRange = Vector2.Distance(Main.currentPlayer.playerCenter, center) <= ShootRange + shootDistanceOffset;
                if (Main.currentPlayer.playerCenter.X > position.X)
                    direction = 1;
                else
                    direction = -1;

                if (!targetInRange)
                {
                    shootTimer = 0;
                    velocity.X += direction * MoveSpeed;
                    if (DetectTileCollisionsByCollisionStyle(center + (velocity * 18f)))
                        currentYVelocity = -JumpStrength;
                    animState = AnimState.Walk;
                }
                else
                {
                    if (attackCooldown <= 0)
                    {
                        shootTimer++;
                        animState = AnimState.Attacking;
                        if (shootTimer >= 5 * 60)
                        {
                            shootTimer = 0;
                            Vector2 spawnPosition = Main.currentPlayer.playerCenter + new Vector2(Main.random.Next(-8 * 16, (8 * 16) + 1), -Main.random.Next(3 * 16, (5 * 16) + 1));
                            int travelDirection = 1;
                            if (Main.currentPlayer.playerCenter.X < spawnPosition.X)
                                travelDirection = -1;

                            LaserRune.NewLaserRune(spawnPosition, travelDirection);
                            attackCooldown = 2 * 60;
                            //SoundPlayer.PlaySoundFromOtherSource(Sounds.PlayerShoot, center, 12, soundPitch: Main.random.Next(4, 6 + 1) / 10f);
                        }
                        int amountOfSmoke = Main.random.Next(6, 9 + 1);
                        for (int i = 0; i < amountOfSmoke; i++)
                        {
                            bool rightArm = Main.random.Next(0, 1 + 1) == 0;
                            Vector2 offset = RightArmOffset;
                            if (rightArm)
                                offset = LeftArmOffset;

                            Vector2 armPosition = position + offset;
                            if (direction == -1)
                                armPosition = position + new Vector2(19, 0) + new Vector2(-offset.X, offset.Y);
                            Vector2 smokePosition = armPosition + new Vector2(Main.random.Next(-30, 30 + 1) / 10f, Main.random.Next(-30, 30 + 1) / 10f);
                            Vector2 smokeVelocity = new Vector2(Main.random.Next(-2, 2 + 1), Main.random.Next(-2, 2 + 1)) / 12f;
                            Smoke.NewSmokeParticle(smokePosition, smokeVelocity, Color.Red, Color.DarkRed, 15, 20, 10, foreground: rightArm);
                        }
                    }
                    else
                        animState = AnimState.Walk;
                }
            }

            if (!tileCollisionDirection[CollisionDirection_Bottom])
            {
                if (currentYVelocity < MaxFallSpeed)
                    currentYVelocity += GravityStrength;
            }
            else
            {
                if (currentYVelocity > 0)
                    currentYVelocity = 0f;
            }

            velocity.Y = currentYVelocity + throwVelocity.Y;
            velocity = DetectTileCollisionsWithVelocity(velocity);
            position += velocity;
            hitbox.X = (int)(position.X + hitboxOffset.X);
            hitbox.Y = (int)(position.Y + hitboxOffset.Y);
            center = hitbox.Center.ToVector2();
            if (throwVelocity != Vector2.Zero)
            {
                if (Math.Abs(throwVelocity.X) < 0.01f && Math.Abs(throwVelocity.Y) < 0.01f)
                    throwVelocity = Vector2.Zero;
                else
                    throwVelocity *= 0.97f;
            }
            AnimateEvilCaster();
        }

        private void AnimateEvilCaster()
        {
            if (animState != oldAnimState)
            {
                frame = 0;
                frameCounter = 0;
                animRect.Y = 0;
                oldAnimState = animState;
            }

            if (animState == AnimState.Walk)
            {
                if (currentTexture != evilCasterWalkSpritesheet)
                    currentTexture = evilCasterWalkSpritesheet;

                frameCounter++;
                if (frameCounter >= 7)
                {
                    frame += 1;
                    frameCounter = 0;
                    if (frame >= 4)
                        frame = 0;

                    animRect.Y = frame * EnemyHeight;
                    //if (frame == 1 || frame == 3)
                    //SoundPlayer.PlaySoundFromOtherSource(Main.random.Next(Sounds.Step_1, Sounds.Step_3 + 1), center, 12, soundPitch: Main.random.Next(-4, 4 + 1) / 10f);
                }
            }
            else if (animState == AnimState.Attacking)
            {
                if (currentTexture != evilCasterAttackSpritesheet)
                    currentTexture = evilCasterAttackSpritesheet;

                frameCounter++;
                if (frameCounter >= 6)
                {
                    frame += 1;
                    frameCounter = 0;
                    if (frame >= 3)
                        frame = 0;

                    animRect.Y = frame * EnemyHeight;
                }
            }
        }

        public override void HurtEffects()
        {
            deathVelocity = new Vector2(4f * -direction, currentYVelocity);
            deathVelocity.X *= 0.1f;
            shootTimer = 0;
            attackCooldown = 30;
            SoundPlayer.PlaySoundFromOtherSource(Sounds.Caster_Hurt, center, 8, soundPitch: Main.random.Next(-2, 2 + 1) / 10f);
        }

        public override void DeathEffects()
        {
            Gore.NewGore(Gore.Caster_1, center, deathVelocity);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (direction == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Vector2 drawPosition = position;
            drawPosition.Y -= 4f;
            drawPosition.Y -= 2f * (float)Math.Sin(MathHelper.ToRadians(sinTimer * 4));
            spriteBatch.Draw(currentTexture, drawPosition, animRect, drawColor, 0f, Vector2.Zero, 1f, spriteEffects, 0f);
        }
    }
}
