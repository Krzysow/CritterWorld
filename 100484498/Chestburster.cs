using CritterController;
using System;
using System.Drawing;
using System.IO;
using System.Collections.Generic;

namespace AFunnyNamespace
{
    public class Chestburster : Blank
    {
        bool lurk = false;
        bool predator = false;

        public Chestburster(string name)
        {
            Name = name;
        }

        protected override void Tick()
        {
            base.Tick();
            if (secondsRemaining < 35)
            {
                lurk = false;
                predator = false;
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
            else if (!lurk && !predator)
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
            lurk = false;
            if (!predator)
            {
                SetDestination(new Point(currentPosition.X + (currentPosition.X - PointFrom(location).X) * 4, currentPosition.Y + (currentPosition.Y - PointFrom(location).Y) * 4));
            }
        }

        public override void Bump(string location)
        {
            SetDestination(new Point(currentPosition.X + (currentPosition.X - PointFrom(location).X) * 4, currentPosition.Y + (currentPosition.Y - PointFrom(location).Y) * 4));
            ReevaluatePath();
        }

        public override void Critter(Point location, string strength, string status)
        {
            if (!lurk && status == "Dead" && health > 70 && energy > 70 && !predator)
            {
                SetDestination(location);
                lurk = true;
            }

            if (((lurk && strength != "Strong") || predator) && status == "Alive")
            {
                predator = true;
                lurk = false;
                SetDestination(location);
                if (energy < 20 || health < 20)
                {
                    predator = false;
                }
            }
        }
    }
}
