using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AFunnyNamespace
{
    public class Grid
    {
        public Node[,] nodes;

        public Grid()
        {
            nodes = new Node[81, 61];

            for (int x = 0; x < 81; x++)
            {
                for (int y = 0; y < 61; y++)
                {
                    nodes[x, y] = new Node(true, x, y);
                }
            }
        }

        public List<Node> GetNeighbours(Node node)
        {
            List<Node> neighbours = new List<Node>();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                        continue;

                    int checkX = node.x + x;
                    int checkY = node.y + y;

                    if (checkX >= 0 && checkX <= 80 && checkY >= 0 && checkY <= 60)
                    {
                        neighbours.Add(nodes[checkX, checkY]);
                    }
                }
            }

            return neighbours;
        }

        public Node NodeRetriever(Point coordinate)
        {
            int x = (int)Math.Round(coordinate.X / 13.75);
            int y = (int)Math.Round(coordinate.Y / 13.75);

            return nodes[x, y];
        }
    }
}
