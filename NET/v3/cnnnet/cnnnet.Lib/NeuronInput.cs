using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace cnnnet.Lib
{
    public class NeuronInput : NeuronBase
    {
        #region Instance

        public NeuronInput(int id, CnnNet cnnNet)
            : base(id, cnnNet)
        {
            HasSomaReachedFinalPosition = true;
        }

        #endregion
    }
}
