using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TDGameLibrary.Ai.Pathfinding
{
    public class Node
    {

        public Node(int x, int y)
        {
            FCost = 0;
            GCost = 0;
            HCost = 0;
            X = x;
            Y = y;
            IsBlocked = false;
        }


        // Node Details
        public int X { get; set; }
        public int Y { get; set; }

        // A Star details
        public bool IsBlocked { get; set; }
        public int FCost { get; set; }
        public int GCost { get; set; }
        public int HCost { get; set; }
        public Node AstarParent { get; set; }


        public bool Equals(Node compareNode)
        {
            return (X == compareNode.X && Y == compareNode.Y);
        }


        public void ResetAstar()
        {
            this.GCost = 0;
            this.HCost = 0;
            this.FCost = 0;
            this.AstarParent = null;
        }

        public Vector2 ToVector2()
        {
            return new Vector2(X, Y);
        }
    }
}
