using AnotherLib;
using AnotherLib.Input;
using AnotherLib.Utilities;
using Caster.UI;
using Caster.Utilities;
using Caster.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using static Caster.Main;

namespace Caster.Entities.Players
{
    public class Player : PlatformerBody
    {
        private const int PlayerWidth = 18;
        private const int PlayerHeight = 32;
        private const float MoveSpeed = 1.5f;
        private const float GravityStrength = 0.18f;
        private const float MaxFallSpeed = 18f;
        private const float MaxFloatSpeed = 0.034f;
        private const float JumpStrength = 3.6f;
        private const int TeleportCooldownTime = 5 * 60;
        private readonly Vector2 ArmPlacementOffset = new Vector2(5, 13);
        private readonly Vector2 ArmOrigin = new Vector2(2, 3);
        public static Texture2D playerWalkSpritesheet;
        public static Texture2D playerJumpFrame;
        public static Texture2D playerFallSpritesheet;
        public static Texture2D playerArmTexture;
        private Texture2D currentTexture;

        public Vector2 playerCenter;
        public Vector2 oldPosition;
        public int direction = 1;
        private float currentYVelocity;
        private int immunityTimer = 0;
        private int teleportCooldownTimer = 0;
        private List<AfterImageData> afterImages;
        private float armRotation;

        private int frame = 0;
        private int frameCounter = 0;
        private Rectangle animRect;
        private PlayerState playerState;
        private PlayerState oldPlayerState;
        private bool loadedWorld = false;
        private int teleportAfterImageSpawnTimer = 0;

        public override CollisionType collisionType => CollisionType.Player;
        public override CollisionType[] colliderTypes => new CollisionType[2] { CollisionType.Enemies, CollisionType.EnemyProjectiles };

        private enum PlayerState
        {
            Idle,
            Walking,
            Shooting,
            Jumping,
            Falling
        }

        private struct AfterImageData
        {
            public Vector2 position;
            public Rectangle animRect;
            public Color drawColor;
            public SpriteEffects spriteEffects;
            public int lifeTime;
            public int lifeTimer;
            public float alpha;
        }

        public override void Initialize()
        {
            currentTexture = playerWalkSpritesheet;
            hitbox = new Rectangle(0, 0, PlayerWidth, PlayerHeight);
            animRect = new Rectangle(0, 0, PlayerWidth, PlayerHeight);
            playerState = PlayerState.Idle;
            afterImages = new List<AfterImageData>();
            uiList.Add(PlayerUI.NewPlayerUI());
        }

        public override void Update()
        {
            if (immunityTimer > 0)
                immunityTimer--;
            if (teleportCooldownTimer > 0)
                teleportCooldownTimer--;
            if (teleportAfterImageSpawnTimer > 0)
                teleportAfterImageSpawnTimer--;

            if (GameData.MouseWorldPosition.X > playerCenter.X)
                direction = 1;
            else
                direction = -1;
            armRotation = (GameData.MouseWorldPosition - playerCenter).GetRotation();

            Vector2 velocity = Move(MoveSpeed);
            DetectTileCollisions();
            if (tileCollisionDirection[CollisionDirection_Bottom] && tileCollisionDirection[CollisionDirection_Left] && tileCollisionDirection[CollisionDirection_Right])
                position.Y -= 1f;

            if (currentYVelocity == 0f)
            {
                if (velocity.X != 0f)
                    playerState = PlayerState.Walking;
                else
                    playerState = PlayerState.Idle;
            }
            else
            {
                if (currentYVelocity > 0f)
                    playerState = PlayerState.Falling;
                else
                    playerState = PlayerState.Jumping;
            }

            if (GameInput.IsAttackJustPressed())
            {
                //Shoot sound
                //SoundPlayer.PlaySoundFromOtherSource(Sounds.PlayerShoot, playerCenter, 12, 0.6f, random.Next(-4, 4 + 1) / 10f);
                //PlayerBullet.NewBullet(playerCenter + new Vector2(6f * direction, -4.5f), new Vector2(16f * direction, 0f));
            }
            if ((InputManager.IsMouseRightJustPressed() || InputManager.IsButtonJustPressed(Buttons.B)) && teleportCooldownTimer <= 0)
            {
                immunityTimer += 60;
                teleportCooldownTimer += TeleportCooldownTime;
                teleportAfterImageSpawnTimer = 45;
                /*Vector2 cameraThrowVector = GameData.MouseWorldPosition - playerCenter;
                float oldLength = cameraThrowVector.Length();
                cameraThrowVector.Normalize();
                cameraThrowVector *= oldLength * 0.2f;
                camera.ThrowCamera(cameraThrowVector, 10);*/
                position = GameData.MouseWorldPosition + new Vector2(PlayerWidth / 2f, PlayerHeight / 2f);
                //SoundPlayer.PlaySoundFromOtherSource(Sounds.PlayerDash, playerCenter, 12);
            }

            AnimatePlayer();
            ManageAfterImages();
            ChunkLoader.UpdateActiveWorldChunk(playerCenter);
            camera.UpdateCamera(playerCenter);
            oldPosition = position;
            if (!loadedWorld)
            {
                loadedWorld = true;
                ChunkLoader.ForceUpdateActiveWorldChunk(playerCenter);
            }
        }

