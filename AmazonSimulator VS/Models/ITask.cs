using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public interface ITask
    {
        void StartTask(BaseModel r);

        bool TaskComplete(BaseModel r);
    }
}
