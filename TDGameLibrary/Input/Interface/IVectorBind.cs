using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TDGameLibrary.Input
{
    public interface IVectorBind : IDirectionalDevice
    {
        List<IDirectionalDevice> DirectionalDevices { get; }
    }
}
