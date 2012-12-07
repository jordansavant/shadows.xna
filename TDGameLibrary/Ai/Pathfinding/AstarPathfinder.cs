using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TDGameLibrary.Utility;
// THIS IS A BIG CHANGE!!!
namespace TDGameLibrary.Ai.Pathfinding
{
    public class AstarPathfinder : IPathfinder
    {
        public AstarPathfinder(NodeMap nodeMap)
        {
            NodeMap = nodeMap;
        }

        public NodeMap NodeMap { get; set; }
        private List<Node> OpenList { get; set; }
        private List<Node> ClosedList { get; set; }
        private Stack<Vector2> ShortestPath { get; set; }

        private Stack<Node> Astar(Node startNode, Node endNode)
        {
            OpenList = new List<Node>();
            ClosedList = new List<Node>();
            Stack<Node> path = new Stack<Node>(); // the shortest path

            // add start node to open list
            Node currentNode = startNode;
            ClosedList.Add(currentNode);


            while (!currentNode.Equals(endNode))
            {
                // Get the open list for the current node.
                // A single node has 8 surrounding nodes, or less depending on edge nodes.
                for (int i = 0; i < 8; i++)
                {
                    int x = 0;
                    int y = 0;
                    int xindex = 0;
                    int yindex = 0;

                    switch (i)
                    {
                        case 0: // left
                            x = currentNode.X - NodeMap.Seperation;
                            y = currentNode.Y;
                            break;
                        case 1: // top left
                            x = currentNode.X - NodeMap.Seperation;
                            y = currentNode.Y - NodeMap.Seperation;
                            break;
                        case 2: // top
                            x = currentNode.X;
                            y = currentNode.Y - NodeMap.Seperation;
                            break;
                        case 3: // top right
                            x = currentNode.X + NodeMap.Seperation;
                            y = currentNode.Y - NodeMap.Seperation;
                            break;
                        case 4: // right
                            x = currentNode.X + NodeMap.Seperation;
                            y = currentNode.Y;
                            break;
                        case 5: // bottom right
                            x = currentNode.X + NodeMap.Seperation;
                            y = currentNode.Y + NodeMap.Seperation;
                            break;
                        case 6: // bottom
                            x = currentNode.X;
                            y = currentNode.Y + NodeMap.Seperation;
                            break;
                        case 7: // bottom left
                            x = currentNode.X - NodeMap.Seperation;
                            y = currentNode.Y + NodeMap.Seperation;
                            break;
                    }
                    // Node map is from 0,0 indexed nodes upward, clamp x and y between zero and the node map size.
                    x = (int)MathHelper.Clamp(x, 0f, NodeMap.Width);
                    y = (int)MathHelper.Clamp(y, 0f, NodeMap.Height);

                    // Convert the x and y to the index of the node in the list
                    xindex = x / NodeMap.Seperation;
                    yindex = y / NodeMap.Seperation;

                    Node cNode = NodeMap.Map[xindex][yindex];

                    int xdiff;
                    int ydiff;
                    int gcost;
                    int hcost;

                    if (cNode.GCost == 0) // gcost
                    {
                        // this is the difference between the current node x and y and this node x and y
                        xdiff = Math.Abs(cNode.X - currentNode.X);
                        ydiff = Math.Abs(cNode.Y - currentNode.Y);
                        gcost = 0;
                        if (ydiff > 0 && xdiff > 0)
                            gcost = (int)((double)(xdiff + ydiff) / 1.4); // 1.4 is rough diagonal length of a square
                        else
                            gcost = xdiff + ydiff; // length of one side

                        cNode.GCost = gcost;
                    }
                    if (cNode.HCost == 0) // h cost
                    {
                        // this is the difference between the oNode and the destination node
                        xdiff = cNode.X - endNode.X;
                        ydiff = cNode.Y - endNode.Y;
                        hcost = (int)Math.Sqrt((double)(Math.Pow(xdiff, 2) + Math.Pow(ydiff, 2)));
                        cNode.HCost = hcost;
                    }
                    if (cNode.FCost == 0)
                    {
                        // this is the gcost and the hcost combined
                        cNode.FCost = (cNode.GCost + cNode.HCost);
                    }


                    // if the connected node is not within an obstacle
                    if (!cNode.IsBlocked)
                    {
                        // if the connected node is not in the closed list
                        if (!ClosedList.Contains(cNode))
                        {

                            // if the connected node is not in the open list
                            if (!OpenList.Contains(cNode))
                            {


                                cNode.AstarParent = currentNode;
                                OpenList.Add(cNode); // add it to the open list
                            }
                            else
                            { // if it is already in the open list

                                // check to see if its current gcost is less than the new gcost of the parent and the old gcost
                                gcost = cNode.GCost + currentNode.GCost;
                                if (gcost < cNode.GCost)
                                {
                                    // if so, make the current node its new parent and recalculate the gcost, and fcost
                                    cNode.AstarParent = currentNode;
                                    cNode.GCost = gcost;
                                    cNode.FCost = (cNode.GCost + cNode.HCost);
                                }
                            }
                        } // end closed list check
                    } // end blocked nodes
                }

                // at this point the open list has been updated to reflect new parents and costs

                // loop through the open list
                Node cheapOpenNode = null;
                for (int i = 0; i < OpenList.Count; i++)
                {
                    // compare the openList nodes for the lowest F Cost
                    Node oNode = OpenList[i];
                    if (cheapOpenNode == null) // initialize our cheapest open node
                    {
                        cheapOpenNode = oNode;
                        continue;
                    }

                    if (oNode.FCost < cheapOpenNode.FCost)
                    {
                        // we found a cheaper open list node
                        cheapOpenNode = oNode;
                    }
                }

                if (cheapOpenNode == null) // we have run out of options, no shortest path
                {
                    return null;
                }

                // now we have the node from the open list that has the cheapest f cost
                // move it to the closed list and set it as the current node
                ClosedList.Add(cheapOpenNode);
                OpenList.Remove(cheapOpenNode);
                currentNode = cheapOpenNode;

            } // A* Complete

            // we have found the end node
            // Loop from the current/end node moving back through the parents until we reach the start node
            // add those to the list and we have our path
            bool moreParents = true;
            Node workingNode = currentNode;
            while (moreParents)
            {
                if (workingNode.AstarParent == null)
                {
                    path.Push(workingNode);
                    moreParents = false;
                }
                else
                {
                    path.Push(workingNode);
                    workingNode = workingNode.AstarParent;
                }
            }

            // before we end, reset all our costs and parents in our nodes
            ResetNodes();

            // reverse so we can have starting node going to end node
            //Collections.reverse(path);

            return path;
        }

