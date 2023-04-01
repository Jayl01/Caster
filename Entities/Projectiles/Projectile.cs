using AnotherLib.Collision;
using Caster.World;
using Microsoft.Xna.Framework;

namespace Caster.Entities.Projectiles
{
    public class Projectile : CollisionBody
    {
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

        public void DestroyInstance()
        {
            Main.activeProjectiles.Remove(this);
        }
    }
}
