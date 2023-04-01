using AnotherLib;
using AnotherLib.Collision;
using AnotherLib.Drawing;
using AnotherLib.Input;
using AnotherLib.UI;
using AnotherLib.Utilities;
using Caster.Effects;
using Caster.Entities.Enemies;
using Caster.Entities.Players;
using Caster.Entities.Projectiles;
using Caster.UI;
using Caster.Utilities;
using Caster.World;
using Caster.World.WorldObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using static AnotherLib.Drawing.SpriteBatchData;

namespace Caster
{
    public class Main : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch spriteBatch;
        public static GameScreen gameScreen;
        public static Camera camera;
        public static Matrix screenMatrix;
        public static SpriteFont gameFont;
        public static Player currentPlayer;
        public static Random random;
        private static bool closeGame;

        private static RenderTarget2D backgroundTarget;
        private static RenderTarget2D gameTarget;

        //SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, camera.cameraMatrix
        public static readonly SpriteBatchParameterData gameSpriteBatchData = new SpriteBatchParameterData()
        {
            sortMode = SpriteSortMode.Deferred,
            blendState = BlendState.AlphaBlend,
            samplerState = SamplerState.PointClamp,
            stencilState = DepthStencilState.None,
            rasterizerState = RasterizerState.CullCounterClockwise,
            effect = null
        };

        public static readonly SpriteBatchParameterData uiSpriteBatchData = new SpriteBatchParameterData()
        {
            sortMode = SpriteSortMode.Deferred,
            blendState = BlendState.AlphaBlend,
            samplerState = SamplerState.PointClamp,
            stencilState = DepthStencilState.None,
            rasterizerState = RasterizerState.CullCounterClockwise,
            effect = null
        };

        public static GameState gameState = GameState.Title;
        public static UIObject activeUI;
        public static List<CollisionBody> activeEnemies = new List<CollisionBody>();      //These are collisionBodies for easier collision detection
        public static List<Projectile> activeProjectiles = new List<Projectile>();
        public static List<VisualEffect> activeBackgroundEffects = new List<VisualEffect>();
        public static List<VisualEffect> activeForegroundEffects = new List<VisualEffect>();
        public static List<UIObject> uiList = new List<UIObject>();
        public static List<UIObject> activeWorldUI = new List<UIObject>();
        public static List<CollisionBody> activePlayers = new List<CollisionBody>();        //Just for easy scanning
        public static EnemySpawner enemySpawner;
        public static bool GameWon = false;

        public enum GameState
        {
            Title,
            InGame,
            GameOver
        }

