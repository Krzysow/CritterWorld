using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AFunnyNamespace
{
    public class Node
    {
        public enum Direction
        {
            None, North, NorthEast, East, SouthEast, South, SouthWest, West, NorthWest
        }

        public bool walkable;
        public int x;
        public int y;

        public int gCost;
        public int hCost;
        public Node parent;
        public Direction direction;
        

        public Node(bool _Walkable, int _X, int _Y)
        {
            walkable = _Walkable;
            x = _X;
            y = _Y;
        }

        public int fCost
        {
            get
            {
                return gCost + hCost;
            }
        }
    }
}
