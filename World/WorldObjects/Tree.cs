using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Caster.World.WorldObjects
{
    public class Tree : WorldObject
    {
        public static Texture2D[] treeTextures;

        private int treeType;
        private readonly Vector2 TreeOrigin = new Vector2(24, 65);

        public static Tree NewTree(Vector2 position, int treeType)
        {
            Tree tree = new Tree();
            tree.position = position;
            tree.treeType = treeType;
            return tree;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(treeTextures[treeType], position, null, Color.LightGray, 0f, TreeOrigin, 1f, SpriteEffects.None, 0f);
        }
    }
}
