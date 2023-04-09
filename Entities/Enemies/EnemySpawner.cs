using AnotherLib;
using Caster.World;
using Microsoft.Xna.Framework;

namespace Caster.Entities.Enemies
{
    public class EnemySpawner
    {
        public const int EnemySpawnTime = 3 * 60;

        private int enemySpawnTimer = 0;

        public void Update()
        {
            enemySpawnTimer++;
            int enemySpawnTime = EnemySpawnTime - (Enemy.EnemiesKilled / 2);
            if (enemySpawnTime < 45)
                enemySpawnTime = 45;

            if (enemySpawnTimer >= enemySpawnTime)
            {
                enemySpawnTimer = 0;
                int direction = 1;
                if (Main.random.Next(0, 1 + 1) == 0)
                    direction = -1;
                Vector2 spawnPos = new Vector2(Main.currentPlayer.playerCenter.X + (GameScreen.halfScreenWidth * direction), (int)(Main.currentPlayer.playerCenter.Y / 16f) * 16);
                if (!ChunkLoader.CheckForSafeTileCoordinates(spawnPos))
                    return;

                while (ChunkLoader.CheckForSafeTileCoordinates(spawnPos) && !(WorldClass.activeWorldData.tiles[(int)(spawnPos.X / 16f), (int)(spawnPos.Y / 16f) + 1].isCollideable && !WorldClass.activeWorldData.tiles[(int)(spawnPos.X / 16f), (int)(spawnPos.Y / 16f)].isCollideable))
                {
                    if (WorldClass.activeWorldData.tiles[(int)(spawnPos.X / 16f), (int)(spawnPos.Y / 16f)].isCollideable && WorldClass.activeWorldData.tiles[(int)(spawnPos.X / 16f), (int)(spawnPos.Y / 16f) + 1].isCollideable)
                        spawnPos.Y -= 16f;
                    else if (!WorldClass.activeWorldData.tiles[(int)(spawnPos.X / 16f), (int)(spawnPos.Y / 16f)].isCollideable && !WorldClass.activeWorldData.tiles[(int)(spawnPos.X / 16f), (int)(spawnPos.Y / 16f) + 1].isCollideable)
                        spawnPos.Y += 16f;
                }

                int enemyType = Main.random.Next(0, 1 + 1);
                if (enemyType == 0)
                    EvilCaster.NewEvilCaster(spawnPos);
                else if (enemyType == 1)
                    MatterCaster.NewMatterCaster(spawnPos + new Vector2(0f, -8f));
            }
        }
    }
}
