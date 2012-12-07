using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FarseerTools
{
    public class FarseerSprite
    {
        public Vector2 Origin;
        public Texture2D Texture;

        public FarseerSprite(Texture2D texture, Vector2 origin)
        {
            this.Texture = texture;
            this.Origin = origin;
        }

        public FarseerSprite(Texture2D sprite)
        {
            Texture = sprite;
            Origin = new Vector2(sprite.Width / 2f, sprite.Height / 2f);
        }
    }
}