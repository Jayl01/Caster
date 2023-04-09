using AnotherLib;
using AnotherLib.Input;
using AnotherLib.UI;
using AnotherLib.UI.UIElements;
using AnotherLib.Utilities;
using Caster.Effects;
using Caster.Entities.Enemies;
using Caster.Entities.Players;
using Caster.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Caster.UI
{
    public class TitleScreen : UIObject
    {
        public Text titleText;
        public TextButton playButton;
        public TextButton controlsButton;
        public TextButton quitButton;

        private Text controlsText;
        private Texture2D controlsPanel;
        private bool showingControlsPanel;

        private const float ButtonSpacing = 30f;

        public static TitleScreen NewTitleScreen()
        {
            TitleScreen titleScreen = new TitleScreen();
            titleScreen.Initialize();
            return titleScreen;
        }

        public override void Initialize()
        {
            Main.camera = new Camera(Vector2.Zero, new Vector2(GameScreen.resolutionWidth, GameScreen.resolutionHeight), Camera.ControlMode.Mouse);
            Main.camera.SetToUICamera();
            Main.camera.cameraOrigin = new Vector2(GameScreen.resolutionWidth, GameScreen.resolutionHeight) / 2f;

            titleText = new Text(Main.gameFont, "CASTER", (GameScreen.center / 3f) - new Vector2(0f, GameScreen.halfScreenHeight - 5), Color.White, 2f, true);
            titleText.position.Y += titleText.size.Y;

            playButton = new TextButton(Main.gameFont, "PLAY", (GameScreen.center / 3f) + new Vector2(-12f, -ButtonSpacing), 0.6f, 0.75f, Color.White, Color.Green, true);
            controlsButton = new TextButton(Main.gameFont, "CONTROLS", (GameScreen.center / 3f) - new Vector2(20f, 0f), 0.5f, 0.6f, Color.White, Color.Orange, true);
            quitButton = new TextButton(Main.gameFont, "QUIT", (GameScreen.center / 3f) + new Vector2(-12f, ButtonSpacing), 0.5f, 0.6f, Color.White, Color.Red, true);

            string controlsMessage =
                "           CONTROLS         \n" +
                "WASD: MOVEMENT              \n" +
                "LEFT-CLICK: CAST RUNE       \n" +
                "RIGHT-CLICK: TELEPORT       \n" +
                "SCROLL: CHANGE ATTACK       \n" +
                "                            \n" +
                "OBJECTIVE: TRAVEL 1600 TILES\n" +
                "CAUSE AS MUCH DAMAGE AS     \n" +
                "POSSIBLE!                   \n" +
                "                            \n" +
                "   *PRESS ESCAPE TO EXIT*   \n";
            controlsText = new Text(Main.gameFont, controlsMessage, GameScreen.center / 3f, Color.White, centerOrigin: true);
            controlsPanel = TextureGenerator.CreatePanelTexture((int)controlsText.size.X + 4, (int)controlsText.size.Y + 4, 2, Color.Black, Color.White);
            ReInitializePositions();
        }

        public override void ReInitializePositions()
        {
            titleText.position = (GameScreen.center / 3f) - new Vector2(0f, GameScreen.halfScreenHeight / 3f - 5);
            titleText.position.Y += titleText.size.Y;

            playButton.buttonPosition = (GameScreen.center / 3f) + new Vector2(-12f, -ButtonSpacing);
            controlsButton.buttonPosition = (GameScreen.center / 3f) - new Vector2(20f, 0f);
            quitButton.buttonPosition = (GameScreen.center / 3f) + new Vector2(-12f, ButtonSpacing);

            controlsText.position = (GameScreen.center / 3f);
        }

        public override void Update()
        {
            playButton.Update();
            controlsButton.Update();
            quitButton.Update();
            if (playButton.buttonPressed)
            {
                Main.ResetGameStats();
                Main.currentPlayer = new Player();
                Main.currentPlayer.Initialize();
                Main.activePlayers.Add(Main.currentPlayer);
                Main.camera.SetToPlayerCamera();
                Main.camera.UpdateCameraView();
                Main.enemySpawner = new EnemySpawner();
                WorldClass.GenerateWorld(1600, 80, 16, 2, 6);
                Main.gameState = Main.GameState.InGame;
            }
            if (controlsButton.buttonPressed)
            {
                showingControlsPanel = !showingControlsPanel;
            }
            if (InputManager.IsKeyJustPressed(Microsoft.Xna.Framework.Input.Keys.Escape))
                showingControlsPanel = false;
            if (quitButton.buttonPressed)
            {
                Main.ExitGame();
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Smoke.smokePixelTextures[0], GameScreen.ScreenRectangle, Color.Black);

            titleText.Draw(spriteBatch);
            playButton.Draw(spriteBatch);
            controlsButton.Draw(spriteBatch);
            quitButton.Draw(spriteBatch);

            if (showingControlsPanel)
            {
                spriteBatch.Draw(controlsPanel, (GameScreen.center / 3f) - (controlsText.size / 2f) - (Vector2.One * 2f), Color.White);
                controlsText.Draw(spriteBatch);
            }
        }
    }
}
