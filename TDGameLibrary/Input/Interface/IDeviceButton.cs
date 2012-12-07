using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TDGameLibrary.Input
{
    public interface IDeviceButton : IStepDrivenInput
    {
        bool IsDown { get; }
        bool IsPressed { get; }
        bool IsReleased { get; }
    }
}
