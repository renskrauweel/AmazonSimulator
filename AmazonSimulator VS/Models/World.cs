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
            new Coordinate(10, 0, 12, 'Q', true), // First Inner Sector - Bottom Right - O
            new Coordinate(15, 0, 12, 'P', true), // First Inner Sector - Bottom Center - P
            new Coordinate(20, 0, 12, 'O', true), // First Inner Sector - Bottom Left - Q

            new Coordinate(10, 0, 15, 'R'), // Second Inner Sector - Top Left - R
            new Coordinate(15, 0, 15, 'S'), // Second Inner Sector - Top Center - S
            new Coordinate(20, 0, 15, 'T'), // Second Inner Sector - Top Right - T
            new Coordinate(10, 0, 17, 'W', true), // Second Inner Sector - Bottom Right - U
            new Coordinate(15, 0, 17, 'V', true), // Second Inner Sector - Bottom Center - V
            new Coordinate(20, 0, 17, 'U', true), // Second Inner Sector - Bottom Left - W

            new Coordinate(10, 0, 20, 'X'), // Third Inner Sector - Top Left - X
            new Coordinate(15, 0, 20, 'Y'), // Third Inner Sector - Top Center - Y
            new Coordinate(20, 0, 20, 'Z'), // Third Inner Sector - Top Right - Z
            new Coordinate(10, 0, 22, '3', true), // Third Inner Sector - Bottom Right - 3
            new Coordinate(15, 0, 22, '2', true), // Third Inner Sector - Bottom Center - 2
            new Coordinate(20, 0, 22, '1', true), // Third Inner Sector - Bottom Left - 1

        };
        private Graph g = new Graph();
        private List<Coordinate> occupationList = new List<Coordinate>();

        /// <summary>
        /// Constructs the world
        /// </summary>
        public World() {
            InitGraph();

            for (int i = 0; i < 4; i++)
            {
                Robot r = CreateRobot(0, 0, 0);
                r.Move(15, 0, 5);
            }

            Airplane a = CreateAirplane(0, 0, 0);
            a.Move(15, 4.3, -15);
            a.Rotate(0, Math.PI / 2, 0);

            PlaceSuitcases(this.coordinates);
        }

        /// <summary>
        /// Create a Robot and add to the world
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        private Robot CreateRobot(double x, double y, double z) {
            Robot r = new Robot(x,y,z,0,0,0);
            worldObjects.Add(r);
            return r;
        }

        /// <summary>
        /// Create an Airplane and add to the world
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        private Airplane CreateAirplane(double x, double y, double z)
        {
            Airplane t = new Airplane(x, y, z, 0, 0, 0);
            worldObjects.Add(t);
            return t;
        }

        /// <summary>
        /// Create Suitcase and add to the world
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        private Suitcase CreateSuitcase(double x, double y, double z)
        {
            Suitcase s = new Suitcase(x, y, z, 0, 0, 0);
            worldObjects.Add(s);
            return s;
        }

        /// <summary>
        /// Subscribe observer
        /// </summary>
        /// <param name="observer"></param>
        /// <returns></returns>
        public IDisposable Subscribe(IObserver<Command> observer)
        {
            if (!observers.Contains(observer)) {
                observers.Add(observer);

                SendCreationCommandsToObserver(observer);
            }
            return new Unsubscriber<Command>(observers, observer);
        }

        /// <summary>
        /// Send command to observers
        /// </summary>
        /// <param name="c"></param>
        private void SendCommandToObservers(Command c) {
            for(int i = 0; i < this.observers.Count; i++) {
                this.observers[i].OnNext(c);
            }
        }

        /// <summary>
        /// Send creation commands to observer
        /// </summary>
        /// <param name="obs"></param>
        private void SendCreationCommandsToObserver(IObserver<Command> obs) {
            foreach(BaseModel m3d in worldObjects) {
                obs.OnNext(new UpdateModel3DCommand(m3d));
            }
        }

        /// <summary>
        /// Update method by ticktime
        /// </summary>
        /// <param name="tick"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Gets the robots
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Gets the suitcases
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Gets the airplanes
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Place suitcases in the world
        /// </summary>
        /// <param name="coordinates"></param>
        private void PlaceSuitcases(List<Coordinate> coordinates)
        {
            foreach (Coordinate c in coordinates)
            {
                if (c.CanBeOccupied())
                {
                    occupationList.Add(c);
                }
            }

            int totalSuitcases = 8;
            Random rnd = new Random();
            for (int i = 0; i < totalSuitcases; i++)
            {
                int r = rnd.Next(occupationList.Count);
                Coordinate c = occupationList[r];
                c.GiveSuitcase(CreateSuitcase(c.GetX(), c.GetY(), c.GetZ()));
                occupationList.RemoveAt(r);
            }
        }

        /// <summary>
        /// Get Coordinates occupied
        /// </summary>
        /// <returns></returns>
        public List<Coordinate> GetOccupationList()
        {
            List<Coordinate> suitcaseCoordinates = new List<Coordinate>();
            foreach (Coordinate coordinate in this.coordinates)
            {
                if (coordinate.GetSuitcase() != null)
                {
                    suitcaseCoordinates.Add(coordinate);
                }
            }
            return suitcaseCoordinates;
        }

        /// <summary>
        /// Get all coordinates
        /// </summary>
        /// <returns></returns>
        public List<Coordinate> GetCoordinates()
        {
            return this.coordinates;
        }
        /// <summary>
        /// Get the graph
        /// </summary>
        /// <returns></returns>
        public Graph GetGraph()
        {
            return this.g;
        }

        /// <summary>
        /// Initialise the graph
        /// </summary>
        private void InitGraph()
        {
            //Start
            g.add_vertex('A', new Dictionary<char, int>() { { 'B', 10 }, { 'K', 10 } });

            //Right Lane
            g.add_vertex('B', new Dictionary<char, int>() { { 'A', 10 }, { 'C', 5 } });
            g.add_vertex('C', new Dictionary<char, int>() { { 'B', 5 }, { 'D', 5 }, { 'N', 5 } });
            g.add_vertex('D', new Dictionary<char, int>() { { 'C', 5 }, { 'E', 5 }, { 'T', 5 } });
            g.add_vertex('E', new Dictionary<char, int>() { { 'D', 5 }, { 'F', 5 }, { 'Z', 5 } });
            g.add_vertex('F', new Dictionary<char, int>() { { 'E', 5 }, { 'G', 20 } });

            //Left Lane
            g.add_vertex('K', new Dictionary<char, int>() { { 'A', 10 }, { 'J', 5 } });
            g.add_vertex('J', new Dictionary<char, int>() { { 'K', 5 }, { 'I', 5 }, { 'L', 5 } });
            g.add_vertex('I', new Dictionary<char, int>() { { 'J', 5 }, { 'H', 5 }, { 'R', 5 } });
            g.add_vertex('H', new Dictionary<char, int>() { { 'I', 5 }, { 'G', 5 }, { 'X', 5 } });
            g.add_vertex('G', new Dictionary<char, int>() { { 'H', 5 }, { 'F', 20 } });

            //First Inner Sector
            g.add_vertex('L', new Dictionary<char, int>() { { 'J', 5 }, { 'M', 5 }, { 'Q', 2 } });
            g.add_vertex('M', new Dictionary<char, int>() { { 'N', 5 }, { 'L', 5 }, { 'P', 2 } });
            g.add_vertex('N', new Dictionary<char, int>() { { 'C', 5 }, { 'M', 5 }, { 'O', 2 } });
            g.add_vertex('O', new Dictionary<char, int>() { { 'N', 2 } });
            g.add_vertex('P', new Dictionary<char, int>() { { 'M', 2 } });
            g.add_vertex('Q', new Dictionary<char, int>() { { 'L', 2 } });

            //Second Inner Sector
            g.add_vertex('R', new Dictionary<char, int>() { { 'I', 5 }, { 'S', 5 }, { 'W', 2 } });
            g.add_vertex('S', new Dictionary<char, int>() { { 'T', 5 }, { 'R', 5 }, { 'V', 2 } });
            g.add_vertex('T', new Dictionary<char, int>() { { 'D', 5 }, { 'S', 5 }, { 'U', 2 } });
            g.add_vertex('U', new Dictionary<char, int>() { { 'T', 2 } });
            g.add_vertex('V', new Dictionary<char, int>() { { 'S', 2 } });
            g.add_vertex('W', new Dictionary<char, int>() { { 'R', 2 } });

            //Third Inner Sector
            g.add_vertex('X', new Dictionary<char, int>() { { 'H', 5 }, { 'Y', 5 }, { '3', 2 } });
            g.add_vertex('Y', new Dictionary<char, int>() { { 'Z', 5 }, { 'X', 5 }, { '2', 2 } });
            g.add_vertex('Z', new Dictionary<char, int>() { { 'E', 5 }, { 'Y', 5 }, { '1', 2 } });
            g.add_vertex('1', new Dictionary<char, int>() { { 'Z', 2 } });
            g.add_vertex('2', new Dictionary<char, int>() { { 'Y', 2 } });
            g.add_vertex('3', new Dictionary<char, int>() { { 'X', 2 } });
        }
    }

    internal class Unsubscriber<Command> : IDisposable
    {
        private List<IObserver<Command>> _observers;
        private IObserver<Command> _observer;

        /// <summary>
        /// Unsubscribe method
        /// </summary>
        /// <param name="observers"></param>
        /// <param name="observer"></param>
        internal Unsubscriber(List<IObserver<Command>> observers, IObserver<Command> observer)
        {
            this._observers = observers;
            this._observer = observer;
        }

        /// <summary>
        /// Dispose method
        /// </summary>
        public void Dispose() 
        {
            if (_observers.Contains(_observer))
                _observers.Remove(_observer);
        }
    }
}