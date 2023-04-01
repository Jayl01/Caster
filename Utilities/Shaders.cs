using AnotherLib.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Caster.Utilities
{
    public class Shaders
    {
        public static Effect gradientEffect;
        private static readonly Color bottomGradientColor = new Color(59, 13, 81);
        private static readonly Color topGradientColor = new Color(25, 4, 56);


        public static void DrawAllScreenEffectShaders(RenderTarget2D gameTarget)
        {
            gradientEffect.Parameters["bottomColor"].SetValue(bottomGradientColor.ToVector3());
            gradientEffect.Parameters["topColor"].SetValue(topGradientColor.ToVector3());
        }
    }
}
