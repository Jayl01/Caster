using Caster.Entities.Players;
using Microsoft.Xna.Framework;
using System.Linq;

namespace Caster.World.WorldObjects.Destructibles
{
    public abstract class DestroyableWorldObject : WorldObject
    {
        public virtual int ObjectGoreType { get; } = 0;
        public virtual int ObjectStartingHealth { get; } = 0;

        public int objectHealth = 0;

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
