using AnotherLib;
using Caster.World.WorldObjects;
using Microsoft.Xna.Framework;
using System;
using System.Linq;

namespace Caster.World
{
    public class ChunkLoader
    {
        public static int ChunkSizeWidth;
        public static int ChunkSizeHeight;
        private static Point previousChunkDimensions;
        public static Point previousChunkCoordinates;

        /// <summary>
        /// Checks if the given coordinates are out of the bounds of the map.
        /// </summary>
        /// <param name="point">The point to check for.</param>
        /// <returns>Whether or not the coordinates are outside of the bounds of the map.</returns>
        public static bool CheckForSafeTileCoordinates(int x, int y)
        {
            return x >= 0 && x < WorldClass.CurrentWorldWidth && y >= 0 && y < WorldClass.CurrentWorldHeight;
        }

        /// <summary>
        /// Checks if the given coordinates are out of the bounds of the map.
        /// </summary>
        /// <param name="point">The point to check for.</param>
        /// <returns>Whether or not the coordinates are outside of the bounds of the map.</returns>
        public static bool CheckForSafeTileCoordinates(Point point)
        {
            return point.X >= 0 && point.X < WorldClass.CurrentWorldWidth && point.Y >= 0 && point.Y < WorldClass.CurrentWorldHeight;
        }

        /// <summary>
        /// Checks if the given coordinates are out of the bounds of the map.
        /// </summary>
        /// <param name="position">The position to check for. This position is automatically converted into tile coordinates.</param>
        /// <returns>Whether or not the coordinates are outside of the bounds of the map.</returns>
        public static bool CheckForSafeTileCoordinates(Vector2 position)
        {
            Point point = (position / 16f).ToPoint();
            return point.X >= 0 && point.X < WorldClass.CurrentWorldWidth && point.Y >= 0 && point.Y < WorldClass.CurrentWorldHeight;
        }

        public static void ResetChunkDimensions()
        {
            ChunkSizeWidth = GameScreen.resolutionWidth / (int)Main.camera.zoomStrength / 16 + 16;
            ChunkSizeHeight = GameScreen.resolutionHeight / (int)Main.camera.zoomStrength / 16 + 16;
        }

        public static void UpdateSimpleChunk(Vector2 centerOfChunk, bool force = false)
        {
            if (!force && Math.Abs(previousChunkCoordinates.X - centerOfChunk.X / 16f) < 5 && Math.Abs(previousChunkCoordinates.Y - centerOfChunk.Y / 16f) < 5)
                return;

            int activeChunkX = (int)(centerOfChunk.X / 16f);
            int activeChunkY = (int)(centerOfChunk.Y / 16f);

            ChunkSizeWidth = GameScreen.resolutionWidth / (int)Main.camera.zoomStrength / 16 + 16;
            ChunkSizeHeight = GameScreen.resolutionHeight / (int)Main.camera.zoomStrength / 16 + 16;
            if (ChunkSizeWidth > WorldClass.activeWorldChunk.GetLength(0) - 1 || ChunkSizeHeight > WorldClass.activeWorldChunk.GetLength(1) - 1)
                WorldClass.activeWorldChunk = new Tile[ChunkSizeWidth, ChunkSizeHeight];

            previousChunkCoordinates = new Point(activeChunkX, activeChunkY);
            if (previousChunkDimensions.X != ChunkSizeWidth || previousChunkDimensions.Y != ChunkSizeHeight)
            {
                WorldClass.activeWorldChunk = new Tile[ChunkSizeWidth, ChunkSizeHeight];
                previousChunkDimensions = new Point(ChunkSizeWidth, ChunkSizeHeight);
            }

            if (activeChunkX - ChunkSizeWidth / 2 < 0)
                activeChunkX = ChunkSizeWidth / 2;
            else if (activeChunkX + ChunkSizeWidth / 2 >= WorldClass.CurrentWorldWidth)
                activeChunkX = WorldClass.CurrentWorldWidth - ChunkSizeWidth / 2;
            if (activeChunkY - ChunkSizeHeight / 2 < 0)
                activeChunkY = ChunkSizeHeight / 2;
            else if (activeChunkY + ChunkSizeHeight / 2 >= WorldClass.CurrentWorldHeight)
                activeChunkY = WorldClass.CurrentWorldHeight - ChunkSizeHeight / 2;
            for (int x = 0; x < ChunkSizeWidth; x++)
            {
                for (int y = 0; y < ChunkSizeHeight; y++)
                {
                    Point pos = new Point(activeChunkX - ChunkSizeWidth / 2 + x, activeChunkY - ChunkSizeHeight / 2 + y);

                    pos.X = Math.Clamp(pos.X, 0, WorldClass.CurrentWorldWidth - 1);
                    pos.Y = Math.Clamp(pos.Y, 0, WorldClass.CurrentWorldHeight - 1);
                    if (WorldClass.CurrentWorldWidth - 1 < pos.X || WorldClass.CurrentWorldHeight - 1 < pos.Y || WorldClass.activeWorldChunk.GetLength(0) - 1 < x || WorldClass.activeWorldChunk.GetLength(1) - 1 < y)
                        continue;

                    WorldClass.activeWorldChunk[x, y] = WorldClass.activeWorldData.tiles[pos.X, pos.Y];
                }
            }


            if (WorldClass.activeWorldData.staticWorldObjects.Count < 0)
                return;

            WorldClass.activeWorldObjects.Clear();
            int[] staticKeys = WorldClass.activeWorldData.staticWorldObjects.Keys.ToArray();
            for (int i = 0; i < WorldClass.activeWorldData.staticWorldObjects.Count; i++)
            {
                WorldObject otherObject = WorldClass.activeWorldData.staticWorldObjects[staticKeys[i]];
                if (Vector2.Distance(Main.camera.position, otherObject.position) <= ChunkSizeWidth * 16f)
                    WorldClass.activeWorldObjects.Add(otherObject);
            }
        }