        private void ResetNodes()
        {
            for (int i = 0; i < NodeMap.Map.Count; i++)
            {
                for (int j = 0; j < NodeMap.Map[i].Count; j++)
                {
                    NodeMap.Map[i][j].ResetAstar();
                }
            }
        }

        public Stack<Vector2> GetShortestPath(Vector2 start, Vector2 finish)
        {
            if (NodeMap == null)
                NodeMap = new Pathfinding.NodeMap(1, 1, 1); //TODO: remove these two lines. Need them right now to stop a null ref exception since NodeMap is null

            // Translate start and finish to non-blocked nodes from node map
            Node closestStartNode = NodeMap.TranslateToNode(start, false);
            Node closestFinishNode = NodeMap.TranslateToNode(finish, false);

            if (closestStartNode == null || closestFinishNode == null) // return null if there is no path
                return null;

            Stack<Node> shortestNodePath = Astar(closestStartNode, closestFinishNode);
            

            if (shortestNodePath == null) // there is no path, its blocked completely
                return null;

            List<Node> shortestNodePathArray = shortestNodePath.ToList<Node>();
            Stack<Vector2> path = new Stack<Vector2>();
            for (int i = shortestNodePathArray.Count - 1; i >= 0 ; i--)
            {
                path.Push(shortestNodePathArray[i].ToVector2());
            }

            //DrawDebug.DrawPoints(path.ToList()); // Draw shortest path for debug

            return path;
        }
    }
}
