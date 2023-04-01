using AnotherLib;
using AnotherLib.Drawing;
using Caster.Effects;
using Caster.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Caster.World
{
    public class WorldDetails
    {
        public Texture2D background;

        public static Texture2D moonTexture;
        private readonly Vector2 MoonTextureSize = new Vector2(128f);
        private List<Smoke> activeStars;

        public void Initialize()
        {
            activeStars = new List<Smoke>();
        }

        public void Update()
        {
            if (Main.random.Next(1, 3 + 1) == 1)
            {
                Vector2 smokePos = new Vector2(Main.random.Next(1, GameScreen.resolutionWidth), Main.random.Next(1, GameScreen.halfScreenHeight));
                Color startColor = Color.Transparent;
                Color endColor = Color.LightYellow * 0.8f * (1f - (smokePos.Y / (float)GameScreen.halfScreenHeight));
                Smoke smoke = Smoke.NewSmokeParticle(smokePos, Vector2.Zero, startColor, endColor, 20 * 60, 20 * 60, 10 * 60, 1.6f, foreground: true);
                activeStars.Add(smoke);
                Main.activeForegroundEffects.Remove(smoke);
            }

            for (int i = 0; i < activeStars.Count; i++)
            {
                activeStars[i].Update();
                if (activeStars[i].lifeTimer <= 1)
                {
                    activeStars.RemoveAt(i);
                    i--;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            ShaderBatch.DrawShaderItemImmediately(GameData.CurrentGameDrawInfo, Smoke.smokePixelTextures[0], GameScreen.ScreenRectangle, null, 0f, Vector2.Zero, SpriteEffects.None, Shaders.gradientEffect);

            foreach (Smoke smoke in activeStars)
            {
                smoke.Draw(spriteBatch);
            }

            //spriteBatch.Draw(moonTexture, new Vector2(GameScreen.resolutionWidth, 0f), null, Color.White, 0f, new Vector2(MoonTextureSize.X, 0f), 3f, SpriteEffects.None, 0f);
        }
    }
}
