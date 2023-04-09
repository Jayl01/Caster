using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caster.World.WorldObjects.Destructibles
{
    public class ExplosiveBarrel : DestroyableWorldObject
    {
        public static Texture2D alarmTowerTexture;

        private readonly Vector2 Origin = new Vector2(8, 112);

        public static void NewExplosiveBarrel(Vector2 position)
        {
            ExplosiveBarrel explosiveBarrel = new ExplosiveBarrel();
            explosiveBarrel.position = position;

        }

        public override void Update()
        {
            UpdatePhysics();

        }

        public override void DestructionEffects(Vector2 hitPosition, int seed)
        {
            
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(alarmTowerTexture, position, null, Color.LightGray, 0f, Origin, 1f, SpriteEffects.None, 0f);
        }
    }
}
