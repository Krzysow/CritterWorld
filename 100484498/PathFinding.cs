using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;

namespace AFunnyNamespace
{
    public class PathFinding
    {
        object gridLock = new object();

        public Queue<Node> Star(Node start, Node end, Grid grid)
        {
            lock (gridLock)
            {
                List<Node> openSet = new List<Node>();
                HashSet<Node> closedSet = new HashSet<Node>();
                openSet.Add(end);
                
                while (openSet.Count > 0)
                {
                    Node node = openSet[0];
                    for (int i = 1; i < openSet.Count; i++)
                    {
                        if (openSet[i].fCost <= node.fCost && openSet[i].hCost < node.hCost)
                        {
                            node = openSet[i];
                        }
                    }

                    openSet.Remove(node);
                    closedSet.Add(node);

                    if (node == start)
                    {
                        return RetracePath(start, end);
                    }

                    foreach (Node neighbour in grid.GetNeighbours(node))
                    {
                        if (!neighbour.walkable || closedSet.Contains(neighbour))
                        {
                            continue;
                        }

                        int newCostToNeighbour = node.gCost + CalculateCost(node, neighbour);
                        if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                        {
                            neighbour.gCost = newCostToNeighbour;
                            neighbour.hCost = CalculateCost(neighbour, end);
                            neighbour.parent = node;

                            if (NodeDiractionX(neighbour) == 0)
                            {
                                if (NodeDiractionY(neighbour) == 1)
                                {
                                    neighbour.direction = Node.Direction.North;
                                }
                                else
                                {
                                    neighbour.direction = Node.Direction.South;
                                }
                            }
                            else if (NodeDiractionX(neighbour) == 1)
                            {
                                if (NodeDiractionY(neighbour) == 1)
                                {
                                    neighbour.direction = Node.Direction.NorthEast;
                                }
                                else if (NodeDiractionY(neighbour) == 0)
                                {
                                    neighbour.direction = Node.Direction.East;
                                }
                                else
                                {
                                    neighbour.direction = Node.Direction.SouthEast;
                                }
                            }
                            else if (NodeDiractionX(neighbour) == -1)
                            {
                                if (NodeDiractionY(neighbour) == -1)
                                {
                                    neighbour.direction = Node.Direction.SouthWest;
                                }
                                else if (NodeDiractionY(neighbour) == 0)
                                {
                                    neighbour.direction = Node.Direction.West;

                                }
                                else
                                {
                                    neighbour.direction = Node.Direction.NorthWest;
                                }
                            }
                            else
                            {
                                neighbour.direction = Node.Direction.None;
                            }


                            if (!openSet.Contains(neighbour))
                            {
                                openSet.Add(neighbour);
                            }
                        }
                    }
                }
                return new Queue<Node>();
            }
        }

        static int NodeDiractionX(Node neighbour)
        {
            return neighbour.x - neighbour.parent.x;
        }

        static int NodeDiractionY(Node neighbour)
        {
            return neighbour.y - neighbour.parent.y;
        }

        static int CalculateCost(Node nodeA, Node nodeB)
        {
            int dstX = Math.Abs(nodeA.x - nodeB.x);
            int dstY = Math.Abs(nodeA.y - nodeB.y);

            if (dstX > dstY)
            {
                return 19 * dstY + 13 * (dstX - dstY);
            }
            return 19 * dstX + 13 * (dstY - dstX);
        }
        
        static Queue<Node> RetracePath(Node startNode, Node endNode)
        {
            Queue<Node> path = new Queue<Node>();
            Node currentNode = startNode;

            while (currentNode != endNode)
            {
                if (currentNode.direction != currentNode.parent.direction)
                {
                    path.Enqueue(currentNode);
                }

                currentNode = currentNode.parent;
            }

            return path;
        }
    }
}