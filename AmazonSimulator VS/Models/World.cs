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
        private Graph g = new Graph();

        public World() {
            InitGraph();

            Robot r = CreateRobot(0,0,0);
            r.Move(4.6, 0, 13);

            Airplane t = CreateAirplane(0, 0, 0);
            t.Move(15, 1.45, -9);
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

        private Airplane CreateAirplane(double x, double y, double z)
        {
            Airplane t = new Airplane(x, y, z, 0, 0, 0);
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

        public List<Airplane> GetAirplanes()
        {
            List<Airplane> airplanes = new List<Airplane>();
            foreach (BaseModel airplane in worldObjects)
            {
                if (airplane is Airplane)
                {
                    airplanes.Add((Airplane)airplane);
                }
            }
            return airplanes;
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
        public Graph GetGraph()
        {
            return this.g;
        }

        private void InitGraph()
        {
            //Start
            g.add_vertex('A', new Dictionary<char, int>() { { 'B', 10 }, { 'K', 10 } });

            //Right Lane
            g.add_vertex('B', new Dictionary<char, int>() { { 'A', 10 }, { 'C', 20 } });
            g.add_vertex('C', new Dictionary<char, int>() { { 'B', 20 }, { 'D', 30 }, { 'N', 30 } });
            g.add_vertex('D', new Dictionary<char, int>() { { 'C', 30 }, { 'E', 40 }, { 'T', 40 } });
            g.add_vertex('E', new Dictionary<char, int>() { { 'D', 40 }, { 'F', 50 }, { 'Z', 50 } });
            g.add_vertex('F', new Dictionary<char, int>() { { 'E', 50 }, { 'G', 70 } });

            //Left Lane
            g.add_vertex('K', new Dictionary<char, int>() { { 'A', 10 }, { 'J', 20 } });
            g.add_vertex('J', new Dictionary<char, int>() { { 'K', 20 }, { 'I', 30 }, { 'L', 30 } });
            g.add_vertex('I', new Dictionary<char, int>() { { 'J', 30 }, { 'H', 40 }, { 'R', 40 } });
            g.add_vertex('H', new Dictionary<char, int>() { { 'I', 40 }, { 'G', 50 }, { 'X', 50 } });
            g.add_vertex('G', new Dictionary<char, int>() { { 'H', 50 }, { 'F', 70 } });

            //First Inner Sector
            g.add_vertex('L', new Dictionary<char, int>() { { 'J', 30 }, { 'M', 50 }, { 'Q', 40 } });
            g.add_vertex('M', new Dictionary<char, int>() { { 'N', 40 }, { 'L', 50 }, { 'P', 50 } });
            g.add_vertex('N', new Dictionary<char, int>() { { 'C', 30 }, { 'M', 40 }, { 'O', 40 } });
            g.add_vertex('O', new Dictionary<char, int>() { { 'N', 40 } });
            g.add_vertex('P', new Dictionary<char, int>() { { 'M', 50 } });
            g.add_vertex('Q', new Dictionary<char, int>() { { 'L', 40 } });

            //Second Inner Sector
            g.add_vertex('R', new Dictionary<char, int>() { { 'I', 40 }, { 'S', 60 }, { 'W', 50 } });
            g.add_vertex('S', new Dictionary<char, int>() { { 'T', 50 }, { 'R', 60 }, { 'V', 60 } });
            g.add_vertex('T', new Dictionary<char, int>() { { 'D', 40 }, { 'S', 50 }, { 'U', 50 } });
            g.add_vertex('U', new Dictionary<char, int>() { { 'T', 50 } });
            g.add_vertex('V', new Dictionary<char, int>() { { 'S', 60 } });
            g.add_vertex('W', new Dictionary<char, int>() { { 'R', 50 } });

            //Third Inner Sector
            g.add_vertex('X', new Dictionary<char, int>() { { 'H', 50 }, { 'Y', 70 }, { '3', 60 } });
            g.add_vertex('Y', new Dictionary<char, int>() { { 'Z', 60 }, { 'X', 70 }, { '2', 70 } });
            g.add_vertex('Z', new Dictionary<char, int>() { { 'E', 50 }, { 'Y', 60 }, { '1', 60 } });
            g.add_vertex('1', new Dictionary<char, int>() { { 'Z', 60 } });
            g.add_vertex('2', new Dictionary<char, int>() { { 'Y', 70 } });
            g.add_vertex('3', new Dictionary<char, int>() { { 'X', 60 } });
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