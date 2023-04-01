using AnotherLib;
using Caster;
using Caster.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Caster.Effects
{
    public class Smoke : VisualEffect
    {
        public static Texture2D[] smokePixelTextures;
        public static readonly int[] smokeTextureSize = new int[3] { 2, 3, 5 };

        public const int WhitePixelTexture = 0;
        public const int StarPixelTexture = 1;
        public const int CirclePixelTexture = 2;

        private int smokeTextureType;
        private Vector2 velocity = Vector2.Zero;
        private Color smokeColor = Color.White;
        private float rotation;
        private float rotationToAdd;
        private float scale;
        private float drawAlpha = 1f;
        private Vector2 smokeOrigin;
        private bool scaleable;
        private float startScale;
        private float endScale;

        private int colorChangeTimerStart = 0;
        private int colorChangeTimer = 0;
        public int lifeTimer = 0;
        private int lifeTime = 0;
        private int fadeTime = 0;
        private ColorData startColor;       //TODO: Using the ColorData struct leads updates Times to be ~1-2ms faster; Determine if the GC hates this and counters the gain
        private ColorData endColor;

        private struct ColorData        //Getting individual color data is EXPENSIVE!; This alternative spends less time per particle
        {
            public byte R;
            public byte G;
            public byte B;

            public ColorData(byte r, byte g, byte b)
            {
                R = r;
                G = g;
                B = b;
            }
        }

        /// <summary>
        /// Spawns a new smoke particle with the given information.
        /// </summary>
        /// <param name="position">The position the smoke will spawn in.</param>
        /// <param name="velocity">The velocity the smoke will retain (The velocity doesn't dampen!)</param>
        /// <param name="startColor">The color of the smoke particle before its life Timer surpasses it's color change Timer.</param>
        /// <param name="endColor">The color the smoke will fade into once the life Timer surpasses it's color change Timer.</param>
        /// <param name="colorChangeTime">The amount of Time it will take for the color change effect to take place (i.e, the amount of Time it will take to change into the end color.)</param>
        /// <param name="lifeTime">The amount of Time (in frames) that the smoke will remain in-game for.</param>
        /// <param name="fadeTime">The Time until the smoke starts fading away.</param>
        /// <param name="scale">The scale of the smoke.</param>
        /// <param name="rotation">The rotation of the smoke, in Degrees, that will continually be added to the smoke.</param>
        /// <param name="textureType">The type of texture the smoke will use.</param>
        public static Smoke NewSmokeParticle(Vector2 position, Vector2 velocity, Color startColor, Color endColor, int colorChangeTime, int lifeTime, int fadeTime = 60, float scale = 0.4f, float rotation = 0f, int textureType = 0, bool foreground = false)
        {
            Smoke newInstance = new Smoke();
            newInstance.position = position;
            newInstance.velocity = velocity;
            newInstance.rotationToAdd = MathHelper.ToRadians(rotation);
            newInstance.startScale = newInstance.scale = scale;
            newInstance.smokeColor = startColor;
            newInstance.startColor = new ColorData(startColor.R, startColor.G, startColor.B);
            newInstance.endColor = new ColorData(endColor.R, endColor.G, endColor.B);
            newInstance.lifeTimer = lifeTime;
            newInstance.fadeTime = fadeTime;
            newInstance.colorChangeTimer = colorChangeTime;
            newInstance.colorChangeTimerStart = colorChangeTime;
            newInstance.smokeTextureType = textureType;
            newInstance.foreground = foreground;
            if (rotation != 0f)
                newInstance.smokeOrigin = new Vector2(smokeTextureSize[textureType] / 2f, smokeTextureSize[textureType] / 2f);

            if (foreground)
                Main.activeForegroundEffects.Add(newInstance);
            else
                Main.activeBackgroundEffects.Add(newInstance);

            return newInstance;
        }

        /// <summary>
        /// Spawns a new smoke particle with the given information. These particles die when their scale hits zero.
        /// </summary>
        /// <param name="position">The position the smoke will spawn in.</param>
        /// <param name="velocity">The velocity the smoke will retain (The velocity doesn't dampen!)</param>
        /// <param name="startColor">The color of the smoke particle before its life Timer surpasses it's color change Timer.</param>
        /// <param name="endColor">The color the smoke will fade into once the life Timer surpasses it's color change Timer.</param>
        /// <param name="colorChangeTime">The amount of Time it will take for the color change effect to take place</param>
        /// <param name="lifeTime">The amount of Time (in frames) that the smoke will remain in-game for.</param>
        /// <param name="fadeTime">The Time until the smoke starts fading away.</param>
        /// <param name="startScale">The scale of the smoke.</param>
        /// <param name="rotation">The rotation of the smoke, in Degrees, that will continually be added to the smoke.</param>
        /// <param name="textureType">The type of texture the smoke will use.</param>
        public static void NewScaleableSmokeParticle(Vector2 position, Vector2 velocity, Color startColor, Color endColor, int colorChangeTime, int lifeTime, int fadeTime = 60, float startScale = 0.4f, float endScale = 0.1f, float rotation = 0f, int textureType = 0, bool foreground = false)
        {
            Smoke newInstance = new Smoke();
            newInstance.position = position;
            newInstance.velocity = velocity;
            newInstance.rotationToAdd = MathHelper.ToRadians(rotation);
            newInstance.scale = startScale;
            newInstance.startScale = startScale;
            newInstance.smokeColor = startColor;
            newInstance.startColor = new ColorData(startColor.R, startColor.G, startColor.B);
            newInstance.endColor = new ColorData(endColor.R, endColor.G, endColor.B);
            newInstance.lifeTimer = lifeTime;
            newInstance.lifeTime = lifeTime;
            newInstance.fadeTime = fadeTime;
            newInstance.colorChangeTimer = colorChangeTime;
            newInstance.colorChangeTimerStart = colorChangeTime;
            newInstance.smokeTextureType = textureType;
            newInstance.foreground = foreground;
            newInstance.scaleable = true;
            newInstance.endScale = endScale;
            if (rotation != 0f)
                newInstance.smokeOrigin = new Vector2(smokeTextureSize[textureType] / 2f, smokeTextureSize[textureType] / 2f);

            if (foreground)
                Main.activeForegroundEffects.Add(newInstance);
            else
                Main.activeBackgroundEffects.Add(newInstance);
        }

        public override void Update()
        {
            lifeTimer--;
            if (lifeTimer <= fadeTime)
                drawAlpha = (float)lifeTimer / (float)fadeTime;

            if (lifeTimer <= 0)
            {
                DestroyInstance();
                return;
            }

            position += velocity;
            rotation += rotationToAdd;

            if (colorChangeTimer > 0)
            {
                colorChangeTimer--;
                smokeColor.R = (byte)MathHelper.Lerp(startColor.R, endColor.R, 1f - ((float)colorChangeTimer / (float)colorChangeTimerStart));
                smokeColor.G = (byte)MathHelper.Lerp(startColor.G, endColor.G, 1f - ((float)colorChangeTimer / (float)colorChangeTimerStart));
                smokeColor.B = (byte)MathHelper.Lerp(startColor.B, endColor.B, 1f - ((float)colorChangeTimer / (float)colorChangeTimerStart));
            }

            if (scaleable)
                scale = MathHelper.Lerp(startScale, endScale, 1f - (lifeTimer / (float)lifeTime));
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            /*float widthx = GameScreen.halfScreenWidth / (Main.camera.zoomStrength - 0.5f);
            float widthy = GameScreen.halfScreenHeight / (Main.camera.zoomStrength - 0.5f);
            bool renderx = (Main.currentPlayer.playerCenter.X - widthx <= position.X) && (Main.currentPlayer.playerCenter.X + widthx >= position.X);
            bool rendery = (Main.currentPlayer.playerCenter.Y - widthy <= position.Y) && (Main.currentPlayer.playerCenter.Y + widthy >= position.Y);

            //For some weird reason these checks completely destroy the garbage collector, but it boosts FPS by about 6-7 frames so whatever
            if (!renderx || !rendery)        //It's not visible on either case
                return;*/

            spriteBatch.Draw(smokePixelTextures[smokeTextureType], position, null, smokeColor * drawAlpha, rotation, smokeOrigin, scale, SpriteEffects.None, 0f);
        }
    }
}