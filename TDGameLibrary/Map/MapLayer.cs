using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TDGameLibrary.Map
{
    public class MapLayer
    {
        public Texture2D Image { get; set; }
        public float Opacity { get; set; }
        private Rectangle DestinationRectangle { get; set; }

        public MapLayer()
        {
        }

        public MapLayer(Texture2D image)
        {
            Image = image;
            Opacity = 1f;
        }


        public virtual Rectangle StartingRectangle(Rectangle screenRectangle)
        {
            int mapW = Image.Width;
            int mapH = Image.Height;

            int offsetX = mapW - screenRectangle.Width;
            if (offsetX > 0)
                offsetX = offsetX / 2;
            offsetX = screenRectangle.X - offsetX;

            int offsetY = mapH - screenRectangle.Height;
            if (offsetY > 0)
                offsetY = offsetY / 2;
            offsetY = screenRectangle.Y - offsetY;

             return new Rectangle(offsetX, offsetY, mapW, mapH);

        }


        public virtual void Update(GameTime gameTime, Rectangle screenRectangle)
        {
            DestinationRectangle = StartingRectangle(screenRectangle);
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch, Rectangle screenRectangle)
        {
            spriteBatch.Draw(
                Image,
                DestinationRectangle,
                Color.White);
        }
    }
}
