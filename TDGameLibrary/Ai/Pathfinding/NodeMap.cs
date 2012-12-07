using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TDGameLibrary.Utility;
using System.Threading;
using FarseerPhysics.Dynamics;
using FarseerTools;

namespace TDGameLibrary.Ai.Pathfinding
{
    public class NodeMap // TODO: Extend List<List<Node>> and change Map to this
    {
        public NodeMap(int total_width, int total_height, int seperation)
        {
            Map = new List<List<Node>>();
            Width = total_width;
            Height = total_height;
            Seperation = Math.Max(seperation, 2);

            // construct 2 dimensional array of node coordinates
            for (int i = 0; i <= total_width; i = i + seperation) // X
            {
                List<Node> innermap = new List<Node>();
                for (int j = 0; j <= total_height; j = j + seperation)
                {
                    innermap.Add(new Node(i, j));
                }
                Map.Add(innermap);
            }


            // draw node map
            /*
            for (int i = 0; i < Map.Count; i++)
            {
                for (int j = 0; j < Map[i].Count; j++)
                {
                    if (i > 0)
                    {
                        if (j > 0)
                            Map[i][j].AddPointer(Map[i - 1][j - 1]);    // add node above left

                        Map[i][j].AddPointer(Map[i - 1][j]);            // add node left 

                        if (j < Map[i].Count - 1)
                            Map[i][j].AddPointer(Map[i - 1][j + 1]);    // add node below left
                    }

                    if (j > 0)
                        Map[i][j].AddPointer(Map[i][j - 1]);            // add node above

                    if (j < Map[i].Count - 1)
                        Map[i][j].AddPointer(Map[i][j + 1]);            // add node below

                    if (i < Map.Count - 1)
                    {
                        if (j > 0)
                            Map[i][j].AddPointer(Map[i + 1][j - 1]);    // add node above right

                        Map[i][j].AddPointer(Map[i + 1][j]);            // add node right 

                        if (j < Map[i].Count - 1)
                            Map[i][j].AddPointer(Map[i + 1][j + 1]);    // add node below right
                    }

                }
            }
            */
        }



        public List<List<Node>> Map;
        public int Seperation { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        private int MaxXIndex
        {
            get
            {
                return (Width / Seperation);
            }
        }
        private int MaxYIndex
        {
            get
            {
                return (Height / Seperation);
            }
        }



        public void ApplyObstacle(Rectangle obstacle)
        {
            // update the nodes accessibility information
            for (int i = 0; i < Map.Count; i++)
            {
                for (int j = 0; j < Map[i].Count; j++)
                {
                    Node node = Map[i][j];
                    if (obstacle.Contains(new Point(node.X, node.Y)))
                    {
                        node.IsBlocked = true;
                    }
                }
            }
        }

        // TODO THIS ALGORITHM IS SHIT! DO NOT USE UNTIL FIXED - J
        Vector2 _v = Vector2.Zero;
        public void ApplyObstacle(Body body)
        {
            foreach (Fixture f in body.FixtureList)
            {
                for (int i = 0; i < Map.Count; i++)
                {
                    for (int j = 0; j < Map[i].Count; j++)
                    {
                        Node node = Map[i][j];
                        _v = ConvertUnits.ToSimUnits(node.X, node.Y);
                        if(f.TestPoint(ref _v))
                        {
                            node.IsBlocked = true;
                        }
                    }
                }
            }
        }

        public void ApplyObstacle(int x, int y, int radius)
        {
            // update the nodes accessibility information
            for (int i = 0; i < Map.Count; i++)
            {
                for (int j = 0; j < Map[i].Count; j++)
                {
                    Node node = Map[i][j];
                    if(Math.Abs(node.X - x) <= radius || Math.Abs(node.Y - y) <= radius)
                    {
                        node.IsBlocked = true;
                    }
                }
            }
        }


        private int ConvertCoordinateToIndex(float x, bool isX)
        {
            return (int)MathHelper.Clamp((float)Math.Round((x / Seperation)), 0f, (float)( (isX) ? MaxXIndex : MaxYIndex));
        }

        public Node TranslateToNode(Vector2 vector2, bool isBlockedNodeAllowed)
        {
            // Look at each node and if the vector's X minus the nodes X in absolute value is less than or equal to half of seperation, choose node
            // else take current node as closest of all and continue, if closest node already exists, compare and update closest node

            int closestNodeXIndex = ConvertCoordinateToIndex(vector2.X, true);
            int closestNodeYIndex = ConvertCoordinateToIndex(vector2.Y, false);
            
            if (isBlockedNodeAllowed || !Map[closestNodeXIndex][closestNodeYIndex].IsBlocked)
            {
                return Map[closestNodeXIndex][closestNodeYIndex];
            }

            // If this node is blocked, we need to start spiraling outward until we find a node that is not blocked, staying in our index bounds
            int index_offset = 1; // bounds from center
            int origin_x_index = closestNodeXIndex;
            int origin_y_index = closestNodeYIndex;
            int nodes_checked = 0;

            while (true)
            {
                while ((closestNodeXIndex - origin_x_index) < index_offset)
                {
                    closestNodeXIndex++;

                    if (closestNodeXIndex > MaxXIndex)
                    {
                        closestNodeXIndex = MaxXIndex;
                        break;
                    }

                    nodes_checked++;
                    if (!Map[closestNodeXIndex][closestNodeYIndex].IsBlocked)
                        return Map[closestNodeXIndex][closestNodeYIndex];
                }

                while ((closestNodeYIndex - origin_y_index) < index_offset)
                {
                    closestNodeYIndex++;

                    if (closestNodeYIndex > MaxYIndex)
                    {
                        closestNodeYIndex = MaxYIndex;
                        break;
                    }

                    nodes_checked++;
                    if (!Map[closestNodeXIndex][closestNodeYIndex].IsBlocked)
                        return Map[closestNodeXIndex][closestNodeYIndex];
                }

                while ((origin_x_index - closestNodeXIndex) < index_offset)
                {
                    closestNodeXIndex--;

                    if (closestNodeXIndex < 0)
                    {
                        closestNodeXIndex = 0;
                        break;
                    }

                    nodes_checked++;
                    if (!Map[closestNodeXIndex][closestNodeYIndex].IsBlocked)
                        return Map[closestNodeXIndex][closestNodeYIndex];
                }

                while ((origin_y_index - closestNodeYIndex) < index_offset)
                {
                    closestNodeYIndex--;

                    if (closestNodeYIndex < 0)
                    {
                        closestNodeYIndex = 0;
                        break;
                    }

                    nodes_checked++;
                    if (!Map[closestNodeXIndex][closestNodeYIndex].IsBlocked)
                        return Map[closestNodeXIndex][closestNodeYIndex];
                }


                if (nodes_checked >= Map.Count) // we traversed all and found nothing.
                {
                    return null;
                }


                index_offset++;
            }
        }

    }
}
