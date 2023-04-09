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
        public static Texture2D cloudSpritesheet;
        private const int CloudLifeTime = 185 * 60;
        private readonly Vector2 MoonTextureSize = new Vector2(48f);
        private List<Smoke> activeStars;
        private List<CloudData> activeClouds;
        private int cloudSimulationTime;

        private struct CloudData
        {
            public Vector2 position;
            public Vector2 velocity;
            public int startTime;
            public int layer;
            public int cloudType;
        }

        public void Initialize()
        {
            activeStars = new List<Smoke>();
            activeClouds = new List<CloudData>();
            cloudSimulationTime = 4 * 60;

            for (int i = 0; i < 240; i++)
            {
                Vector2 smokePos = new Vector2(Main.random.Next(1, GameScreen.resolutionWidth), Main.random.Next(1, GameScreen.halfScreenHeight));
                Color startColor = Color.LightYellow * 0.8f * (1f - (smokePos.Y / (float)GameScreen.halfScreenHeight));
                Color endColor = Color.Transparent;
                Smoke smoke = Smoke.NewSmokeParticle(smokePos, Vector2.Zero, startColor, endColor, 20 * 60, 20 * 60, 10 * 60, 1.6f, foreground: true);
                activeStars.Add(smoke);
                Main.activeForegroundEffects.Remove(smoke);
            }

            for (int i = 0; i < 45; i++)
            {
                int layer = Main.random.Next(1, 3 + 1);
                int direction = Main.random.Next(0, 1 + 1) == 0 ? 1 : -1;
                Vector2 cloudPos = new Vector2(Main.random.Next(1, GameScreen.resolutionWidth), Main.random.Next(1, GameScreen.halfScreenHeight / 3) - (layer * 3));
                Vector2 cloudVel = new Vector2((Main.random.Next(2, (7 - layer) + 1) * -direction) / 24f, 0f) / (float)layer;
                CloudData newCloud = new CloudData()
                {
                    position = cloudPos,
                    velocity = cloudVel,
                    startTime = Main.random.Next(0, 4 * 60),
                    layer = layer,
                    cloudType = Main.random.Next(0, 2 + 1)
                };
                activeClouds.Add(newCloud);
            }
        }

        public void Update()
        {
            cloudSimulationTime++;
            if (Main.random.Next(1, 3 + 1) == 1)
            {
                Vector2 smokePos = new Vector2(Main.random.Next(1, GameScreen.resolutionWidth), Main.random.Next(1, GameScreen.halfScreenHeight));
                Color startColor = Color.Transparent;
                Color endColor = Color.LightYellow * 0.8f * (1f - (smokePos.Y / (float)GameScreen.halfScreenHeight));
                Smoke smoke = Smoke.NewSmokeParticle(smokePos, Vector2.Zero, startColor, endColor, 20 * 60, 20 * 60, 10 * 60, 1.6f, foreground: true);
                activeStars.Add(smoke);
                Main.activeForegroundEffects.Remove(smoke);
            }

            if (Main.random.Next(1, 75 + 1) == 1)
            {
                int layer = Main.random.Next(1, 3 + 1);
                int direction = Main.random.Next(0, 1 + 1) == 0 ? 1 : -1;
                Vector2 cloudPos = new Vector2(-32 * 3f, Main.random.Next(1, GameScreen.halfScreenHeight / 3) - (layer * 3));
                if (direction == 1)
                    cloudPos.X = GameScreen.resolutionWidth + (32f * 3f);
                Vector2 cloudVel = new Vector2((Main.random.Next(2, (7 - layer) + 1) * -direction) / 24f, 0f) / (float)layer;
                CloudData newCloud = new CloudData()
                {
                    position = cloudPos,
                    velocity = cloudVel,
                    startTime = cloudSimulationTime,
                    layer = layer,
                    cloudType = Main.random.Next(0, 2 + 1)
                };
                activeClouds.Add(newCloud);
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

            for (int i = 0; i < activeClouds.Count; i++)
            {
                if (cloudSimulationTime - activeClouds[i].startTime > CloudLifeTime)
                {
                    activeClouds.RemoveAt(i);
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

            spriteBatch.Draw(moonTexture, new Vector2(GameScreen.resolutionWidth / 2f, GameScreen.resolutionHeight / 8f), null, Color.White, 0f, MoonTextureSize / 2f, 3f, SpriteEffects.None, 0f);

            for (int layer = 3; layer >= 1; layer--)
            {
                for (int i = 0; i < activeClouds.Count; i++)
                {
                    if (activeClouds[i].layer == layer)
                    {
                        float alpha = cloudSimulationTime - activeClouds[i].startTime - (CloudLifeTime - (2 * 60));
                        alpha = 1f - (MathHelper.Clamp(alpha, 0f, 120f) / 120f);
                        alpha *= 0.9f - (0.1f * layer);
                        Rectangle animRect = new Rectangle(0, activeClouds[i].cloudType * 16, 32, 16);
                        spriteBatch.Draw(cloudSpritesheet, activeClouds[i].position + (activeClouds[i].velocity * (cloudSimulationTime - activeClouds[i].startTime)), animRect, Color.Lerp(Color.White, Color.Purple, activeClouds[i].layer / 3f) * alpha, 0f, Vector2.Zero, 3.3f - (0.3f * activeClouds[i].layer), SpriteEffects.None, 0f);
                    }
                }
            }
        }
    }
}