        public Vector2 Move(float moveSpeed)
        {
            Vector2 velocity = Vector2.Zero;

            DetectWorldObjectCollisions();
            if (!GameInput.ControllerConnected)
            {
                if (GameInput.IsLeftPressed() && !tileCollisionDirection[CollisionDirection_Left])
                {
                    direction = -1;
                    velocity.X -= moveSpeed;
                }
                if (GameInput.IsRightPressed() && !tileCollisionDirection[CollisionDirection_Right])
                {
                    direction = 1;
                    velocity.X += moveSpeed;
                }
                if ((GameInput.IsUpPressed() || InputManager.IsKeyJustPressed(Keys.Space)) && tileCollisionDirection[CollisionDirection_Bottom])
                {
                    currentYVelocity = -JumpStrength;
                    //Jump sound
                }
            }
            else
            {
                Vector2 leftAnalog = GameInput.GetLeftAnalogVector();
                velocity = leftAnalog * moveSpeed;
                if (velocity.Y < 0f && tileCollisionDirection[CollisionDirection_Top])
                    velocity.Y = 0f;
                if (velocity.X < 0f && tileCollisionDirection[CollisionDirection_Left])
                    velocity.X = 0f;
                if (velocity.Y > 0f && tileCollisionDirection[CollisionDirection_Bottom])
                    velocity.Y = 0f;
                if (velocity.X > 0f && tileCollisionDirection[CollisionDirection_Right])
                    velocity.X = 0f;

                if (velocity.X > 0.05f)
                    direction = 1;
                else if (velocity.X < -0.05f)
                    direction = -1;
            }

            if (!tileCollisionDirection[CollisionDirection_Bottom])
            {
                if (currentYVelocity < MaxFallSpeed)
                    currentYVelocity += GravityStrength;
                if ((GameInput.IsUpPressed() || InputManager.IsKeyPressed(Keys.Space)) && currentYVelocity > 0)
                {
                    currentYVelocity = MathHelper.Lerp(currentYVelocity, MaxFloatSpeed, 0.12f);
                }
            }
            else
            {
                teleportAfterImageSpawnTimer = 0;
                if (currentYVelocity > 0)
                    currentYVelocity = 0f;
            }

            velocity.Y = currentYVelocity;
            position += velocity;
            playerCenter = position + new Vector2(PlayerWidth / 2f, PlayerHeight / 2f);
            hitbox.X = (int)(position.X + hitboxOffset.X);
            hitbox.Y = (int)(position.Y + hitboxOffset.Y);
            GameData.AudioPosition = playerCenter;
            return velocity;
        }

        public Vector2 Move(Vector2 velocity, bool detectCollisions = false)
        {
            if (detectCollisions)
            {
                DetectTileCollisions();
                if (velocity.Y < 0 && tileCollisionDirection[CollisionDirection_Top])
                    velocity.Y = 0f;
                else if (velocity.Y > 0 && tileCollisionDirection[CollisionDirection_Bottom])
                    velocity.Y = 0f;

                if (velocity.X < 0 && tileCollisionDirection[CollisionDirection_Left])
                    velocity.X = 0f;
                else if (velocity.X > 0 && tileCollisionDirection[CollisionDirection_Right])
                    velocity.X = 0f;
            }

            position += velocity;
            playerCenter = position + new Vector2(PlayerWidth / 2f, PlayerHeight / 2f);
            hitbox.X = (int)(position.X + hitboxOffset.X);
            hitbox.Y = (int)(position.Y + hitboxOffset.Y);
            GameData.AudioPosition = playerCenter;
            return velocity;
        }

