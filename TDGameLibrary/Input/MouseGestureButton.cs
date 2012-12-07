using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace TDGameLibrary.Input
{
    public sealed class MouseGestureButton : IDeviceButton
    {
        private MouseState CurrentState { get; set; }
        private MouseState PreviousState { get; set; }
        private MouseGesture Button { get; set; }

        public MouseGestureButton(MouseGesture button)
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
                    case MouseGesture.Left:
                        return CurrentState.X < PreviousState.X;
                    case MouseGesture.Up:
                        return CurrentState.Y < PreviousState.Y;
                    case MouseGesture.Right:
                        return CurrentState.X > PreviousState.X;
                    case MouseGesture.Down:
                        return CurrentState.Y > PreviousState.Y;
                    default: return false;
                }
            }
        }

        public bool IsPressed
        {
            get
            {
                return IsDown;
            }
        }

        public bool IsReleased
        {
            get
            {
                return CurrentState.X == PreviousState.X && CurrentState.Y == PreviousState.Y;
            }
        }

        public void Update()
        {
            PreviousState = CurrentState;
            CurrentState = Mouse.GetState();
        }

    }
}
