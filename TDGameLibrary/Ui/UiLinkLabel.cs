using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TDGameLibrary.Utility;

namespace TDGameLibrary.Ui
{
    public abstract class UiLinkLabel : UiLabel
    {
        public UiLinkLabel(string text, string font, Color color, Color selectedColor, EventHandler onSelect)
            : this(text, font, color, selectedColor)
        {
            OnSelect += onSelect;
        }

        public UiLinkLabel(string text, string font, Color color, Color selectedColor)
            : base(text, font, color)
        {
            IsSelectable = true;
            IsInFocus = false;
            Position = Vector2.Zero;
            SelectedColor = selectedColor;
        }


        public Color SelectedColor { get; set; }


        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (IsVisible)
            {
                if (IsInFocus)
                {
                    spriteBatch.DrawString(SpriteFont, Text, Position, new Color(SelectedColor.R, SelectedColor.G, SelectedColor.B, Opacity));
                }
                else
                {
                    spriteBatch.DrawString(SpriteFont, Text, Position, new Color(Color.R, Color.G, Color.B, Opacity));
                }
            }
        }
    }
}
