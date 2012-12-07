using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TDGameLibrary.Utility;
using TDGameLibrary.Ui.Modifiers;

namespace TDGameLibrary.Ui
{
    /**
     * UiControlBase is responsible for defining the properties of any ui control
     */
    public abstract class UiControl
    {
        public UiControl()
        {
            Color = Color.White;
            IsEnabled = true;
            IsVisible = true;
            DisplayRectangle = new Rectangle();
            Opacity = 1f;
            Scale = 1f;
            Rotation = 0f;
            Origin = Vector2.Zero;
            SpriteEffects = SpriteEffects.None;
            Modifiers = new List<UiControlModifier>();
        }

        public List<UiControlModifier> Modifiers;
        public event EventHandler OnSelect;
        public string Name;
        protected string text;
        public virtual string Text
        {
            get { return text; }
            set { text = value; if(SpriteFont != null) Size = SpriteFont.MeasureString(value); }
        }
        protected Vector2 size;
        public Vector2 Size
        {
            get { return size; }
            set { size = value; UpdateRectangle(); }
        }
        public Rectangle DisplayRectangle;
        protected Vector2 position;
        public Vector2 Position
        {
            get { return position; }
            set
            {
                position = value;
                UpdateRectangle();
            }
        }
        public object Value;
        public bool IsInFocus;
        public bool IsEnabled;
        public bool IsVisible;
        public bool IsSelectable;
        public SpriteFont SpriteFont;
        public Color Color;
        public string Type;
        public float Opacity;
        public float Scale;
        public SpriteEffects SpriteEffects;
        public Vector2 Origin;
        public float Rotation;

        public Vector2 TextSize
        {
            get
            {
                return SpriteFont.MeasureString(Text);
            }
        }

        private void UpdateRectangle()
        {
            if (position != null)   // If position has changed, update rectangle location
            {
                DisplayRectangle.X = (int)position.X;
                DisplayRectangle.Y = (int)position.Y;
            }

            if (Text != null)       // If this is a text control and the text has changed, update the rectangle width dynamically
            {
                DisplayRectangle.Width = (int)SpriteFont.MeasureString(Text).X;
                DisplayRectangle.Height = (int)SpriteFont.MeasureString(Text).Y;
            }
        }


        public virtual void Update(GameTime gameTime)
        {
            foreach (UiControlModifier modifier in Modifiers)
            {
                modifier.Update(gameTime);
            }
        }

        
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (IsVisible && TDGameLibrary.GameEnvironment.IsDebugModeActive)
            {
                DrawUtility.DrawRectangle(DisplayRectangle, Color.PaleTurquoise);
            }
        }


        protected virtual void OnSelected(EventArgs e)
        {
            if (OnSelect != null)
            {
                OnSelect(this, e);
            }
        }


        public virtual void HandleInput()
        {
            if (!IsInFocus)
            {
                return;
            }

            if (IsSelected())
            {
                OnSelected(null);
            }
        }


        protected virtual bool IsSelected()
        {
            return false;
        }

        public bool UseAbsolutePositioning;
    }
}
