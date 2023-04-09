using AnotherLib;
using AnotherLib.Input;
using AnotherLib.UI;
using AnotherLib.UI.UIElements;
using Caster.Effects;
using Caster.Entities.Players;
using Caster.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Caster.UI
{
    public class PlayerUI : UIObject
    {
        public static Texture2D playerIndicatorTexture;
        public static Texture2D[] runeSymbolTextures;
        public static Texture2D heartSymbol;
        private const float IndicatorRange = 95;        //How far it goes on each side
        private readonly Vector2 indicatorOrigin = new Vector2(11f, 10f) / 2f;

        private TextureButton[] runeButtons;
        private int scrollChangeCooldown = 0;

        private readonly Dictionary<int, Color[]> UIColors = new Dictionary<int, Color[]>()
        {
            { Player.Rune_Electric, new Color[2] { Color.Yellow, Color.Orange } },
            { Player.Rune_Fire, new Color[2] { Color.Red, Color.Yellow} },
            { Player.Rune_Gravity, new Color[2] { Color.Magenta, Color.Blue } }
        };

        public static PlayerUI NewPlayerUI()
        {
            PlayerUI playerUI = new PlayerUI();
            playerUI.Initialize();
            return playerUI;
        }

        public override void Initialize()
        {
            runeButtons = new TextureButton[3];
            for (int i = 0; i < runeButtons.Length; i++)
            {
                Vector2 buttonPosition = (GameScreen.center / 3f) + new Vector2(24f * (i - 1), (GameScreen.halfScreenHeight / 3f) - 20f);
                runeButtons[i] = new TextureButton(runeSymbolTextures[i], buttonPosition, 10, 10, 1f, 1.2f, Color.White, UIColors[i][1], true);
            }
        }

        public override void ReInitializePositions()
        {
            for (int i = 0; i < runeButtons.Length; i++)
            {
                runeButtons[i].buttonPosition = (GameScreen.center / 3f) + new Vector2(24f * (i - 1), (GameScreen.halfScreenHeight / 3f) - 20f);
            }
        }

        public override void Update()
        {
            if (scrollChangeCooldown > 0)
                scrollChangeCooldown--;

            int deltaScroll = InputManager.currentMouseState.ScrollWheelValue - InputManager.previousMouseState.ScrollWheelValue;
            if (deltaScroll != 0 && scrollChangeCooldown <= 0)
            {
                scrollChangeCooldown += 2;
                Player.SelectedRuneType += Math.Abs(deltaScroll) / deltaScroll;
                if (Player.SelectedRuneType < 0)
                    Player.SelectedRuneType = 2;
                if (Player.SelectedRuneType >= 3)
                    Player.SelectedRuneType = 0;
            }

            for (int i = 0; i < runeButtons.Length; i++)
            {
                runeButtons[i].Update();
                if (runeButtons[i].buttonPressed)
                    Player.SelectedRuneType = i;
                runeButtons[i].focused = Player.SelectedRuneType == i;
                if (runeButtons[i].focused)
                    runeButtons[i].drawColor = UIColors[Player.SelectedRuneType][0];
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 barLocation = (GameScreen.center / 3f) + new Vector2(0f, -(GameScreen.halfScreenHeight / 3f) + 5f);
            Vector2 pos1 = barLocation + new Vector2(-IndicatorRange, 0f);
            Vector2 pos2 = barLocation + new Vector2(IndicatorRange, 0f);

            spriteBatch.Draw(Smoke.smokePixelTextures[0], pos1, null, Color.White, 0f, Vector2.One, new Vector2(2f, 5f) / 2f, SpriteEffects.None, 0f);
            spriteBatch.Draw(Smoke.smokePixelTextures[0], pos2, null, Color.White, 0f, Vector2.One, new Vector2(2f, 5f) / 2f, SpriteEffects.None, 0f);
            int amountOfTicks = 20;
            for (int i = 1; i < amountOfTicks; i++)
            {
                spriteBatch.Draw(Smoke.smokePixelTextures[0], Vector2.Lerp(pos1, pos2, i / (float)amountOfTicks), null, Color.White, 0f, Vector2.One, new Vector2(5f, 2f) / 2f, SpriteEffects.None, 0f);
            }

            float percentage = (int)(Main.currentPlayer.playerCenter.X / 16f) / (float)WorldClass.CurrentWorldWidth;
            Vector2 indicatorPos = Vector2.Lerp(pos1, pos2, percentage);

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Main.currentPlayer.direction == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(playerIndicatorTexture, indicatorPos, null, Color.White, 0f, indicatorOrigin, 1f, spriteEffects, 0f);

            for (int i = 0; i < 3; i++)
            {
                Vector2 heartDrawPosition = barLocation + new Vector2((8 * (i - 1)) - 6f, 8f);
                Rectangle destinationRect = new Rectangle(heartDrawPosition.ToPoint(), new Point(12, 12));
                spriteBatch.Draw(heartSymbol, destinationRect, Main.currentPlayer.playerHealth >= i + 1 ? Color.Yellow : Color.Red);
            }

            percentage = (float)Math.Round(percentage * 100, 0);
            spriteBatch.DrawString(Main.gameFont, percentage + "%", barLocation + new Vector2(0f, 5f), Color.White, 0f, Main.gameFont.MeasureString(percentage + "%") / 2f, 0.5f, SpriteEffects.None, 0f);

            for (int i = 0; i < runeButtons.Length; i++)
            {
                runeButtons[i].Draw(spriteBatch);
                if (i == 2)
                {
                    Vector2 numberPosition = runeButtons[i].buttonPosition + (new Vector2(runeButtons[i].buttonWidth + 2, runeButtons[i].buttonHeight + 2) * runeButtons[i].scale);
                    spriteBatch.DrawString(Main.gameFont, Main.currentPlayer.gravityRuneUsesLeft.ToString(), numberPosition, runeButtons[i].drawColor, 0f, Main.gameFont.MeasureString(Main.currentPlayer.gravityRuneUsesLeft.ToString()) / 2f, 0.5f, SpriteEffects.None, 0f);
                }
            }
        }
    }
}
