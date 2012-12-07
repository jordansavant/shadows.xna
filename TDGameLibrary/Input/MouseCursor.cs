using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TDGameLibrary.Input
{
    public class MouseCursor : ICursorSensor
    {
        public MouseCursor()
        {
            MouseState = LastMouseState = Mouse.GetState();
            
            _screenPositionRectangle = new Rectangle(MouseState.X, MouseState.Y, 1, 1);
            _screenPosition = new Vector2(MouseState.X, MouseState.Y);
        }

        public MouseState LastMouseState { get; private set; }
        public MouseState MouseState { get; private set; }

        protected Rectangle _screenPositionRectangle;
        protected Vector2 _screenPosition;

        public virtual void Update()
        {
            LastMouseState = MouseState;
            MouseState = Mouse.GetState();
            
            _screenPositionRectangle.X = MouseState.X;
            _screenPosition.X = MouseState.X;
            _screenPositionRectangle.Y = MouseState.Y;
            _screenPosition.Y = MouseState.Y;
        }

        public virtual bool IsCursorHovering(Rectangle rectangle)
        {
            return rectangle.Intersects(_screenPositionRectangle);
        }

        public virtual bool IsCursorHovering(Vector2 position, float radius)
        {
            float distance = Vector2.Distance(position, _screenPosition);
            return (distance < radius);
        }

        public virtual bool IsCursorMoving()
        {
            return LastMouseState != MouseState;
        }

        public virtual Vector2 ScreenPosition
        {
            get
            {
                return _screenPosition;
            }
        }

        public virtual Rectangle ScreenPositionRectangle
        {
            get
            {
                return _screenPositionRectangle;
            }
        }

    }
}
