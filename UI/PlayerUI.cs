using AnotherLib;
using AnotherLib.UI;
using Caster.Effects;
using Caster.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Caster.UI
{
    public class PlayerUI : UIObject
    {
        public static Texture2D playerIndicatorTexture;
        private const float IndicatorRange = 80;        //How far it goes on each side
        private readonly Vector2 indicatorOrigin = new Vector2(6f, 7f) / 2f;

        public static PlayerUI NewPlayerUI()
        {
            PlayerUI playerUI = new PlayerUI();
            playerUI.Initialize();
            return playerUI;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 barLocation = (GameScreen.center / 3f) + new Vector2(0f, -(GameScreen.halfScreenHeight / 3f) + 5f);
            Vector2 pos1 = barLocation + new Vector2(-IndicatorRange, 0f);
            Vector2 pos2 = barLocation + new Vector2(IndicatorRange, 0f);

            spriteBatch.Draw(Smoke.smokePixelTextures[0], pos1, null, Color.White, 0f, Vector2.One, new Vector2(2f, 5f) / 2f, SpriteEffects.None, 0f);
            spriteBatch.Draw(Smoke.smokePixelTextures[0], pos2, null, Color.White, 0f, Vector2.One, new Vector2(2f, 5f) / 2f, SpriteEffects.None, 0f);

            float percentage = (int)(Main.currentPlayer.playerCenter.X / 16f) / (float)WorldClass.CurrentWorldWidth;
            Vector2 indicatorPos = Vector2.Lerp(pos1, pos2, percentage);

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Main.currentPlayer.direction == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;
            //spriteBatch.Draw(playerIndicatorTexture, indicatorPos, null, Color.White, 0f, indicatorOrigin, 1f, spriteEffects, 0f);

            percentage = (float)System.Math.Round(percentage * 100, 0);
            spriteBatch.DrawString(Main.gameFont, percentage + "%", barLocation + new Vector2(0f, 5f), Color.White, 0f, Main.gameFont.MeasureString(percentage + "%") / 2f, 0.5f, SpriteEffects.None, 0f);

        }
    }
}
