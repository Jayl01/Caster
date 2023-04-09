using Caster.World;
using Microsoft.Xna.Framework;
using System;

namespace Caster.Entities.Enemies
{
    public class Enemy : PlatformerBody
    {
        public virtual int EnemyWidth { get; }
        public virtual int EnemyHeight { get; }
        public virtual int EnemyHealth { get; }

        public int health;
        public Vector2 throwVelocity;
        public Color drawColor;
        public int hurtTimer = 0;

        public static int EnemiesKilled = 0;

        public void ThrowEnemy(Vector2 throwVelocity)
        {
            this.throwVelocity = throwVelocity;
        }

        /// <summary>
        /// Detects collisions against tiles with the velocity in consideration.
        /// </summary>
        public Vector2 DetectTileCollisionsWithVelocity(Vector2 velocity, float repulsionForce = 0.05f)
        {
            for (int i = 0; i < tileCollisionDirection.Length; i++)
                tileCollisionDirection[i] = false;

            Vector2 modifiedVelocity = velocity;
            Rectangle modifiedHitbox = hitbox;
            modifiedHitbox.X += (int)velocity.X;
            modifiedHitbox.Y += (int)velocity.Y;

            int widthMult = (hitbox.Width / 16) + 1;
            int heightMult = (hitbox.Height / 16) + 1;
            for (int x = -1 * widthMult; x <= 1 * widthMult; x++)
            {
                if (modifiedVelocity == Vector2.Zero)
                    break;

                for (int y = -1 * heightMult; y <= 1 * heightMult; y++)
                {
                    int pointX = hitbox.X / 16;
                    int pointY = hitbox.Y / 16;
                    Point tilePos = new Point(pointX + x, pointY + y);
                    tilePos.X = Math.Clamp(tilePos.X, 0, WorldClass.CurrentWorldWidth - 1);
                    tilePos.Y = Math.Clamp(tilePos.Y, 0, WorldClass.CurrentWorldHeight - 1);

                    if (!WorldClass.activeWorldData.tiles[tilePos.X, tilePos.Y].isCollideable)
                        continue;

                    Rectangle collidingRect = WorldClass.activeWorldData.tiles[tilePos.X, tilePos.Y].tileCollisionRect;
                    if (modifiedHitbox.Intersects(collidingRect))
                    {
                        int hitboxReductionFactor = 2;
                        bool withinXBoundaries = modifiedHitbox.Left + hitboxReductionFactor < collidingRect.Right && modifiedHitbox.Right - hitboxReductionFactor > collidingRect.X;
                        bool withinYBoundaries = modifiedHitbox.Top + hitboxReductionFactor < collidingRect.Bottom && modifiedHitbox.Bottom - hitboxReductionFactor > collidingRect.Top;

                        if (withinXBoundaries)
                        {
                            if (modifiedHitbox.Top < collidingRect.Bottom && modifiedHitbox.Center.Y - hitboxReductionFactor > collidingRect.Center.Y)      //Top
                            {
                                modifiedVelocity.Y = repulsionForce;
                                tileCollisionDirection[CollisionDirection_Top] = true;
                            }
                            else if (modifiedHitbox.Bottom > collidingRect.Top && modifiedHitbox.Center.Y + hitboxReductionFactor < collidingRect.Center.Y)     //Bottom
                            {
                                modifiedVelocity.Y = -repulsionForce;
                                tileCollisionDirection[CollisionDirection_Bottom] = true;
                            }
                        }

                        if (withinYBoundaries)
                        {
                            if (modifiedHitbox.Left < collidingRect.Right && modifiedHitbox.Center.X + hitboxReductionFactor < collidingRect.Center.X)      //Left
                            {
                                modifiedVelocity.X = -repulsionForce;
                                tileCollisionDirection[CollisionDirection_Left] = true;
                            }
                            else if (modifiedHitbox.Right > collidingRect.Left && modifiedHitbox.Center.X - hitboxReductionFactor > collidingRect.Center.X)     //Right
                            {
                                modifiedVelocity.X = repulsionForce;
                                tileCollisionDirection[CollisionDirection_Right] = true;
                            }
                        }
                        HandleAnyCollision();
                        HandleTileCollision();
                    }
                }
            }
            return modifiedVelocity;
        }

        public virtual void HurtEffects()
        { }

        public virtual void DeathEffects()
        { }

        public void HurtEnemy(int damage)
        {
            if (hurtTimer > 0)
                return;

            health -= damage;
            hurtTimer = 5;
            drawColor = Color.Red;
            HurtEffects();
            if (health <= 0)
            {
                DeathEffects();
                DestroyInstance();
                EnemiesKilled += 1;
            }
        }

        public void DestroyInstance()
        {
            Main.activeEnemies.Remove(this);
        }
    }
}