        public Main()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            GameData.DebugMode = true;
            GameData.MusicVolume = 0.4f;
            GameData.SoundEffectVolume = 0.6f;
            spriteBatch = new SpriteBatch(GraphicsDevice);
            gameScreen = new GameScreen(Window, _graphics, (int)(1280 / 1.2f), (int)(1080 / 1.2f));
            backgroundTarget = new RenderTarget2D(GraphicsDevice, GameScreen.resolutionWidth, GameScreen.resolutionHeight);
            gameTarget = new RenderTarget2D(GraphicsDevice, GameScreen.resolutionWidth, GameScreen.resolutionHeight);
            screenMatrix = Matrix.CreateScale(3f, 3f, 1f);
            camera = new Camera(Vector2.Zero, new Vector2(GameScreen.resolutionWidth, GameScreen.resolutionHeight), Camera.ControlMode.Mouse);
            camera.SetToUICamera();
            camera.cameraOrigin = new Vector2(GameScreen.resolutionWidth, GameScreen.resolutionHeight) / 2f;
            GameData.mouseScreenDivision = new Vector2(3f);
            GameData.random = random = new Random();
            AssetLoader.LoadAssets(Content);
            GameData.CurrentGameDrawInfo = new GameDrawInfo();
            GameData.CurrentGameDrawInfo.spriteBatchData = gameSpriteBatchData;
            GameData.CurrentGameDrawInfo.matrix = camera.cameraMatrix;
            InputManager.currentKeyboardState = Keyboard.GetState();
            InputManager.currentMouseState = Mouse.GetState();
            InputManager.currentGamepadState = GamePad.GetState(PlayerIndex.One);
            ShaderBatch.InitializeShaderBatchLists();
            ShaderBatch.spriteBatch = spriteBatch;
            Window.ClientSizeChanged += ScreenResized;
            activeUI = TitleScreen.NewTitleScreen();
            SoundPlayer.activeSoundEffects = new List<TrackedSoundEffectInstance>();
        }

        protected override void Update(GameTime gameCaster)
        {
            if (closeGame)
            {
                Exit();
                return;
            }

            GameData.CurrentGameDrawInfo.matrix = camera.cameraMatrix;
            InputManager.UpdateControlsState();

            if (gameState == GameState.Title)
            {
                activeUI.Update();
            }
            else if (gameState == GameState.InGame)
            {
                if (currentPlayer.position.X > WorldClass.CurrentWorldWidth * 16)
                {
                    GameWon = true;
                    gameState = GameState.GameOver;
                    activeUI = GameOverScreen.NewGameOverScreen();
                    GameMusicPlayer.StopMusic();
                    return;
                }
                if (currentPlayer.position.Y > WorldClass.CurrentWorldHeight * 16)
                {
                    gameState = GameState.GameOver;
                    activeUI = GameOverScreen.NewGameOverScreen("MMMMMAYBE DON'T TRY THAT NEXT TIME? YOU DO YOU THOUGH.");
                    GameMusicPlayer.StopMusic();
                    return;
                }

                enemySpawner.Update();
                GameMusicPlayer.Update();
                WorldClass.Update();
                VisualEffect[] activeBackgroundEffectsCopy = activeBackgroundEffects.ToArray();
                VisualEffect[] activeForegroundEffectsCopy = activeForegroundEffects.ToArray();
                CollisionBody[] activeProjectilesCopy = activeProjectiles.ToArray();
                CollisionBody[] activeEnemiesCopy = activeEnemies.ToArray();

                foreach (VisualEffect effect in activeBackgroundEffectsCopy)
                    effect.Update();
                foreach (CollisionBody enemy in activeEnemiesCopy)
                    enemy.Update();
                currentPlayer.Update();
                foreach (CollisionBody projectile in activeProjectilesCopy)
                    projectile.Update();
                foreach (WorldObject worldObject in WorldClass.activeWorldObjects)
                    worldObject.Update();
                foreach (VisualEffect effect in activeForegroundEffectsCopy)
                    effect.Update();
            }
            else if (gameState == GameState.GameOver)
            {
                activeUI.Update();
            }

            camera.Update();
        }

        protected override void Draw(GameTime gameCaster)
        {
            GraphicsDevice.SetRenderTarget(backgroundTarget);
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin(gameSpriteBatchData);
            GameData.CurrentGameDrawInfo.matrix = Matrix.Identity;

            if (gameState == GameState.InGame)
                WorldClass.DrawDetails(spriteBatch);

            spriteBatch.End();
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Black);


            //Draw out the game
            GraphicsDevice.SetRenderTarget(gameTarget);
            GraphicsDevice.Clear(Color.Transparent);

            spriteBatch.Begin(gameSpriteBatchData, camera.cameraMatrix);
            GameData.CurrentGameDrawInfo.matrix = camera.cameraMatrix;

            if (gameState == GameState.Title)
            {
                activeUI.Draw(spriteBatch);
            }
            else if (gameState == GameState.InGame)
            {
                WorldClass.Draw(spriteBatch);

                VisualEffect[] activeBackgroundEffectsCopy = activeBackgroundEffects.ToArray();
                VisualEffect[] activeForegroundEffectsCopy = activeForegroundEffects.ToArray();
                CollisionBody[] activeProjectilesCopy = activeProjectiles.ToArray();
                CollisionBody[] activeEnemiesCopy = activeEnemies.ToArray();

                foreach (VisualEffect effect in activeBackgroundEffectsCopy)
                    effect.Draw(spriteBatch);
                foreach (CollisionBody enemy in activeEnemiesCopy)
                    enemy.Draw(spriteBatch);
                currentPlayer.Draw(spriteBatch);
                foreach (CollisionBody projectile in activeProjectilesCopy)
                    projectile.Draw(spriteBatch);
                foreach (VisualEffect effect in activeForegroundEffectsCopy)
                    effect.Draw(spriteBatch);
            }
            else if (gameState == GameState.GameOver)
            {
                activeUI.Draw(spriteBatch);
            }

            spriteBatch.End();
            ShaderBatch.DrawQueuedShaderDraws();
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, null);
            spriteBatch.Draw(backgroundTarget, Vector2.Zero, Color.White);
            spriteBatch.Draw(gameTarget, Vector2.Zero, Color.White);
            spriteBatch.End();

            Shaders.DrawAllScreenEffectShaders(backgroundTarget);
            Shaders.DrawAllScreenEffectShaders(gameTarget);

            //Draws World UI with no lighting
            spriteBatch.Begin(uiSpriteBatchData, camera.cameraMatrix);

            UIObject[] activeWorldUICopy = activeWorldUI.ToArray();
            foreach (UIObject uiObject in activeWorldUICopy)
                uiObject.Draw(spriteBatch);

            spriteBatch.End();

            //Draws the UI
            spriteBatch.Begin(uiSpriteBatchData, screenMatrix);

            UIObject[] activeUICopy = uiList.ToArray();
            foreach (UIObject uiObject in activeUICopy)
                uiObject.Draw(spriteBatch);

            DebugTools.DrawDebugText(spriteBatch, gameFont);

            spriteBatch.End();
        }

        public static void ResetGameStats()
        {
            activeEnemies = new List<CollisionBody>();
            activeProjectiles = new List<Projectile>();
            activeBackgroundEffects = new List<VisualEffect>();
            activeForegroundEffects = new List<VisualEffect>();
            uiList = new List<UIObject>();
            activeWorldUI = new List<UIObject>();
            activePlayers = new List<CollisionBody>();
            GameWon = false;
            Enemy.EnemiesKilled = 0;
        }

        public static void ResetActiveData()
        {
            int[] keys = WorldClass.activeWorldData.staticWorldObjects.Keys.ToArray();
            for (int i = 0; i < WorldClass.activeWorldData.staticWorldObjects.Count; i++)
            {
                WorldClass.activeWorldData.staticWorldObjects[keys[i]].ClearRemainingData();
                WorldClass.activeWorldData.staticWorldObjects.Remove(keys[i]);
            }

            for (int i = 0; i < WorldClass.activeWorldObjects.Count; i++)
            {
                WorldClass.activeWorldObjects[i].ClearRemainingData();
                WorldClass.activeWorldObjects.RemoveAt(i);
            }
        }

        public static void ExitGame()
        {
            closeGame = true;
        }

        private void ScreenResized(object sender, EventArgs e)
        {
            backgroundTarget = new RenderTarget2D(GraphicsDevice, GameScreen.resolutionWidth, GameScreen.resolutionHeight);
            gameTarget = new RenderTarget2D(GraphicsDevice, GameScreen.resolutionWidth, GameScreen.resolutionHeight);
            if (gameState == GameState.InGame)
                camera.SetToPlayerCamera();
            else
            {
                camera.SetToUICamera();
                activeUI.ReInitializePositions();
            }
            camera.cameraOrigin = new Vector2(GameScreen.resolutionWidth, GameScreen.resolutionHeight) / 2f;
        }
    }
}