using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace TDGameLibrary.Input
{
    public sealed class MouseDeviceButton : IDeviceButton
    {
        private MouseState CurrentState { get; set; }
        private MouseState PreviousState { get; set; }
        private MouseButton Button { get; set; }

        public MouseDeviceButton(MouseButton button)
        {
            CurrentState = Mouse.GetState();
            PreviousState = CurrentState;
            Button = button;
        }

        public bool IsDown
        {
            get 
            {
                switch (Button)
                {
                    case MouseButton.Left:
                        return CurrentState.LeftButton == ButtonState.Pressed;
                    case MouseButton.Right:
                        return CurrentState.RightButton == ButtonState.Pressed;
                    case MouseButton.Middle:
                        return CurrentState.MiddleButton == ButtonState.Pressed;
                    default: return false;
                }
            }
        }

        public bool IsPressed
        {
            get
            {
                switch (Button)
                {
                    case MouseButton.Left:
                        return CurrentState.LeftButton == ButtonState.Pressed && PreviousState.LeftButton == ButtonState.Released;
                    case MouseButton.Right:
                        return CurrentState.RightButton == ButtonState.Pressed && PreviousState.RightButton == ButtonState.Released;
                    case MouseButton.Middle:
                        return CurrentState.MiddleButton == ButtonState.Pressed && PreviousState.MiddleButton == ButtonState.Released;
                    default: return false;
                }
            }
        }

        public bool IsReleased
        {
            get
            {
                switch (Button)
                {
                    case MouseButton.Left:
                        return CurrentState.LeftButton == ButtonState.Released && PreviousState.LeftButton == ButtonState.Pressed;
                    case MouseButton.Right:
                        return CurrentState.RightButton == ButtonState.Released && PreviousState.RightButton == ButtonState.Pressed;
                    case MouseButton.Middle:
                        return CurrentState.MiddleButton == ButtonState.Released && PreviousState.MiddleButton == ButtonState.Pressed;
                    default: return false;
                }
            }
        }

        public void Update()
        {
            PreviousState = CurrentState;
            CurrentState = Mouse.GetState();
        }

    }
}
