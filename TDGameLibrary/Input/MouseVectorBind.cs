using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace TDGameLibrary.Input
{
    public sealed class MouseVectorBind : IDirectionalDevice
    {
        private MouseState CurrentState { get; set; }
        private MouseState PreviousState { get; set; }
        private Vector2 CurrentPosition;
        private Vector2 PreviousPosition;
        private Func<Vector2> GetComparisonPointFunction { get; set; }
        private Vector2 CurrentComparisonPoint { get; set; }
        private Vector2 PreviousComparisonPoint { get; set; }
        private int DeadZoneRadius { get; set; }

        public MouseVectorBind(Func<Vector2> getComparisonPoint, int deadZoneRadius)
        {
            CurrentState = Mouse.GetState();
            PreviousState = CurrentState;

            PreviousPosition = new Vector2();
            PreviousPosition.X = PreviousState.X;
            PreviousPosition.Y = PreviousState.Y;
            CurrentPosition = new Vector2();
            CurrentPosition.X = CurrentState.X;
            CurrentPosition.Y = CurrentState.Y;

            GetComparisonPointFunction = getComparisonPoint;
            DeadZoneRadius = deadZoneRadius;
        }

        public bool IsNeutral
        {
            get
            {
                return Vector2.Distance(CurrentComparisonPoint, CurrentPosition) < DeadZoneRadius;
            }
        }

        public Vector2 GetCurrentVector()
        {
            return CurrentPosition - CurrentComparisonPoint;
        }

        public Vector2 GetLastVector()
        {
            return PreviousPosition - PreviousComparisonPoint;
        }

        public void Update()
        {
            PreviousState = CurrentState;
            CurrentState = Mouse.GetState();

            PreviousPosition.X = PreviousState.X;
            PreviousPosition.Y = PreviousState.Y;
            
            CurrentPosition.X = CurrentState.X;
            CurrentPosition.Y = CurrentState.Y;

            if (GetComparisonPointFunction != null)
            {
                PreviousComparisonPoint = CurrentComparisonPoint;
                CurrentComparisonPoint = GetComparisonPointFunction.Invoke();
            }
            else
            {
                PreviousComparisonPoint = Vector2.Zero;
                CurrentComparisonPoint = Vector2.Zero;
            }

        }
    }
}
