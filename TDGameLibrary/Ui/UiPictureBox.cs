using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TDGameLibrary.Ui
{
    public class UiPictureBox : UiControl
    {
        public UiPictureBox(Texture2D image, Rectangle displayRectangle, Rectangle sourceRectangle)
            : this(image, displayRectangle)
        {
            SourceRectangle = sourceRectangle;
        }

        public UiPictureBox(Texture2D image, Rectangle displayRectangle)
        {
            Image = image;
            DisplayRectangle = displayRectangle;
            Color = Color.White;
            SourceRectangle = new Rectangle(0, 0, image.Width, image.Height);
        }

        public UiPictureBox(Texture2D image)
        {
            Image = image;
            Color = Color.White;
            DisplayRectangle = SourceRectangle = new Rectangle(0, 0, image.Width, image.Height);
            Size = new Vector2(DisplayRectangle.Width, DisplayRectangle.Height);
        }

        private Texture2D image;
        public Texture2D Image
        {
            get
            {
                return image;
            }
            set
            {
                if(value != null)
                    SourceRectangle = new Rectangle(0, 0, value.Width, value.Height);
                image = value;
            }
        }
        public Rectangle SourceRectangle { get; set; }


        public override void Draw(SpriteBatch spriteBatch)
        {
            if (IsVisible && Image != null)
            {
                spriteBatch.Draw(Image, DisplayRectangle, SourceRectangle, Color.ToOpacity(Opacity));
            }

            base.Draw(spriteBatch);
        }


        public void SetPosition(Vector2 newPosition)
        {
            DisplayRectangle = new Rectangle(
                (int)newPosition.X,
                (int)newPosition.Y,
                SourceRectangle.Width,
                SourceRectangle.Height);
        }
    }
}
