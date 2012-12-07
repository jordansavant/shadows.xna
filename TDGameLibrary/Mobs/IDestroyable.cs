using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TDGameLibrary.Mobs
{
    public interface IDestroyable
    {
        bool IsDestroyed { get; set; }
        event EventHandler OnDestroy;
        void Destruct();
    }
}
