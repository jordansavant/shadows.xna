using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TDGameLibrary.Input
{
    public interface IButtonBind : IDeviceButton
    {
        List<IDeviceButton> Buttons { get; }
    }
}
