using System;
using System.Collections.Generic;
using System.Linq;

namespace Models {
    interface IUpdatable
    {
        /// <summary>
        /// Update method
        /// </summary>
        /// <param name="tick"></param>
        /// <returns></returns>
        bool Update(int tick);
    }
}
