using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TDGameLibrary.Input
{
    public interface IDirectionalDevice : IStepDrivenInput
    {
        bool IsNeutral { get; }

        Vector2 GetCurrentVector();
        Vector2 GetLastVector();
    }
}