        private void AnimatePlayer()
        {
            if (oldPlayerState != playerState)
            {
                frame = 0;
                frameCounter = 0;
                animRect.Y = 0;
                oldPlayerState = playerState;
            }

            frameCounter++;
            if (playerState == PlayerState.Idle)
            {
                if (currentTexture != playerWalkSpritesheet)
                    currentTexture = playerWalkSpritesheet;

                frame = 0;
                animRect.Y = frame * PlayerHeight;
            }
            else if (playerState == PlayerState.Walking)
            {
                if (currentTexture != playerWalkSpritesheet)
                    currentTexture = playerWalkSpritesheet;

                if (frameCounter >= 7)
                {
                    frame += 1;
                    frameCounter = 0;
                    if (frame >= 4)
                        frame = 0;

                    animRect.Y = frame * PlayerHeight;
                    //if (frame == 1 || frame == 3)
                        //SoundPlayer.PlaySoundFromOtherSource(Main.random.Next(Sounds.Step_1, Sounds.Step_3 + 1), playerCenter, 12, soundPitch: Main.random.Next(-4, 4 + 1) / 10f);
                }

            }
            else if (playerState == PlayerState.Jumping)
            {
                if (currentTexture != playerJumpFrame)
                    currentTexture = playerJumpFrame;
            }
            else if (playerState == PlayerState.Falling)
            {
                if (currentTexture != playerFallSpritesheet)
                    currentTexture = playerFallSpritesheet;

                if (frameCounter >= 8)
                {
                    frame += 1;
                    frameCounter = 0;
                    if (frame >= 3)
                        frame = 0;

                    animRect.Y = frame * PlayerHeight;
                }
            }
        }

        private void ManageAfterImages()
        {
            if (teleportAfterImageSpawnTimer > 0 && teleportAfterImageSpawnTimer % 4 == 0)
            {
                SpriteEffects spriteEffects = SpriteEffects.None;
                if (direction == -1)
                    spriteEffects = SpriteEffects.FlipHorizontally;

                AfterImageData afterImage = new AfterImageData()
                {
                    position = oldPosition,
                    animRect = animRect,
                    drawColor = Color.White * 0.5f,
                    alpha = 1f,
                    lifeTime = 15,
                    lifeTimer = 15,
                    spriteEffects = spriteEffects
                };
                afterImages.Add(afterImage);
            }

            for (int i = 0; i < afterImages.Count; i++)
            {
                AfterImageData afterImage = afterImages[i];
                afterImage.lifeTimer -= 1;
                afterImage.alpha = (float)afterImage.lifeTimer / (float)afterImage.lifeTime;
                if (afterImage.lifeTimer > 0)
                {
                    afterImages[i] = afterImage;
                }
                else
                {
                    afterImages.RemoveAt(i);
                    i--;
                }
            }
        }

        public void Hurt()
        {
            if (immunityTimer > 0)
                return;

            immunityTimer += 60;
            SoundPlayer.PlayLocalSound(Sounds.AttemptHit);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < afterImages.Count; i++)
            {
                spriteBatch.Draw(currentTexture, afterImages[i].position, afterImages[i].animRect, afterImages[i].drawColor * afterImages[i].alpha, 0f, Vector2.Zero, 1f, afterImages[i].spriteEffects, 0f);
            }
            SpriteEffects spriteEffects = SpriteEffects.None;
            SpriteEffects armSpriteEffects = SpriteEffects.None;
            Vector2 armDrawPosition = position + ArmPlacementOffset;
            if (direction == -1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
                armSpriteEffects = SpriteEffects.FlipVertically;
                armDrawPosition.X += 7f;
            }

            spriteBatch.Draw(currentTexture, position, animRect, Color.White, 0f, Vector2.Zero, 1f, spriteEffects, 0f);
            spriteBatch.Draw(playerArmTexture, armDrawPosition, null, Color.White, armRotation, ArmOrigin, 1f, armSpriteEffects, 0f);
        }
    }
}
