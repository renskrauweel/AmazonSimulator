using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public class AirplaneMove : ITask
    {
        List<Coordinate> coordinates;

        public void StartTask(BaseModel r)
        {
            throw new NotImplementedException();
        }

        public bool TaskComplete(BaseModel r)
        {
            throw new NotImplementedException();
        }
    }
}