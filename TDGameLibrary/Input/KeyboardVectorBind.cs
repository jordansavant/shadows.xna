using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace TDGameLibrary.Input
{
    public sealed class KeyboardVectorBind : IDirectionalDevice
    {
        private KeyboardState CurrentState { get; set; }
        private KeyboardState PreviousState { get; set; }
        private Keys Left { get; set; }
        private Keys Right { get; set; }
        private Keys Up { get; set; }
        private Keys Down { get; set; }

        public KeyboardVectorBind(Keys keyLeft, Keys keyRight, Keys keyUp, Keys keyDown)
        {
            CurrentState = Keyboard.GetState();
            PreviousState = CurrentState;
            Left = keyLeft;
            Right = keyRight;
            Up = keyUp;
            Down = keyDown;
        }

        public bool IsNeutral
        {
            get
            {
                return !CurrentState.IsKeyDown(Left) && !CurrentState.IsKeyDown(Right) && !CurrentState.IsKeyDown(Up) && !CurrentState.IsKeyDown(Down);
            }
        }

        public Vector2 GetCurrentVector()
        {
            Vector2 direction = Vector2.Zero;

            if (CurrentState.IsKeyDown(Left))
            {
                direction.X += -1;
            }
            if (CurrentState.IsKeyDown(Right))
            {
                direction.X += 1;
            }
            if (CurrentState.IsKeyDown(Up))
            {
                direction.Y += -1;
            }
            if (CurrentState.IsKeyDown(Down))
            {
                direction.Y += 1;
            }

            return direction;
        }

        public Vector2 GetLastVector()
        {
            Vector2 direction = Vector2.Zero;

            if (PreviousState.IsKeyDown(Left))
            {
                direction.X += -1;
            }
            if (PreviousState.IsKeyDown(Right))
            {
                direction.X += 1;
            }
            if (PreviousState.IsKeyDown(Up))
            {
                direction.Y += -1;
            }
            if (PreviousState.IsKeyDown(Down))
            {
                direction.Y += 1;
            }

            return direction;
        }

        public void Update()
        {
            PreviousState = CurrentState;
            CurrentState = Keyboard.GetState();
        }
    }
}
