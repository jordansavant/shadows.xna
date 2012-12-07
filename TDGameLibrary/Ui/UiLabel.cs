using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TDGameLibrary.Ui
{
    public class UiLabel : UiControl
    {
        public UiLabel(string text, string font, Color color)
        {
            IsSelectable = false;
            Text = text;
            SpriteFont = GameEnvironment.Game.Content.Load<SpriteFont>(font);
            Size = SpriteFont.MeasureString(Text);
            Color = color;
        }

        public float MaxWidth = 0;
        private string[] Words;
        private string WrappedText = "";
        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;

                // Word wrapping
                if (MaxWidth > 0 && Size.X > MaxWidth) // throw \r\n into space around that area;
                {
                    WrappedText = "";
                    Words = Text.Split(' ');
                    foreach (string word in Words)
                    {
                        if (SpriteFont.MeasureString(WrappedText + " " + word).X <= MaxWidth)
                        {
                            WrappedText += " " + word;
                        }
                        else
                        {
                            WrappedText += "\r\n" + word;
                        }
                    }

                    Text = WrappedText.Trim();
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (IsVisible)
            {
                spriteBatch.DrawString(SpriteFont, Text, Position, Color.ToOpacity(Opacity), Rotation, Origin, Scale, SpriteEffects, 0f);
            }

            base.Draw(spriteBatch);
        }
    }
}
