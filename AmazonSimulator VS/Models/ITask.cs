using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public interface ITask
    {
        void StartTask(Robot r);

        bool TaskComplete(Robot r);
    }
}
