using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Caster.Effects
{
    public abstract class VisualEffect
    {
        public int frame = 0;
        public int frameCounter = 0;
        public bool foreground = false;
        public Vector2 position;

        public virtual void Update()
        { }

        public virtual void Draw(SpriteBatch spriteBatch)
        { }

        public void DestroyInstance()
        {
            if (foreground)
                Main.activeForegroundEffects.Remove(this);
            else
                Main.activeBackgroundEffects.Remove(this);
        }
    }
}
