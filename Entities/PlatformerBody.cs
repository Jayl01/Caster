using AnotherLib.Collision;
using Caster.World;
using Caster.World.WorldObjects;
using Microsoft.Xna.Framework;

namespace Caster.Entities
{
    public class PlatformerBody : CollisionBody
    {
        public bool standingOnGround = false;

        /// <summary>
        /// Detects collisions between the object this method is being called on and the active World chunk.
        /// </summary>
        /// <returns>Whether or not a collision happened. Direction is returnd in the CollisionDirection array.</returns>
        public bool DetectTileCollisions()
        {
            bool colliding = false;
            for (int i = 0; i < tileCollisionDirection.Length; i++)
                tileCollisionDirection[i] = false;

            for (int x = 0; x < ChunkLoader.ChunkSizeWidth; x++)
            {
                for (int y = 0; y < ChunkLoader.ChunkSizeHeight; y++)
                {
                    /*if (WorldClass.activeWorldData.tiles.GetLength(0) - 1 < x || WorldClass.activeWorldData.tiles.GetLength(1) - 1 < y || WorldClass.activeWorldChunk.GetLength(0) - 1 < x || WorldClass.activeWorldChunk.GetLength(1) - 1 < y)
                        continue;*/

                    if (x < 0 || x >= WorldClass.CurrentWorldWidth || y < 0 || y >= WorldClass.CurrentWorldHeight)
                        continue;

                    Tile possibleColliderTile = WorldClass.activeWorldChunk[x, y];
                    if (possibleColliderTile == null)
                        continue;

                    if (!possibleColliderTile.isCollideable)
                        continue;

                    if (hitbox.Intersects(possibleColliderTile.tileCollisionRect))
                    {
                        colliding = true;
                        Rectangle collidingRect = possibleColliderTile.tileCollisionRect;
                        int hitboxReductionFactor = 2;
                        bool withinXBoundaries = hitbox.Left + hitboxReductionFactor < collidingRect.Right && hitbox.Right - hitboxReductionFactor > collidingRect.X;
                        bool withinYBoundaries = hitbox.Top + hitboxReductionFactor < collidingRect.Bottom && hitbox.Bottom - hitboxReductionFactor > collidingRect.Top;

                        if (withinXBoundaries)
                        {
                            if (hitbox.Top < collidingRect.Bottom && hitbox.Center.Y > collidingRect.Center.Y)
                                tileCollisionDirection[CollisionDirection_Top] = true;

                            if (hitbox.Bottom > collidingRect.Top && hitbox.Center.Y < collidingRect.Center.Y)
                                tileCollisionDirection[CollisionDirection_Bottom] = true;
                        }

                        if (withinYBoundaries)
                        {
                            if (hitbox.Left < collidingRect.Right && hitbox.Center.X < collidingRect.Center.X)
                                tileCollisionDirection[CollisionDirection_Right] = true;

                            if (hitbox.Right > collidingRect.Left && hitbox.Center.X > collidingRect.Center.X)
                                tileCollisionDirection[CollisionDirection_Left] = true;
                        }
                        HandleAnyCollision();
                        HandleTileCollision();
                        //DebugTools.AddDebugText("Left: " + tileCollisionDirection[CollisionDirection_Left] + "; Right: " + tileCollisionDirection[CollisionDirection_Right];
                    }
                }
            }
            return colliding;
        }

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

        /// <summary>
        /// Detects collisions with World Objects in the active chunk.
        /// </summary>
        /// <returns>Whether or not a collision happened. Direction is returned in the CollisionDirection array.</returns>
        public bool DetectWorldObjectCollisions()
        {
            bool colliding = false;
            WorldObject[] possibleIntersectorsCopy = World.WorldClass.activeWorldObjects.ToArray();
            for (int i = 0; i < possibleIntersectorsCopy.Length; i++)
            {
                WorldObject possibleColliderObject = possibleIntersectorsCopy[i];
                if (possibleColliderObject.noCollision)
                    continue;

                if (hitbox.Intersects(possibleColliderObject.collisionRect))
                {
                    colliding = true;
                    Rectangle collidingRect = possibleColliderObject.collisionRect;
                    int hitboxReductionFactor = 2;
                    bool withinXBoundaries = hitbox.Left + hitboxReductionFactor < collidingRect.Right && hitbox.Right - hitboxReductionFactor > collidingRect.X;
                    bool withinYBoundaries = hitbox.Top + hitboxReductionFactor < collidingRect.Bottom && hitbox.Bottom - hitboxReductionFactor > collidingRect.Top;

                    if (withinXBoundaries)
                    {
                        if (hitbox.Top < collidingRect.Bottom && hitbox.Center.Y > collidingRect.Center.Y)
                            tileCollisionDirection[CollisionDirection_Top] = true;

                        if (hitbox.Bottom > collidingRect.Top && hitbox.Center.Y < collidingRect.Center.Y)
                            tileCollisionDirection[CollisionDirection_Bottom] = true;
                    }

                    if (withinYBoundaries)
                    {
                        if (hitbox.Left < collidingRect.Right && hitbox.Center.X < collidingRect.Center.X)
                            tileCollisionDirection[CollisionDirection_Right] = true;

                        if (hitbox.Right > collidingRect.Left && hitbox.Center.X > collidingRect.Center.X)
                            tileCollisionDirection[CollisionDirection_Left] = true;
                    }
                    HandleAnyCollision();
                    HandleTileCollision();
                    //HandleWorldObjectCollision(possibleColliderObject);
                }
            }
            return colliding;
        }

        /// <summary>
        /// Detects collisions with World Objects in the active chunk and calls HandleWorldCollisions() if a collision was detected.
        /// </summary>
        public void DetectHandledWorldObjectCollisions()
        {
            WorldObject[] possibleIntersectorsCopy = WorldClass.activeWorldObjects.ToArray();
            foreach (WorldObject intersector in possibleIntersectorsCopy)
            {
                if (intersector.noCollision)
                    continue;

                if (hitbox.Intersects(intersector.collisionRect))
                {
                    HandleAnyCollision();
                    HandleWorldObjectCollision(intersector);
                    break;
                }
            }
        }

        /// <summary>
        /// A method that gets called whenever a specific World object collision happens.
        /// </summary>
        /// <param name="collider">The collider.</param>
        public virtual void HandleWorldObjectCollision(WorldObject collider)
        { }
    }
}
