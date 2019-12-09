using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AFunnyNamespace
{
    class Peasant : Blank
    {
        public Peasant(string name)
        {
            Name = name;
        }

        protected override void Tick()
        {
            base.Tick();
            if (secondsRemaining < 35)
            {
                headingForExit = true;
            }
        }

        public override void ReachedDestination()
        {
            if (secondsRemaining < 30)
            {
                destination = exit;

                if (pathBuffer.Count != 0)
                {
                    SetDestination(PointMaker(pathBuffer.Peek()));
                    pathBuffer.Dequeue();
                }
                else
                {
                    pathBuffer = pathFinding.Star(grid.NodeRetriever(currentPosition), grid.NodeRetriever(destination), grid);
                }
            }
            else
            {
                if (!grid.NodeRetriever(currentPosition).walkable)
                {
                    SetDestination(PointMaker(lastValidLocation));
                    ReevaluatePath();
                }
                else if (pathBuffer.Count != 0)
                {
                    SetDestination(PointMaker(pathBuffer.Peek()));
                    pathBuffer.Dequeue();
                }
                else
                {
                    SetDestination(destination);
                    FindDestination();
                    pathBuffer = pathFinding.Star(grid.NodeRetriever(currentPosition), grid.NodeRetriever(destination), grid);
                }
            }
        }

        public override void Fight(string location)
        {
                SetDestination(new Point(currentPosition.X + (currentPosition.X - PointFrom(location).X) * 4, currentPosition.Y + (currentPosition.Y - PointFrom(location).Y) * 4));
        }

        public override void Bump(string location)
        {
            SetDestination(new Point(currentPosition.X + (currentPosition.X - PointFrom(location).X) * 4, currentPosition.Y + (currentPosition.Y - PointFrom(location).Y) * 4));
            ReevaluatePath();
        }

        public override void Critter(Point location, string strength, string status)
        {
            //This one doesn't care
        }
    }
}
