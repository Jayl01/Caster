using Caster.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Caster.Entities.Projectiles
{
    public class LaserRune : Projectile
    {
        public static Texture2D runeSymbolTexture;
        public static Texture2D laserSpritesheet;

        private readonly Point Size = new Point(24);
        private readonly Color RuneColor = Color.OrangeRed;
        private const int LifeTime = 8 * 60;

        private int lifeTimer = 0;
        private float baseRuneRotation;
        private int laserSegments = 0;
        private float runeScale = 0f;
        private int laserTravelDirection = 0;
        private float moveSpeed = 0f;
        private Rectangle laserHitbox;

        public override CollisionType collisionType => CollisionType.FriendlyProjectiles;
        public override CollisionType[] colliderTypes => new CollisionType[1] { CollisionType.Enemies };

        public static void NewLaserRune(Vector2 position, int travelDirection)
        {
            LaserRune laserRune = new LaserRune();
            laserRune.position = position;
            laserRune.laserTravelDirection = travelDirection;
            laserRune.Initialize();
            Main.activeProjectiles.Add(laserRune);
        }

        public override void Initialize()
        {
            moveSpeed = Main.random.Next(3, 12 + 1) / 100f;
            hitbox = new Rectangle((int)position.X, (int)position.Y, Size.X, Size.Y);
            laserHitbox = new Rectangle((int)position.X, (int)position.Y, 6, 1);
        }

        public override void Update()
        {
            lifeTimer++;
            if (lifeTimer >= LifeTime)
                DestroyInstance();

            int amountOfSmoke = Main.random.Next(1, 3 + 1);
            for (int i = 0; i < amountOfSmoke; i++)
            {
                Vector2 smokePosition = position + (new Vector2(Main.random.Next(0, Size.X + 1), Main.random.Next(0, Size.Y + 1)) - ((Size.ToVector2() / 2f))) * runeScale;
                Vector2 smokeVelocity = new Vector2(Main.random.Next(-4, 4 + 1), Main.random.Next(-4, 4 + 1)) / 12f;
                Smoke.NewSmokeParticle(smokePosition, smokeVelocity, Color.OrangeRed, Color.DarkRed, 15, 20, 10, foreground: true);
            }

            if (lifeTimer >= 2 * 60 && lifeTimer < LifeTime - 60)
            {
                for (int i = 0; i < 60; i++)
                {
                    if (DetectTileCollisionsByCollisionStyle(new Vector2(position.X, position.Y + (i * 4))))
                    {
                        laserSegments = i;
                        laserHitbox.Height = laserSegments * 4;
                        break;
                    }
                }
                if (laserHitbox.Intersects(Main.currentPlayer.hitbox))
                    Main.currentPlayer.Hurt();

                int amountOfLaserSmoke = Main.random.Next(4, 7 + 1);
                for (int i = 0; i < amountOfLaserSmoke; i++)
                {
                    Vector2 smokePosition = position + new Vector2(Main.random.Next(-4, 2 + 1), Main.random.Next(0, (laserSegments * 4) + 1));
                    Vector2 smokeVelocity = new Vector2(Main.random.Next(-6, 6 + 1), Main.random.Next(-4, 4 + 1)) / 12f;
                    Smoke.NewSmokeParticle(smokePosition, smokeVelocity, Color.OrangeRed, Color.DarkRed, 15, 20, 10, foreground: true);
                }

                float playerDistanceToLaser = Vector2.Distance(Main.currentPlayer.playerCenter, position + new Vector2(0f, laserSegments * 4));
                if (playerDistanceToLaser <= 6 * 16f)
                    Main.camera.ShakeCamera((int)((1f - (playerDistanceToLaser / (6f * 16f)))) + 1, 1);
            }

            else if (lifeTimer >= LifeTime - 60)
                runeScale = (LifeTime - (lifeTimer)) / 60f;
            else
                runeScale = ((lifeTimer + 1f) / 120f);

            baseRuneRotation += MathHelper.ToRadians(0.8f);
            position.X += laserTravelDirection * moveSpeed;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < laserSegments; i++)
            {
                Vector2 laserSegmentPosition = position + new Vector2(-3f, i * 4);
                int frame = Main.random.Next(0, 3 + 1);
                spriteBatch.Draw(laserSpritesheet, laserSegmentPosition, new Rectangle(0, frame * 4, 6, 4), Color.White);
            }

            spriteBatch.Draw(ElectricRune.baseRuneTexture, position, null, RuneColor, baseRuneRotation, new Vector2(12), runeScale, SpriteEffects.None, 0f);
            spriteBatch.Draw(ElectricRune.baseRuneInnerCircleTexture, position, null, RuneColor, -baseRuneRotation * 0.5f, new Vector2(12), runeScale, SpriteEffects.None, 0f);
            spriteBatch.Draw(runeSymbolTexture, position, null, RuneColor, 0f, new Vector2(12), runeScale, SpriteEffects.None, 0f);
        }
    }
}
