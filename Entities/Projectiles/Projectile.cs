using AnotherLib.Collision;
using Caster.World;
using Microsoft.Xna.Framework;
using System;

namespace Caster.Entities.Projectiles
{
    public class Projectile : CollisionBody
    {
        public Vector2 throwVelocity;
        public virtual bool CanBeThrownAround { get; } = false;

        /// <summary>
        /// Detects the tile's collision style in the given coordinates.
        /// </summary>
        /// <param name="position">The position of the point to check.</param>
        /// <returns>Whether or not a collision happeend.</returns>
        public bool DetectTileCollisionsByCollisionStyle(Vector2 position)
        {
            Point positionPoint = (position / 16).ToPoint();
            if (!ChunkLoader.CheckForSafeTileCoordinates(positionPoint))
                return false;

            if (WorldClass.activeWorldData.tiles[positionPoint.X, positionPoint.Y].isCollideable)
                return true;

            return false;
        }

        public void ThrowProjectile(Vector2 throwVelocity)
        {
            this.throwVelocity = throwVelocity;
        }

        public void UpdateThrowPhysics()
        {
            if (throwVelocity != Vector2.Zero)
            {
                if (Math.Abs(throwVelocity.X) < 0.01f && Math.Abs(throwVelocity.Y) < 0.01f)
                    throwVelocity = Vector2.Zero;
                else
                    throwVelocity *= 0.97f;
            }
        }

        public void DestroyInstance()
        {
            Main.activeProjectiles.Remove(this);
        }
    }
}
