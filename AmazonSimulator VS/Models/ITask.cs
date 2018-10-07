using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public interface ITask<T>
    {
        /// <summary>
        /// Start task method
        /// </summary>
        /// <param name="r"></param>
        void StartTask(T r);

        /// <summary>
        /// Task complete method
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        bool TaskComplete(T r);
    }
}
