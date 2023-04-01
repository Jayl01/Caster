using AnotherLib.Utilities;
using Caster.Effects;
using Caster.Entities.Players;
using Caster.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Caster.Utilities
{
    public class AssetLoader : ContentLoader
    {
        private static ContentLoader contentLoader;
        private static AssetLoader assetLoader;

        public AssetLoader(ContentManager content) : base(content)
        {
            contentManager = content;
        }

        public static void LoadAssets(ContentManager content)
        {
            assetLoader = new AssetLoader(content);
            Main.gameFont = assetLoader.LoadFont("MainFont");
            assetLoader.LoadTextures();
            assetLoader.LoadSounds();
            Shaders.gradientEffect = content.Load<Effect>("Effects/Gradient");
        }

        private void LoadTextures()
        {
            Tile.tileTextures = new Dictionary<Tile.TileType, Texture2D>
            {
                { Tile.TileType.Dirt, LoadTex("Tiles/Dirt") },
                { Tile.TileType.Grass, LoadTex("Tiles/Grass") },
                { Tile.TileType.LeftGrass, LoadTex("Tiles/LeftGrass") },
                { Tile.TileType.RightGrass, LoadTex("Tiles/RightGrass") },
                { Tile.TileType.DirtToUndergroundDirt, LoadTex("Tiles/DirtToUndergroundDirt") },
                { Tile.TileType.UndergroundDirt, LoadTex("Tiles/UndergroundDirt") }
            };

            //WorldDetails.moonTexture = LoadTex("Environment/Moon");

            Player.playerWalkSpritesheet = LoadTex("Player/Player_Walk");
            Player.playerJumpFrame = LoadTex("Player/Player_Jump");
            Player.playerFallSpritesheet = LoadTex("Player/Player_Fall");
            Player.playerArmTexture = LoadTex("Player/Player_Arm");

            Smoke.smokePixelTextures = new Texture2D[1];
            Smoke.smokePixelTextures[Smoke.WhitePixelTexture] = TextureGenerator.CreatePanelTexture(2, 2, 1, Color.White, Color.White, false);
        }

        private void LoadSounds()
        {
            /*SoundPlayer.sounds = new SoundEffect[20];
            SoundPlayer.sounds[Sounds.Step_1] = LoadSFX("SFX/Step_1");
            SoundPlayer.sounds[Sounds.Step_2] = LoadSFX("SFX/Step_2");
            SoundPlayer.sounds[Sounds.Step_3] = LoadSFX("SFX/Step_3");
            SoundPlayer.sounds[Sounds.PlayerJump] = LoadSFX("SFX/PlayerJump");
            SoundPlayer.sounds[Sounds.PlayerShoot] = LoadSFX("SFX/PlayerShoot");
            SoundPlayer.sounds[Sounds.Goon_Hurt1] = LoadSFX("SFX/Goon_Hurt_1");
            SoundPlayer.sounds[Sounds.Goon_Hurt2] = LoadSFX("SFX/Goon_Hurt_2");
            SoundPlayer.sounds[Sounds.GameEnd] = LoadSFX("SFX/DeathSound");
            SoundPlayer.sounds[Sounds.GoonShootCharge] = LoadSFX("SFX/ShotCharge_1");
            SoundPlayer.sounds[Sounds.TurretShootCharge] = LoadSFX("SFX/ShotCharge_2");
            SoundPlayer.sounds[Sounds.TurretHurt] = LoadSFX("SFX/Turret_Hit");
            SoundPlayer.sounds[Sounds.WrongDirectionSound] = LoadSFX("SFX/WrongDirectionSound");
            SoundPlayer.sounds[Sounds.TimeOutSound] = LoadSFX("SFX/TimeOutSound");
            SoundPlayer.sounds[Sounds.TurretExplosionSound] = LoadSFX("SFX/TurretExplosion");
            SoundPlayer.sounds[Sounds.PlayerDash] = LoadSFX("SFX/DashSound");
            SoundPlayer.sounds[Sounds.AttemptHit] = LoadSFX("SFX/AttemptHitSound");
            SoundPlayer.sounds[Sounds.DirectionalSound] = LoadSFX("SFX/DirectionalAppear");
            SoundPlayer.sounds[Sounds.One] = LoadSFX("SFX/One");
            SoundPlayer.sounds[Sounds.Three] = LoadSFX("SFX/Three");
            SoundPlayer.sounds[Sounds.Four] = LoadSFX("SFX/Four");

            GameMusicPlayer.theme1 = LoadSFX("Music/Theme_1").CreateInstance();
            GameMusicPlayer.theme2 = LoadSFX("Music/Theme_2").CreateInstance();
            BoomBox.boomBoxSong = LoadSFX("Music/RadioTheme");*/
        }
    }
}
