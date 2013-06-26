using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace cnnnet.Lib.Neurons
{
    public class NeuronInput : NeuronBase
    {
        #region Instance

        public NeuronInput(int id, CnnNet cnnNet)
            : base(id, cnnNet)
        {
            HasSomaReachedFinalPosition = true;
        }

        #endregion Instance
    }
}