        public static void UpdateActiveWorldChunk(Vector2 centerOfChunk)
        {
            if (Vector2.Distance(previousChunkCoordinates.ToVector2(), centerOfChunk / 16f) < 6f)
                return;

            int activeChunkX = (int)(centerOfChunk.X / 16f);
            int activeChunkY = (int)(centerOfChunk.Y / 16f);
            ChunkSizeWidth = GameScreen.resolutionWidth / (int)Main.camera.zoomStrength / 16 + 16;
            ChunkSizeHeight = GameScreen.resolutionHeight / (int)Main.camera.zoomStrength / 16 + 16;
            previousChunkCoordinates = new Point(activeChunkX, activeChunkY);
            if (previousChunkDimensions.X != ChunkSizeWidth || previousChunkDimensions.Y != ChunkSizeHeight)
            {
                WorldClass.activeWorldChunk = new Tile[ChunkSizeWidth, ChunkSizeHeight];
                previousChunkDimensions = new Point(ChunkSizeWidth, ChunkSizeHeight);
            }

            if (activeChunkX - ChunkSizeWidth / 2 < 0)
                activeChunkX = ChunkSizeWidth / 2;
            else if (activeChunkX + ChunkSizeWidth / 2 >= WorldClass.CurrentWorldWidth)
                activeChunkX = WorldClass.CurrentWorldWidth - ChunkSizeWidth / 2;
            if (activeChunkY - ChunkSizeHeight / 2 < 0)
                activeChunkY = ChunkSizeHeight / 2;
            else if (activeChunkY + ChunkSizeHeight / 2 >= WorldClass.CurrentWorldHeight)
                activeChunkY = WorldClass.CurrentWorldHeight - ChunkSizeHeight / 2;

            for (int x = 0; x < ChunkSizeWidth; x++)
            {
                for (int y = 0; y < ChunkSizeHeight; y++)
                {
                    Point pos = new Point(activeChunkX - ChunkSizeWidth / 2 + x, activeChunkY - ChunkSizeHeight / 2 + y);
                    pos.X = Math.Clamp(pos.X, 0, WorldClass.CurrentWorldWidth - 1);
                    pos.Y = Math.Clamp(pos.Y, 0, WorldClass.CurrentWorldHeight - 1);

                    WorldClass.activeWorldChunk[x, y] = WorldClass.activeWorldData.tiles[pos.X, pos.Y];
                }
            }

            WorldClass.activeWorldObjects.Clear();

            int[] priorityKeys = WorldClass.activeWorldData.priorityWorldObjects.Keys.ToArray();
            for (int i = 0; i < WorldClass.activeWorldData.priorityWorldObjects.Count; i++)
            {
                WorldClass.activeWorldObjects.Add(WorldClass.activeWorldData.priorityWorldObjects[priorityKeys[i]]);
            }

            int[] staticKeys = WorldClass.activeWorldData.staticWorldObjects.Keys.ToArray();
            for (int i = 0; i < WorldClass.activeWorldData.staticWorldObjects.Count; i++)
            {
                WorldObject otherObject = WorldClass.activeWorldData.staticWorldObjects[staticKeys[i]];
                if (Vector2.Distance(Main.camera.position, otherObject.position) <= ChunkSizeWidth * 16f)
                    WorldClass.activeWorldObjects.Add(otherObject);
            }

            int[] destroyableKeys = WorldClass.activeWorldData.destroyableWorldObjects.Keys.ToArray();
            for (int i = 0; i < WorldClass.activeWorldData.destroyableWorldObjects.Count; i++)
            {
                WorldObject otherObject = WorldClass.activeWorldData.destroyableWorldObjects[destroyableKeys[i]];
                if (Vector2.Distance(Main.camera.position, otherObject.position) <= ChunkSizeWidth * 16f)
                    WorldClass.activeWorldObjects.Add(otherObject);
            }
        }

