using Caster.Entities.Players;
using Microsoft.Xna.Framework;
using System.Linq;

namespace Caster.World.WorldObjects.Destructibles
{
    public abstract class DestroyableWorldObject : WorldObject
    {
        public virtual int ObjectGoreType { get; } = 0;
        public virtual int ObjectStartingHealth { get; } = 0;

        private const float GravityStrength = 0.18f;
        private const float MaxFallSpeed = 18f;

        public int objectHealth = 0;
        public Vector2 velocity;

        public virtual void Throw(Vector2 velocity)
        {
            this.velocity = velocity;
        }

        public void UpdatePhysics()
        {
            if (DetectTileCollisionsByCollisionStyle(position + velocity))
                velocity = Vector2.Zero;

            position += velocity;
            if (velocity.Y < MaxFallSpeed)
                velocity.Y += GravityStrength;
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

        public virtual void DamageObject(int damage)
        {
            objectHealth -= damage;
            if (objectHealth <= 0)
                DestroyObject();
        }

        public virtual void DamageObject(int damage, Player damager)
        {
            //to keep track of who destroyed something to make sure that only the current player unlocks an achievement
            objectHealth -= damage;
            if (objectHealth <= 0)
                DestroyObject();
        }

        public virtual void DamageObject(int damage, Vector2 hitPosition)
        {
            objectHealth -= damage;
            if (objectHealth <= 0)
                DestroyObject(hitPosition);
        }

        public void DestroyObject(bool forceUpdateChunk = true)
        {
            DestructionEffects(collisionRect.Center.ToVector2(), objectID);
            WorldClass.activeWorldData.destroyableWorldObjects.Remove(objectID);
            if (forceUpdateChunk)
                ChunkLoader.ForceUpdateActiveWorldChunk(Main.currentPlayer.position);
        }

        public void DestroyObject(Vector2 hitPosition, bool forceUpdateChunk = true)
        {
            DestructionEffects(hitPosition, objectID);
            WorldClass.activeWorldData.destroyableWorldObjects.Remove(objectID);
            if (forceUpdateChunk)
                ChunkLoader.ForceUpdateActiveWorldChunk(Main.currentPlayer.position);
        }

        /// <summary>
        /// A method that gets called when this object is destroyed.
        /// </summary>
        public virtual void DestructionEffects(Vector2 hitPosition, int seed)
        { }
    }
}
