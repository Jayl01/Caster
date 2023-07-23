using AnotherLib.Utilities;
using Caster.Effects;
using Caster.Entities.Enemies;
using Caster.Entities.Players;
using Caster.Entities.Projectiles;
using Caster.UI;
using Caster.World;
using Caster.World.WorldObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
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

            WorldDetails.moonTexture = LoadTex("Environment/Moon");
            WorldDetails.cloudSpritesheet = LoadTex("Environment/Clouds");
            Tree.treeTextures = new Texture2D[3];
            Tree.treeTextures[0] = LoadTex("Environment/Tree_1");
            Tree.treeTextures[1] = LoadTex("Environment/Tree_2");
            Tree.treeTextures[2] = LoadTex("Environment/Tree_3");

            Player.playerWalkSpritesheet = LoadTex("Player/Player_Walk");
            Player.playerJumpFrame = LoadTex("Player/Player_Jump");
            Player.playerFallSpritesheet = LoadTex("Player/Player_Fall");
            Player.playerArmTexture = LoadTex("Player/Player_Arm");

            Gore.goreTextures = new Texture2D[2];
            Gore.goreTextures[Gore.Caster_1] = LoadTex("Effects/Gore_1");
            Gore.goreTextures[Gore.Caster_2] = LoadTex("Effects/Gore_2");
            EvilCaster.evilCasterWalkSpritesheet = LoadTex("Enemies/EvilCaster_Walk");
            EvilCaster.evilCasterAttackSpritesheet = LoadTex("Enemies/EvilCaster_Attack");
            MatterCaster.matterCasterWalkSpritesheet = LoadTex("Enemies/MatterCaster_Walk");
            MatterCaster.matterCasterAttackSpritesheet = LoadTex("Enemies/MatterCaster_Attack");

            ElectricRune.baseRuneTexture = LoadTex("Projectiles/BaseRune");
            ElectricRune.baseRuneInnerCircleTexture = LoadTex("Projectiles/BaseRuneInnerCircle");
            ElectricRune.runeSymbolTexture = LoadTex("Projectiles/ElectricRuneSymbol");
            GravityRune.runeSymbolTexture = LoadTex("Projectiles/GravityRuneSymbol");
            FireballRune.runeSymbolTexture = LoadTex("Projectiles/FireRuneSymbol");
            Fireball.fireballTexture = LoadTex("Projectiles/Fireball");

            BombRune.runeSymbolTexture = LoadTex("Projectiles/ChainRuneSymbol");
            LaserRune.runeSymbolTexture = LoadTex("Projectiles/LaserRuneSymbol");
            LaserRune.laserSpritesheet = LoadTex("Projectiles/Laser");
            SpiritWall.spiritWallTexture = LoadTex("Projectiles/SpiritWall");
            SpiritBomb.spiritBombTexture = LoadTex("Projectiles/SpiritBomb");

            PlayerUI.runeSymbolTextures = new Texture2D[3];
            PlayerUI.runeSymbolTextures[Player.Rune_Electric] = LoadTex("UI/ElectricRuneSymbol");
            PlayerUI.runeSymbolTextures[Player.Rune_Fire] = LoadTex("UI/FireRuneSymbol");
            PlayerUI.runeSymbolTextures[Player.Rune_Gravity] = LoadTex("UI/GravityRuneSymbol");
            PlayerUI.heartSymbol = LoadTex("UI/Heart");
            PlayerUI.playerIndicatorTexture = LoadTex("UI/PlayerIndicator");

            Smoke.smokePixelTextures = new Texture2D[1];
            Smoke.smokePixelTextures[Smoke.WhitePixelTexture] = TextureGenerator.CreatePanelTexture(2, 2, 1, Color.White, Color.White, false);

        }

        private void LoadSounds()
        {
            GameMusicPlayer.mainTheme = LoadSFX("Music/Caster_Theme").CreateInstance();

            SoundPlayer.sounds = new SoundEffect[16];
            SoundPlayer.sounds[Sounds.Step_1] = LoadSFX("Ambience/Step_1");
            SoundPlayer.sounds[Sounds.Step_2] = LoadSFX("Ambience/Step_2");
            SoundPlayer.sounds[Sounds.Step_3] = LoadSFX("Ambience/Step_3");
            SoundPlayer.sounds[Sounds.ElectricRune_Active] = LoadSFX("Projectiles/ElectricRune_Active");
            SoundPlayer.sounds[Sounds.ElectricRune_Shock] = LoadSFX("Projectiles/ElectricRune_Shock");
            SoundPlayer.sounds[Sounds.Jump] = LoadSFX("Ambience/Jump");
            SoundPlayer.sounds[Sounds.Jump_Land] = LoadSFX("Ambience/Jump_Land");
            SoundPlayer.sounds[Sounds.Caster_Chant] = LoadSFX("Enemies/Caster_Chant");
            SoundPlayer.sounds[Sounds.FireballRune_Active] = LoadSFX("Projectiles/FireballRune_Active");
            SoundPlayer.sounds[Sounds.FireballRune_Shoot] = LoadSFX("Projectiles/FireballRune_Shoot");
            SoundPlayer.sounds[Sounds.Fireball_Land] = LoadSFX("Projectiles/Fireball_Land");
            SoundPlayer.sounds[Sounds.GravityRune_Fling] = LoadSFX("Projectiles/GravityRune_Fling");
            SoundPlayer.sounds[Sounds.Teleport] = LoadSFX("Ambience/Teleport");
            SoundPlayer.sounds[Sounds.SpiritBomb_Explosion] = LoadSFX("Projectiles/SpiritBombExplosion");
            SoundPlayer.sounds[Sounds.Caster_Hurt] = LoadSFX("Enemies/Enemy_Hurt");
        }
    }
}
