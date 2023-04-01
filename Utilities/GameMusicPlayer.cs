using AnotherLib.Utilities;
using Microsoft.Xna.Framework.Audio;
using System;

namespace Caster.Utilities
{
    public class GameMusicPlayer
    {
        public static SoundEffectInstance theme1;
        public static SoundEffectInstance theme2;

        public static void Update()
        {
        }

        public static void StopMusic()
        {
            theme1.Stop();
            theme2.Stop();
        }
    }
}
