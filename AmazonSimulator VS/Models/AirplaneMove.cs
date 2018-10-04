using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public class AirplaneMove : ITask<Airplane>
    {
        List<Coordinate> coordinates;

        public void StartTask(Airplane r)
        {
            throw new NotImplementedException();
        }

        public bool TaskComplete(Airplane r)
        {
            throw new NotImplementedException();
        }
    }
}