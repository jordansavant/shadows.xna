using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TDGameLibrary.Ai.Pathfinding
{
    public interface IPathfinder
    {
        Stack<Vector2> GetShortestPath(Vector2 start, Vector2 finish);
    }
}
