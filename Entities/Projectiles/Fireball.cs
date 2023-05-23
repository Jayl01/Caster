using AnotherLib.Collision;
using AnotherLib.Utilities;
using Caster.Effects;
using Caster.Entities.Enemies;
using Caster.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Caster.Entities.Projectiles
{
    public class Fireball : Projectile
    {
        public static Texture2D fireballTexture;

        private readonly Point Size = new Point(12);
        private int spawnTimer = 0;
        private Vector2 velocity;

        public override CollisionType collisionType => CollisionType.FriendlyProjectiles;
        public override CollisionType[] colliderTypes => new CollisionType[1] { CollisionType.Enemies };
        public override bool CanBeThrownAround => true;

        public static void NewFireball(Vector2 position, Vector2 velocity)
        {
            Fireball fireball = new Fireball();
            fireball.position = position;
            fireball.velocity = velocity;
            fireball.Initialize();
            Main.activeProjectiles.Add(fireball);
        }

        public override void Initialize()
        {
            hitbox = new Rectangle((int)position.X, (int)position.Y, Size.X, Size.Y);
        }

        public override void Update()
        {
            if (spawnTimer < 10)
                spawnTimer++;

            int amountOfFireballSmoke = Main.random.Next(1, 3 + 1);
            for (int i = 0; i < amountOfFireballSmoke; i++)
            {
                Vector2 smokePosition = position + new Vector2(Main.random.Next(0, Size.X + 1), Main.random.Next(0, Size.Y + 1));
                Vector2 smokeVelocity = new Vector2(Main.random.Next(-4, 4 + 1), Main.random.Next(-4, 4 + 1)) / 12f;
                Color startColor = Color.Orange;
                Color endColor = Color.Gray;
                bool alternateStyle = Main.random.Next(0, 1 + 1) == 0;
                if (alternateStyle)
                {
                    startColor = Color.Yellow;
                    endColor = Color.DarkOrange;
                }
                Smoke.NewSmokeParticle(smokePosition, smokeVelocity, startColor, endColor, 30, 30, 20, foreground: true);
            }

            UpdateThrowPhysics();
            position += velocity + throwVelocity;
            hitbox.X = (int)position.X;
            hitbox.Y = (int)position.Y;
            DetectCollisions(Main.activeEnemies);
            if (DetectTileCollisionsByCollisionStyle(position + velocity))
            {
                int amountOfSmoke = Main.random.Next(12, 25 + 1);
                for (int i = 0; i < amountOfSmoke; i++)
                {
                    Vector2 smokePosition = position + new Vector2(Main.random.Next(0, Size.X + 1), Main.random.Next(0, Size.Y + 1));
                    Vector2 smokeVelocity = velocity * (-Main.random.Next(4, 7 + 1) / 40f);
                    Smoke.NewSmokeParticle(smokePosition, smokeVelocity, Color.Orange, Color.Gray, 30, 30, 20, foreground: true);
                }

                if (Vector2.Distance(Main.currentPlayer.playerCenter, position) <= 6 * 16)
                    Main.camera.ShakeCamera(2, 10);
                DestroyInstance();
                SoundPlayer.PlaySoundFromOtherSource(Sounds.Fireball_Land, position, 12, soundPitch: Main.random.Next(-7, 7 + 1) / 10f);
            }
        }

        public override void HandleCollisions(CollisionBody collider, CollisionType colliderType)
        {
            Enemy enemy = collider as Enemy;
            enemy.HurtEnemy(Main.random.Next(12, 15 + 1) + Enemy.EnemiesKilled);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(fireballTexture, position, Color.White * (spawnTimer / 10f));
        }
    }
}
