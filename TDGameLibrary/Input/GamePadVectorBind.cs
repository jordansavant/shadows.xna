using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace TDGameLibrary.Input
{
    public sealed class GamePadVectorBind : IDirectionalDevice
    {
        private GamePadState CurrentState { get; set; }
        private GamePadState PreviousState { get; set; }
        private XboxThumbsticks Thumbstick { get; set; }

        public GamePadVectorBind(XboxThumbsticks thumbstick)
        {
            CurrentState = GamePad.GetState(GameEnvironment.PlayerIndex);
            PreviousState = CurrentState;
            Thumbstick = thumbstick;
        }

        public bool IsNeutral
        {
            get
            {
                if (Thumbstick == XboxThumbsticks.Left)
                {
                    return CurrentState.ThumbSticks.Left == Vector2.Zero;
                }

                return CurrentState.ThumbSticks.Right == Vector2.Zero;
            }
        }

        public Vector2 GetCurrentVector()
        {
            if (Thumbstick == XboxThumbsticks.Left)
            {
                return new Vector2(CurrentState.ThumbSticks.Left.X, 0 - CurrentState.ThumbSticks.Left.Y);
            }

            return new Vector2(CurrentState.ThumbSticks.Right.X, 0 - CurrentState.ThumbSticks.Right.Y);
        }

        public Vector2 GetLastVector()
        {
            if (Thumbstick == XboxThumbsticks.Left)
            {
                return new Vector2(PreviousState.ThumbSticks.Left.X, 0 - PreviousState.ThumbSticks.Left.Y);
            }

            return new Vector2(PreviousState.ThumbSticks.Right.X, 0 - PreviousState.ThumbSticks.Right.Y);
        }

        public void Update()
        {
            PreviousState = CurrentState;
            CurrentState = GamePad.GetState(GameEnvironment.PlayerIndex);
        }
    }
}
