using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace TDGameLibrary.Input
{
    public interface IButtonSensor : IStepDrivenInput
    {
        bool IsButtonDown(Enum button);
        bool IsButtonPressed(Enum button);
        bool IsButtonReleased(Enum button);
        void BindButton(Enum button, Enum deviceButton);
    }
}
