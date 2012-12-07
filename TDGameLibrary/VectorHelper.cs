using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TDGameLibrary
{
    public static class VectorHelper
    {
        public static void Zero(ref Vector2 vector2)
        {
            vector2.X = 0;
            vector2.Y = 0;
        }

        public static void Assign(ref Vector2 assignee, ref Vector2 assigner)
        {
            assignee.X = assigner.X;
            assignee.Y = assigner.Y;
        }

        public static void AssignAddition(ref Vector2 assignee, ref Vector2 assignerA, ref Vector2 assignerB)
        {
            assignee.X = assignerA.X + assignerB.X;
            assignee.Y = assignerA.Y + assignerB.Y;
        }

        public static void AssignSubtraction(ref Vector2 assignee, ref Vector2 assignerA, ref Vector2 assignerB)
        {
            assignee.X = assignerA.X - assignerB.X;
            assignee.Y = assignerA.Y - assignerB.Y;
        }

        public static void AssignAdditionScale(ref Vector2 assignee, ref Vector2 assignerA, ref Vector2 assignerB, float scale)
        {
            assignee.X = assignerA.X + assignerB.X * scale;
            assignee.Y = assignerA.Y + assignerB.Y * scale;
        }

        public static void AssignSubtractionScale(ref Vector2 assignee, ref Vector2 assignerA, ref Vector2 assignerB, float scale)
        {
            assignee.X = assignerA.X - assignerB.X * scale;
            assignee.Y = assignerA.Y - assignerB.Y * scale;
        }

        public static void AssignScale(ref Vector2 assignee, ref Vector2 assigner, float scale)
        {
            assignee.X = assigner.X + scale;
            assignee.Y = assigner.Y + scale;
        }
    }
}
