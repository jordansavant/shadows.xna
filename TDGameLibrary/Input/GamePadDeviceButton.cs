using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace TDGameLibrary.Input
{
    public sealed class GamePadDeviceButton : IDeviceButton
    {
        private GamePadState CurrentState { get; set; }
        private GamePadState PreviousState { get; set; }
        private Buttons Button { get; set; }

        public GamePadDeviceButton(Buttons button)
        {
            CurrentState = GamePad.GetState(GameEnvironment.PlayerIndex);
            PreviousState = CurrentState;
            Button = button;
        }

        public bool IsDown
        {
            get
            {
                return CurrentState.IsButtonDown(Button);
            }
        }

        public bool IsPressed
        {
            get
            {
                return CurrentState.IsButtonDown(Button) && PreviousState.IsButtonUp(Button);
            }
        }

        public bool IsReleased
        {
            get
            {
                return CurrentState.IsButtonUp(Button) && PreviousState.IsButtonDown(Button);
            }
        }

        public void Update()
        {
            PreviousState = CurrentState;
            CurrentState = GamePad.GetState(GameEnvironment.PlayerIndex);
        }

    }
}
