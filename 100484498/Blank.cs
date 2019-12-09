using CritterController;
using System;
using System.Drawing;
using System.IO;
using System.Collections.Generic;

namespace AFunnyNamespace
{
    public abstract class Blank : ICritterController
    {
        protected Grid grid = new Grid();
        protected PathFinding pathFinding = new PathFinding();
        protected Queue<Node> pathBuffer = new Queue<Node>();
        protected Node lastValidLocation;
        protected Point destination;
        protected Point currentPosition = Point.Empty;
        protected List<Point> kiwis = new List<Point>();
        protected List<Point> gifts = new List<Point>();
        protected List<Point> bombs = new List<Point>();
        protected List<Point> walls = new List<Point>();
        protected Point exit = Point.Empty;
        protected System.Timers.Timer getInfoTimer;
        protected float energy = 100;
        protected float health = 100;
        protected string healthKeyWord = "Strong";
        protected int secondsRemaining = int.MaxValue;
        protected bool hungry = false;
        protected bool headingForExit = false;
        protected const int requestNumber = 4;

        public string Name { get; set; }

        public Send Responder { get; set; }

        public Send Logger { get; set; }

        public int speed { get; set; } = 10;

        public string Filepath { get; set; }

        private void LoadSettings()
        {
            string fileName = "CritterSpeedSettings.cfg";
            string fileSpec = Filepath + "/" + fileName;
            try
            {
                using (StreamReader reader = new StreamReader(fileSpec))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] lineParts = line.Split('=');
                        switch (lineParts[0])
                        {
                            case "EatSpeed":
                                speed = int.Parse(lineParts[1]);
                                break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log("Reading configuration " + fileSpec + " failed due to " + e);
            }
        }

        public void SaveSettings()
        {
            string fileName = "CritterSpeedSettings.cfg";
            string fileSpec = Filepath + "/" + fileName;
            try
            {
                using (StreamWriter writer = new StreamWriter(fileSpec, false))
                {
                    writer.WriteLine("Speed=" + speed);
                }
            }
            catch (Exception e)
            {
                Log("Writing configuration " + fileSpec + " failed due to " + e);
            }
        }

        protected void Log(string message)
        {
            if (Logger == null)
            {
                Console.WriteLine(message);
            }
            else
            {
                Logger(message);
            }
        }

        public void LaunchUI()
        {
            BlankSettings settings = new BlankSettings(this);
            settings.Show();
            settings.Focus();
        }

        protected static Point PointFrom(string coordinate)
        {
            string[] coordinateParts = coordinate.Substring(1, coordinate.Length - 2).Split(',');
            string rawX = coordinateParts[0].Substring(2);
            string rawY = coordinateParts[1].Substring(2);
            int x = int.Parse(rawX);
            int y = int.Parse(rawY);
            return new Point(x, y);
        }

        protected virtual void Tick()
        {
            Responder("SCAN:" + requestNumber);
            Responder("GET_LOCATION:" + requestNumber);
            Responder("GET_ENERGY:" + requestNumber);
            Responder("GET_LEVEL_TIME_REMAINING:" + requestNumber);
            Responder("GET_HEALTH:" + requestNumber);
        }

        public void Receive(string message)
        {
            Log("Message from body for " + Name + ": " + message);
            string[] msgParts = message.Split(':');
            string notification = msgParts[0];
            string location = msgParts[1];

            switch (notification)
            {
                case "LAUNCH":
                    Tick();
                    getInfoTimer = new System.Timers.Timer();
                    getInfoTimer.Interval = 20;
                    getInfoTimer.Elapsed += (obj, evt) => Tick();
                    getInfoTimer.Start();
                    Responder("SET_DESTINATION:" + 0 + ":" + 0 + ":" + speed);
                    break;
                case "SHUTDOWN":
                    getInfoTimer.Stop();
                    break;
                case "REACHED_DESTINATION":
                    ReachedDestination();
                    break;
                case "FIGHT":
                    Fight(location);
                    break;
                case "BUMP":
                    Bump(location);
                    break;
                case "ATE":
                    if (health > 70 && energy > 70)
                    {
                        hungry = false;
                    }
                    break;
                case "SCAN":
                    Scan(message);
                    break;
                case "SEE":
                    See(message);
                    break;
                case "HEALTH":
                    health = float.Parse(msgParts[2]);
                    healthKeyWord = msgParts[3];
                    if (health < 30)
                    {
                        hungry = true;
                    }
                    break;
                case "ENERGY":
                    energy = float.Parse(msgParts[2]);
                    if (energy < 40)
                    {
                        hungry = true;
                    }
                    break;
                case "LOCATION":
                    currentPosition = PointFrom(msgParts[2]);

                    if (grid.NodeRetriever(currentPosition).walkable)
                    {
                        lastValidLocation = grid.NodeRetriever(currentPosition);
                    }
                    break;
                case "LEVEL_TIME_REMAINING":
                    secondsRemaining = int.Parse(msgParts[2]);

                    if (secondsRemaining < 35)
                    {
                        headingForExit = true;
                    }
                    break;
                case "Error":
                    Log(message);
                    break;
                case "CRASHED":
                    System.Diagnostics.Debug.WriteLine(message);
                    break;
            }
        }

        public abstract void ReachedDestination();

        public abstract void Fight(string location);

        public abstract void Bump(string location);

        private string[] Split(string message)
        {
            string[] separation = message.Split('\n');
            string[] things = separation[1].Split('\t');

            return things;
        }

