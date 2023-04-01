using Caster.World.WorldObjects;
using Caster.World.WorldObjects.Destructibles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Caster.World
{
    public class WorldData
    {
        public static Dictionary<byte, WorldData> LoadedWorldDataInstances = new Dictionary<byte, WorldData>();

        public const byte CreationTag_LobbyWorld = 0;
        public const byte CreationTag_MainWorld = 1;
        public const byte CreationTag_BossWorld = 2;

        public Tile[,] tiles;
        public Dictionary<int, WorldObject> staticWorldObjects;
        public Dictionary<int, DestroyableWorldObject> destroyableWorldObjects;
        public Dictionary<int, WorldObject> priorityWorldObjects;
        public Vector2 spawnPosition;
        public Vector2 dimensions;      //In tiles
        public bool canEnemiesSpawn;

        /// <summary>
        /// Initializes a new instance of WorldData. Contains tile info and World object info.
        /// </summary>
        /// <param name="customWidth">A custom width for this World. If left as 0, WorldWidth will be used instead.</param>
        /// <param name="customHeight">A custom height for this World. If left as 0, WorldHeight will be used instead.</param>
        /// <param name="simple">Whether or not the World is "simple." </param>
        public WorldData(byte creationTag, int customWidth, int customHeight, bool enemySpawning = true, bool simple = false)
        {
            tiles = new Tile[customWidth, customHeight];
            staticWorldObjects = new Dictionary<int, WorldObject>();
            destroyableWorldObjects = new Dictionary<int, DestroyableWorldObject>();
            priorityWorldObjects = new Dictionary<int, WorldObject>();
            if (LoadedWorldDataInstances.ContainsKey(creationTag))
                LoadedWorldDataInstances[creationTag] = this;
            else
                LoadedWorldDataInstances.Add(creationTag, this);
            canEnemiesSpawn = enemySpawning;
            dimensions = new Vector2(customWidth, customHeight);
        }

        public static void ClearWorldData(WorldData data)
        {
            Array.Clear(data.tiles, 0, data.tiles.Length);
            data.staticWorldObjects.Clear();
            data.destroyableWorldObjects.Clear();
            data.priorityWorldObjects.Clear();
            data.dimensions = Vector2.Zero;
        }
    }
}
