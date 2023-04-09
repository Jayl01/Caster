using AnotherLib;
using AnotherLib.UI;
using AnotherLib.UI.UIElements;
using AnotherLib.Utilities;
using Caster.Effects;
using Caster.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Caster.UI
{
    public class GameOverScreen : UIObject
    {
        public Text endGameText;
        private int endGameTimer = 0;
        private string customMessage = string.Empty;

        private readonly string[] WinMessages = new string[3] {
            "NICE ONE, NOW TRY THE SECRET DEMONIC MODE.",
            "YOUR REFLEXES ARE IMPRESSIVE!\nNOW LET'S SEE HOW YOU DO WITHOUT FINGERS.",
            "YOU CAN NOW PERFORM YOUR VICTORY DANCE AND SHARE YOUR WIN WITH THE ONE PERSON YOU KNOW!" };
        private readonly string[] LoseMessages = new string[3] {
            "REALLY? WE'RE STILL IN EASY MODE...",
            "WORK ON THOSE REACTION TIMES, YOU BARELY EVEN HIT THE LAST ONE!",
            "RETRY RETRY RETRY RETRY RETRY RETRY RETRY RETRY RETRY RETRY RETRY RETRY RETRY RETRY RETRY RETRY RETRY RETRY RETRY RETRY RETRY RETRY" };

        public static GameOverScreen NewGameOverScreen()
        {
            GameOverScreen gameOverScreen = new GameOverScreen();
            gameOverScreen.Initialize();
            return gameOverScreen;
        }

        public static GameOverScreen NewGameOverScreen(string customMessage)
        {
            GameOverScreen gameOverScreen = new GameOverScreen();
            gameOverScreen.customMessage = customMessage;
            gameOverScreen.Initialize();
            return gameOverScreen;
        }

        public override void Initialize()
        {
            string text = string.Empty;
            if (customMessage == string.Empty)
            {
                text = LoseMessages[Main.random.Next(0, LoseMessages.Length)];
                if (Main.GameWon)
                    text = WinMessages[Main.random.Next(0, WinMessages.Length)];
            }
            else
            {
                text = customMessage;
            }


            endGameText = new Text(Main.gameFont, text, GameScreen.center / 3f, Color.White, centerOrigin: true);
            //SoundPlayer.PlayLocalSound(Sounds.GameEnd);
            Main.uiList.Clear();

            Main.camera.SetToUICamera();
            ReInitializePositions();
            Main.camera.cameraOrigin = new Vector2(GameScreen.resolutionWidth, GameScreen.resolutionHeight) / 2f;
        }

        public override void ReInitializePositions()
        {
            endGameText.position = GameScreen.center / 3f;
        }

        public override void Update()
        {
            endGameTimer++;
            if (endGameTimer >= 10 * 60)
            {
                Main.gameState = Main.GameState.Title;
                endGameTimer = 0;
                Main.ResetActiveData();
                Main.ResetGameStats();
                Main.activeUI = TitleScreen.NewTitleScreen();
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Smoke.smokePixelTextures[0], GameScreen.ScreenRectangle, Color.Black);

            endGameText.Draw(spriteBatch);
        }
    }
}