        private void See(string message)
        {
            List<Point> locationsToCheck = new List<Point>();
            bool sawNewObstacle = false;

            foreach (string thing in Split(message))
            {
                string[] thingAttributes = thing.Split(':');

                if (thingAttributes[0] == "Nothing")
                {
                    //If sees nothing does nothing. Prevents a crash
                }
                else
                {
                    Point location = PointFrom(thingAttributes[1]);
                    locationsToCheck.Add(location);

                    switch (thingAttributes[0])
                    {
                        case "Bomb":
                            if (!bombs.Contains(location))
                            {
                                SpotBombs(location);
                                sawNewObstacle = true;
                            }
                            break;
                        case "Critter":
                            Critter(location, thingAttributes[4], thingAttributes[5]);
                            break;
                        case "Terrain":
                            if (!walls.Contains(location))
                            {
                                SpotObstacles(location);
                                sawNewObstacle = true;
                            }
                            break;
                        case "Gift":
                            if (!gifts.Contains(location))
                            {
                                gifts.Add(location);
                            }
                            break;
                        case "Food":
                            if (!kiwis.Contains(location))
                            {
                                kiwis.Add(location);
                            }
                            break;
                    }
                }
            }

            kiwis = VisualConfirmation(kiwis, locationsToCheck);
            gifts = VisualConfirmation(gifts, locationsToCheck);
            bombs = VisualConfirmation(bombs, locationsToCheck);

            if (sawNewObstacle)
            {
                ReevaluatePath();
            }
        }

        public abstract void Critter(Point location, string strength, string status);

        private void Scan(string message)
        {
            gifts.Clear();
            kiwis.Clear();

            foreach (string thing in Split(message))
            {
                string[] thingAttributes = thing.Split(':');
                Point location = PointFrom(thingAttributes[1]);
                switch (thingAttributes[0])
                {
                    case "Gift":
                        gifts.Add(location);
                        break;
                    case "Food":
                        kiwis.Add(location);
                        break;
                    case "EscapeHatch":
                        exit = location;

                        for (int i = -3; i <= 3; i++)
                        {
                            for (int j = -3; j <= 3; j++)
                            {
                                if (!(grid.NodeRetriever(exit).x + i < 0 || grid.NodeRetriever(exit).x + i > 80 || grid.NodeRetriever(exit).y + j < 0 || grid.NodeRetriever(exit).y + j > 60))
                                {
                                    if (!headingForExit)
                                    {
                                        grid.nodes[grid.NodeRetriever(exit).x + i, grid.NodeRetriever(exit).y + j].walkable = false;
                                    }
                                    else
                                    {
                                        grid.nodes[grid.NodeRetriever(exit).x + i, grid.NodeRetriever(exit).y + j].walkable = true;
                                    }
                                }
                            }
                        }
                        break;
                }
            }
        }

        protected void FindDestination()
        {
            if (hungry)
            {
                destination = FindNearestItem(kiwis);
            }
            else
            {
                destination = FindNearestItem(gifts);
            }
        }

        protected void SetDestination(Point target)
        {
            Responder("SET_DESTINATION:" + target.X + ":" + target.Y + ":" + speed);
        }

        // It calculates distance squared but the outcome stays the same and it doesn't need to perform additional calculations.
        public static double CalculateDistance(Point position, Point thing)
        {
            return (position.X - thing.X) * (position.X - thing.X) + (position.Y - thing.Y) * (position.Y - thing.Y);
        }

        protected Point FindNearestItem(List<Point> items)
        {
            Point closest = Point.Empty;
            double distance = double.MaxValue;

            foreach (Point item in items)
            {
                double distanceToCheck = CalculateDistance(currentPosition, item);
                while (distanceToCheck < distance && grid.NodeRetriever(item).walkable)
                {
                    distance = distanceToCheck;
                    closest = item;
                }
            }
            return closest;
        }

        // Instead of rooting the item distance, square the seeing distance. If the item is too far assume it's there.
        private List<Point> VisualConfirmation(List<Point> items, List<Point> locations)
        {
            for (int i = items.Count - 1; i >= 0; i--)
            {
                if (CalculateDistance(currentPosition, items[i]) < 10000 && !locations.Contains(items[i]))
                {
                    items.Remove(items[i]);
                }
            }

            return items;
        }

        private void SpotObstacles(Point location)
        {
            walls.Add(location);
            Node wall = grid.NodeRetriever(location);

            for (int i = -2; i <= 3; i++)
            {
                for (int j = -2; j <= 3; j++)
                {
                    if (!(wall.x + i < 0 || wall.x + i > 80 || wall.y + j < 0 || wall.y + j > 60))
                    {
                        grid.nodes[wall.x + i, wall.y + j].walkable = false;
                    }
                }
            }
        }

        public void SpotBombs(Point location)
        {
            bombs.Add(location);
            Node bomb = grid.NodeRetriever(location);

            for (int i = -2; i <= 2; i++)
            {
                for (int j = -2; j <= 2; j++)
                {
                    if (!(bomb.x + i < 0 || bomb.x + i > 80 || bomb.y + j < 0 || bomb.y + j > 60))
                    {
                        grid.nodes[bomb.x + i, bomb.y + j].walkable = false;
                    }
                }
            }
        }

        protected void ReevaluatePath()
        {
            if (!grid.NodeRetriever(destination).walkable)
            {
                FindDestination();
            }
            pathBuffer = pathFinding.Star(grid.NodeRetriever(currentPosition), grid.NodeRetriever(destination), grid);
            if (pathBuffer.Count == 0)
            {
                //wait for the reach destination case to try again
            }
            else
            {
                SetDestination(PointMaker(pathBuffer.Peek()));
            }
        }

        public static Point PointMaker(Node coordinate)
        {
            int x = (int)Math.Round(coordinate.x * 13.75);
            int y = (int)Math.Round(coordinate.y * 13.75);

            return new Point(x, y);
        }
    }
}
