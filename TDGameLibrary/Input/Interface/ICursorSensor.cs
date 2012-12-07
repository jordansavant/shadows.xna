using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TDGameLibrary.Input
{
    public interface ICursorSensor : IStepDrivenInput
    {
        bool IsCursorHovering(Rectangle rectangle);
        bool IsCursorHovering(Vector2 position, float radius);
        bool IsCursorMoving();
        
        Vector2 ScreenPosition { get; }
        Rectangle ScreenPositionRectangle { get; }
    }
}
