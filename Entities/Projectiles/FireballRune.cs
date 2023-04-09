using AnotherLib;
using AnotherLib.Input;
using AnotherLib.Utilities;
using Caster.Effects;
using Caster.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Caster.Entities.Projectiles
{
    public class FireballRune : Projectile
    {
        public static Texture2D runeSymbolTexture;

        private readonly Point Size = new Point(24);
        private readonly Color RuneColor = Color.Orange;
        private int spawnTimer = 0;
        private bool active = true;
        private float baseRuneRotation;
        private int fireballShootTimer = 0;
        private int activePlayTimer = 0;


        public override CollisionType collisionType => CollisionType.FriendlyProjectiles;
        public override CollisionType[] colliderTypes => new CollisionType[1] { CollisionType.Enemies };

        public static void NewFireballRune(Vector2 position)
        {
            FireballRune fireballRune = new FireballRune();
            fireballRune.position = position;
            fireballRune.Initialize();
            Main.activeProjectiles.Add(fireballRune);
        }

        public override void Initialize()
        {
            active = true;
            hitbox = new Rectangle((int)position.X, (int)position.Y, Size.X, Size.Y);
        }

        public override void Update()
        {
            if (!active)
            {
                spawnTimer -= 5;
                if (spawnTimer <= 0)
                    DestroyInstance();
                return;
            }

            if (!InputManager.IsMouseLeftHeld() || DetectTileCollisionsByCollisionStyle(GameData.MouseWorldPosition))
            {
                active = false;
                return;
            }

            if (spawnTimer < 60)
                spawnTimer++;
            if (fireballShootTimer > 0)
                fireballShootTimer--;
            activePlayTimer--;
            if (activePlayTimer <= 0)
            {
                activePlayTimer = 2 * 60;
                SoundPlayer.PlaySoundFromOtherSource(Sounds.FireballRune_Active, this, 12);
            }

            position = GameData.MouseWorldPosition;
            hitbox.X = (int)position.X;
            hitbox.Y = (int)position.Y;
            baseRuneRotation += MathHelper.ToRadians(0.5f);

            int amountOfSmoke = Main.random.Next(1, 3 + 1);
            for (int i = 0; i < amountOfSmoke; i++)
            {
                Vector2 smokePosition = position + (new Vector2(Main.random.Next(0, Size.X + 1), Main.random.Next(0, Size.Y + 1)) - ((Size.ToVector2() / 2f))) * (spawnTimer / 60f);
                Vector2 smokeVelocity = new Vector2(Main.random.Next(-4, 4 + 1), Main.random.Next(-4, 4 + 1)) / 12f;
                Smoke.NewSmokeParticle(smokePosition, smokeVelocity, Color.Orange, Color.DarkRed, 15, 20, 10, foreground: true);
            }

            if (spawnTimer >= 60 && fireballShootTimer <= 0)
            {
                fireballShootTimer += Main.random.Next(5, 10 + 1);
                Fireball.NewFireball(position, new Vector2(Main.random.Next(-40, 40 + 1) / 10f, 7f));
                SoundPlayer.PlaySoundFromOtherSource(Sounds.FireballRune_Shoot, position, 16);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(ElectricRune.baseRuneTexture, position, null, RuneColor * (spawnTimer / 60f), baseRuneRotation, new Vector2(12), (spawnTimer / 60f), SpriteEffects.None, 0f);
            spriteBatch.Draw(ElectricRune.baseRuneInnerCircleTexture, position, null, RuneColor * (spawnTimer / 60f), -baseRuneRotation * 0.5f, new Vector2(12), (spawnTimer / 60f), SpriteEffects.None, 0f);
            spriteBatch.Draw(runeSymbolTexture, position, null, RuneColor * (spawnTimer / 60f), 0f, new Vector2(12), (spawnTimer / 60f), SpriteEffects.None, 0f);
        }

        public static void DrawLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color)
        {
            Vector2 lineVector = end - start;
            Vector2 drawScale = new Vector2(lineVector.Length(), 1f) * 0.5f;
            spriteBatch.Draw(Smoke.smokePixelTextures[0], start, null, color, lineVector.GetRotation(), Vector2.Zero, drawScale, SpriteEffects.None, 0f);
        }
    }
}
