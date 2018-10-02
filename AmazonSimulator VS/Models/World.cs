using System;
using System.Collections.Generic;
using System.Linq;
using Controllers;

namespace Models {
    public class World : IObservable<Command>, IUpdatable
    {
        private List<BaseModel> worldObjects = new List<BaseModel>();
        private List<IObserver<Command>> observers = new List<IObserver<Command>>();
        private List<Coordinate> coordinates = new List<Coordinate> {
            new Coordinate(15, 0, 5, 'A'), // Outer Line - Top center - A
            new Coordinate(25, 0, 5, 'B'), // Outer Line - Top right - B
            new Coordinate(25, 0, 10, 'C'), // Outer Line - Right lane 1 - C
            new Coordinate(25, 0, 15, 'D'), // Outer Line - Right lane 2 - D
            new Coordinate(25, 0, 20, 'E'), // Outer Line - Right lane 3 - E
            new Coordinate(25, 0, 25, 'F'), // Outer Line - Bottom right - F
            new Coordinate(5, 0, 25, 'G'), // Outer Line - Bottom left - G
            new Coordinate(5, 0, 20, 'H'), // Outer Line - Left lane 3
            new Coordinate(5, 0, 15, 'I'), // Outer Line - Left lane 2
            new Coordinate(5, 0, 10, 'J'), // Outer Line - Left lane 1
            new Coordinate(5, 0, 5, 'K'), // Outer Line - Top left - B

            new Coordinate(10, 0, 10, 'L'), // First Inner Sector - Top Left - L
            new Coordinate(15, 0, 10, 'M'), // First Inner Sector - Top Center - M
            new Coordinate(20, 0, 10, 'N'), // First Inner Sector - Top Right - N
            new Coordinate(10, 0, 12, 'O', true), // First Inner Sector - Bottom Right - O
            new Coordinate(15, 0, 12, 'P', true), // First Inner Sector - Bottom Center - P
            new Coordinate(20, 0, 12, 'Q', true), // First Inner Sector - Bottom Left - Q

            new Coordinate(10, 0, 15, 'R'), // Second Inner Sector - Top Left - R
            new Coordinate(15, 0, 15, 'S'), // Second Inner Sector - Top Center - S
            new Coordinate(20, 0, 15, 'T'), // Second Inner Sector - Top Right - T
            new Coordinate(10, 0, 17, 'U', true), // Second Inner Sector - Bottom Right - U
            new Coordinate(15, 0, 17, 'V', true), // Second Inner Sector - Bottom Center - V
            new Coordinate(20, 0, 17, 'W', true), // Second Inner Sector - Bottom Left - W

            new Coordinate(10, 0, 10, 'X'), // Third Inner Sector - Top Left - X
            new Coordinate(15, 0, 10, 'Y'), // Third Inner Sector - Top Center - Y
            new Coordinate(20, 0, 10, 'Z'), // Third Inner Sector - Top Right - Z
            new Coordinate(10, 0, 12, '1', true), // Third Inner Sector - Bottom Right - Æ
            new Coordinate(15, 0, 12, '2', true), // Third Inner Sector - Bottom Center - Ø
            new Coordinate(20, 0, 12, '3', true), // Third Inner Sector - Bottom Left - Å

        };

    public World() {
            Robot r = CreateRobot(0,0,0);
            r.Move(4.6, 0, 13);

            Truck t = CreateTruck(0, 0, 0);
            t.Move(5, 1.6, 0);
            t.Rotate(0, Math.PI / 2, 0);

            //Suitcase s1 = CreateSuitcase(0, 0, 0);
            //s1.Move(20, 0, 11);
            //Suitcase s2 = CreateSuitcase(0, 0, 0);
            //s2.Move(20, 0, 16);
            PlaceSuitcases(this.coordinates);
        }

        private Robot CreateRobot(double x, double y, double z) {
            Robot r = new Robot(x,y,z,0,0,0);
            worldObjects.Add(r);
            return r;
        }

        private Truck CreateTruck(double x, double y, double z)
        {
            Truck t = new Truck(x, y, z, 0, 0, 0);
            worldObjects.Add(t);
            return t;
        }

        private Suitcase CreateSuitcase(double x, double y, double z)
        {
            Suitcase s = new Suitcase(x, y, z, 0, 0, 0);
            worldObjects.Add(s);
            return s;
        }

        public IDisposable Subscribe(IObserver<Command> observer)
        {
            if (!observers.Contains(observer)) {
                observers.Add(observer);

                SendCreationCommandsToObserver(observer);
            }
            return new Unsubscriber<Command>(observers, observer);
        }

        private void SendCommandToObservers(Command c) {
            for(int i = 0; i < this.observers.Count; i++) {
                this.observers[i].OnNext(c);
            }
        }

        private void SendCreationCommandsToObserver(IObserver<Command> obs) {
            foreach(BaseModel m3d in worldObjects) {
                obs.OnNext(new UpdateModel3DCommand(m3d));
            }
        }

        public bool Update(int tick)
        {
            for(int i = 0; i < worldObjects.Count; i++) {
                BaseModel u = worldObjects[i];

                if(u is IUpdatable) {
                    bool needsCommand = ((IUpdatable)u).Update(tick);

                    if(needsCommand) {
                        SendCommandToObservers(new UpdateModel3DCommand(u));
                    }
                }
            }

            return true;
        }

        public List<Robot> GetRobots()
        {
            List<Robot> robots = new List<Robot>();
            foreach (BaseModel r in this.worldObjects)
            {
                if(r is Robot)
                {
                    robots.Add((Robot)r);
                }
            }
            return robots;
        }

        public List<Suitcase> GetSuitcases()
        {
            List<Suitcase> suitcases = new List<Suitcase>();
            foreach (BaseModel s in this.worldObjects)
            {
                if (s is Suitcase)
                {
                    suitcases.Add((Suitcase)s);
                }
            }
            return suitcases;
        }

        public List<Truck> GetTrucks()
        {
            List<Truck> trucks = new List<Truck>();
            foreach (BaseModel truck in worldObjects)
            {
                if (truck is Truck)
                {
                    trucks.Add((Truck)truck);
                }
            }
            return trucks;
        }

        private void PlaceSuitcases(List<Coordinate> coordinates)
        {
            List<Coordinate> occupationList = new List<Coordinate>();
            foreach (Coordinate c in coordinates)
            {
                if (c.CanBeOccupied())
                {
                    occupationList.Add(c);
                }
            }

            int totalSuitcases = 4;
            Random rnd = new Random();
            for (int i = 0; i < totalSuitcases; i++)
            {
                int r = rnd.Next(occupationList.Count);
                Coordinate c = occupationList[r];
                c.GiveSuitcase(CreateSuitcase(c.GetX(), c.GetY(), c.GetZ()));
                occupationList.RemoveAt(r);
            }
        }

        public List<Coordinate> GetCoordinates()
        {
            return this.coordinates;
        }
    }

    internal class Unsubscriber<Command> : IDisposable
    {
        private List<IObserver<Command>> _observers;
        private IObserver<Command> _observer;

        internal Unsubscriber(List<IObserver<Command>> observers, IObserver<Command> observer)
        {
            this._observers = observers;
            this._observer = observer;
        }

        public void Dispose() 
        {
            if (_observers.Contains(_observer))
                _observers.Remove(_observer);
        }
    }
}