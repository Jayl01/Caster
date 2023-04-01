using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Caster.World.WorldObjects
{
    public class WorldObject
    {
        public virtual bool advancedDraw { get; } = false;
        public virtual bool affectedByYSort { get; } = true;
        public virtual bool noCollision { get; } = false;
        public virtual byte objectType { get; } = 0;
        public virtual bool priorityObject { get; } = false;

        public Texture2D objectTexture;
        public Vector2 position;
        public Vector2 origin;
        public Rectangle collisionRect;
        public bool flipped = false;
        public int objectHeight = 0;
        public int objectID = 0;

        public virtual void Initialize()
        { }

        public virtual void Update()
        { }

        public virtual void ClearRemainingData()
        { }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            /*float widthx = GameScreen.halfScreenWidth / (Main.camera.zoomStrength - 0.5f);
            float widthy = GameScreen.halfScreenHeight / (Main.camera.zoomStrength - 0.5f);
            bool renderx = (Main.currentPlayer.playerCenter.X - widthx <= position.X) && (Main.currentPlayer.playerCenter.X + widthx >= position.X);
            bool rendery = (Main.currentPlayer.playerCenter.Y - widthy <= position.Y) && (Main.currentPlayer.playerCenter.Y + widthy >= position.Y);


            if (!(renderx && rendery))      //World Objects are already chunk-based
            {
                return;
            }*/

            if (!advancedDraw)
            {
                spriteBatch.Draw(objectTexture, position, Color.White);
            }
            else
            {
                SpriteEffects effect = SpriteEffects.None;
                if (flipped)
                    effect = SpriteEffects.FlipHorizontally;

                spriteBatch.Draw(objectTexture, position, null, Color.White, 0f, origin, 1f, effect, 0f);
            }
        }
    }
}
