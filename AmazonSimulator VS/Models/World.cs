using System;
using System.Collections.Generic;
using System.Linq;
using Controllers;

namespace Models {
    public class World : IObservable<Command>, IUpdatable
    {
        private List<BaseModel> worldObjects = new List<BaseModel>();
        private List<IObserver<Command>> observers = new List<IObserver<Command>>();
        
        public World() {
            Robot r = CreateRobot(0,0,0);
            r.Move(4.6, 0, 13);

            Truck t = CreateTruck(0, 0, 0);
            t.Move(4.6, 0, 13);

            Suitcase s1 = CreateSuitcase(0, 0, 0);
            s1.Move(25, 0, 10);
            Suitcase s2 = CreateSuitcase(0, 0, 0);
            s2.Move(20, 0, 16);
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