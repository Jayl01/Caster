using AnotherLib.Collision;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Caster.Effects;
using Caster.Entities.Enemies;
using AnotherLib;
using AnotherLib.Input;

namespace Caster.Entities.Projectiles
{
    public class ElectricRune : Projectile
    {
        public static Texture2D runeTexture;

        private Vector2 velocity;
        private int direction;
        private readonly Point Size = new Point(7, 3);
        private int transparencyTimer = 0;

        public override CollisionType collisionType => CollisionType.FriendlyProjectiles;
        public override CollisionType[] colliderTypes => new CollisionType[1] { CollisionType.Enemies };

        public static void NewBullet(Vector2 position, Vector2 velocity)
        {
            ElectricRune playerBullet = new ElectricRune();
            playerBullet.position = position;
            playerBullet.velocity = velocity;
            playerBullet.Initialize();
            Main.activeProjectiles.Add(playerBullet);
        }

        public override void Initialize()
        {
            if (velocity.X > 0)
                direction = 1;
            else
                direction = -1;
            hitbox = new Rectangle((int)position.X, (int)position.Y, Size.X, Size.Y);
        }

        public override void Update()
        {
            if (!InputManager.IsMouseRightHeld() || DetectTileCollisionsByCollisionStyle(GameData.MouseWorldPosition))
            {
                DestroyInstance();
                return;
            }

            if (transparencyTimer < 60)
                transparencyTimer++;

            position += velocity;
            hitbox.X = (int)position.X;
            hitbox.Y = (int)position.Y;
            Smoke.NewSmokeParticle(position + new Vector2(Main.random.Next(0, Size.X + 1), Main.random.Next(0, Size.Y + 1)), -velocity * 0.02f, Color.Yellow, Color.Orange, 10, 10, 0);
            DetectCollisions(Main.activeEnemies);
        }

        public override void HandleCollisions(CollisionBody collider, CollisionType colliderType)
        {
            Enemy enemy = collider as Enemy;
            enemy.HurtEnemy(1);
            DestroyInstance();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(runeTexture, position, Color.White * (transparencyTimer / 60f));
        }
    }
}
