using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace TDGameLibrary.Input
{
    public sealed class KeyboardDeviceButton : IDeviceButton
    {
        private KeyboardState CurrentState { get; set; }
        private KeyboardState PreviousState { get; set; }
        private Keys Button { get; set; }

        public KeyboardDeviceButton(Keys button)
        {
            CurrentState = Keyboard.GetState();
            PreviousState = CurrentState;
            Button = button;
        }

        public bool IsDown
        {
            get 
            {
                return CurrentState.IsKeyDown(Button);
            }
        }

        public bool IsPressed
        {
            get
            {
                return CurrentState.IsKeyDown(Button) && PreviousState.IsKeyUp(Button);
            }
        }

        public bool IsReleased
        {
            get
            {
                return CurrentState.IsKeyUp(Button) && PreviousState.IsKeyDown(Button);
            }
        }

        public void Update()
        {
            PreviousState = CurrentState;
            CurrentState = Keyboard.GetState();
        }

    }
}
