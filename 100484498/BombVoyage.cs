using CritterController;
using System;
using System.Drawing;
using System.IO;
using System.Collections.Generic;

namespace AFunnyNamespace
{
    public class BombVoyage : Blank
    {
        float angle = 0.0f;
        bool lure = false;
        bool catAndMouse = false;

        public BombVoyage(string name)
        {
            Name = name;
        }

        protected override void Tick()
        {
            base.Tick();
            if (secondsRemaining < 35)
            {
                lure = false;
                catAndMouse = false;
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
            else if (lure)
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
                    lure = false;
                    catAndMouse = true;
                }
            }
            else if (catAndMouse)
            {
                SetDestination(BombCircler());
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
            if (bombs.Count != 0)
            {
                lure = true;
                destination = BombCircler();
                pathBuffer = pathFinding.Star(grid.NodeRetriever(currentPosition), grid.NodeRetriever(destination), grid);
            }
            else
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
            //This one doesn't care
        }

        public Point BombCircler()
        {
            Point circleCenter;
            float radius = 50;

            Point onTheCircle = Point.Empty;

            circleCenter = FindNearestItem(bombs);

            onTheCircle = CirclePoint(radius, angle, circleCenter);

            if (onTheCircle.X < 0)
            {
                onTheCircle.X = 0;

                if (onTheCircle.Y < 0)
                {
                    onTheCircle.Y = 0;
                }
                else if (onTheCircle.Y > 1099)
                {
                    onTheCircle.Y = 1099;
                }
            }
            else if (onTheCircle.X > 1099)
            {
                onTheCircle.X = 1099;

                if (onTheCircle.Y < 0)
                {
                    onTheCircle.Y = 0;
                }
                else if (onTheCircle.Y > 1099)
                {
                    onTheCircle.Y = 1099;
                }
            }

            if (angle < 360)
            {
                angle += 9f;
            }
            else
            {
                angle = 0;
            }

            return onTheCircle;
        }

        public Point CirclePoint(float radius, float angle, Point circleCenter)
        {
            int x = (int)(radius * Math.Cos(angle * Math.PI / 180f)) + circleCenter.X;
            int y = (int)(radius * Math.Sin(angle * Math.PI / 180f)) + circleCenter.Y;

            return new Point(x, y);
        }
    }
}