        public static void ForceUpdateActiveWorldChunk(Vector2 centerOfChunk)
        {
            int activeChunkX = (int)(centerOfChunk.X / 16f);
            int activeChunkY = (int)(centerOfChunk.Y / 16f);
            ChunkSizeWidth = GameScreen.resolutionWidth / (int)Main.camera.zoomStrength / 16 + 16;
            ChunkSizeHeight = GameScreen.resolutionHeight / (int)Main.camera.zoomStrength / 16 + 16;
            previousChunkCoordinates = new Point(activeChunkX, activeChunkY);
            if (previousChunkDimensions.X != ChunkSizeWidth || previousChunkDimensions.Y != ChunkSizeHeight)
            {
                WorldClass.activeWorldChunk = new Tile[ChunkSizeWidth, ChunkSizeHeight];
                previousChunkDimensions = new Point(ChunkSizeWidth, ChunkSizeHeight);
            }

            if (activeChunkX - ChunkSizeWidth / 2 < 0)
                activeChunkX = ChunkSizeWidth / 2;
            else if (activeChunkX + ChunkSizeWidth / 2 >= WorldClass.CurrentWorldWidth)
                activeChunkX = WorldClass.CurrentWorldWidth - ChunkSizeWidth / 2;
            if (activeChunkY - ChunkSizeHeight / 2 < 0)
                activeChunkY = ChunkSizeHeight / 2;
            else if (activeChunkY + ChunkSizeHeight / 2 >= WorldClass.CurrentWorldHeight)
                activeChunkY = WorldClass.CurrentWorldHeight - ChunkSizeHeight / 2;
            for (int x = 0; x < ChunkSizeWidth; x++)
            {
                for (int y = 0; y < ChunkSizeHeight; y++)
                {
                    Point pos = new Point(activeChunkX - ChunkSizeWidth / 2 + x, activeChunkY - ChunkSizeHeight / 2 + y);
                    pos.X = Math.Clamp(pos.X, 0, WorldClass.CurrentWorldWidth - 1);
                    pos.Y = Math.Clamp(pos.Y, 0, WorldClass.CurrentWorldHeight - 1);

                    WorldClass.activeWorldChunk[x, y] = WorldClass.activeWorldData.tiles[pos.X, pos.Y];
                }
            }

            WorldClass.activeWorldObjects.Clear();

            int[] priorityKeys = WorldClass.activeWorldData.priorityWorldObjects.Keys.ToArray();
            for (int i = 0; i < WorldClass.activeWorldData.priorityWorldObjects.Count; i++)
            {
                WorldClass.activeWorldObjects.Add(WorldClass.activeWorldData.priorityWorldObjects[priorityKeys[i]]);
            }

            int[] staticKeys = WorldClass.activeWorldData.staticWorldObjects.Keys.ToArray();
            for (int i = 0; i < WorldClass.activeWorldData.staticWorldObjects.Count; i++)
            {
                WorldObject otherObject = WorldClass.activeWorldData.staticWorldObjects[staticKeys[i]];
                if (Vector2.Distance(Main.camera.position, otherObject.position) <= ChunkSizeWidth * 16f)
                    WorldClass.activeWorldObjects.Add(otherObject);
            }

            int[] destroyableKeys = WorldClass.activeWorldData.destroyableWorldObjects.Keys.ToArray();
            for (int i = 0; i < WorldClass.activeWorldData.destroyableWorldObjects.Count; i++)
            {
                WorldObject otherObject = WorldClass.activeWorldData.destroyableWorldObjects[destroyableKeys[i]];
                if (Vector2.Distance(Main.camera.position, otherObject.position) <= ChunkSizeWidth * 16f)
                    WorldClass.activeWorldObjects.Add(otherObject);
            }
        }

        public static void ForceUpdateEntireWorld()
        {
            for (int x = 0; x < WorldClass.CurrentWorldWidth; x++)
            {
                for (int y = 0; y < WorldClass.CurrentWorldHeight; y++)
                {
                    Point pos = new Point(x, y);
                    /*pos.X = Math.Clamp(pos.X, 0, WorldClass.CurrentWorldWidth - 1);
                    pos.Y = Math.Clamp(pos.Y, 0, WorldClass.CurrentWorldWidth - 1);*/

                    WorldClass.activeWorldData.tiles[pos.X, pos.Y].Update();
                }
            }

            int[] priorityKeys = WorldClass.activeWorldData.priorityWorldObjects.Keys.ToArray();
            for (int i = 0; i < WorldClass.activeWorldData.priorityWorldObjects.Count; i++)
            {
                WorldClass.activeWorldData.priorityWorldObjects[priorityKeys[i]].Update();
            }

            int[] staticKeys = WorldClass.activeWorldData.staticWorldObjects.Keys.ToArray();
            for (int i = 0; i < WorldClass.activeWorldData.staticWorldObjects.Count; i++)
            {
                WorldClass.activeWorldData.staticWorldObjects[staticKeys[i]].Update();
            }

            int[] destroyableKeys = WorldClass.activeWorldData.destroyableWorldObjects.Keys.ToArray();
            for (int i = 0; i < WorldClass.activeWorldData.destroyableWorldObjects.Count; i++)
            {
                WorldClass.activeWorldData.destroyableWorldObjects[destroyableKeys[i]].Update();
            }
        }
    }
}
