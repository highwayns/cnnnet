using cnnnet.Lib.AxonGuidanceForces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace cnnnet.Lib.Neurons
{
    public class NeuronInput : NeuronBase
    {
        #region Instance

        public NeuronInput(int id, CnnNet cnnNet, IEnumerable<IAxonGuidanceForce> axonGuidanceForces)
            : base(id, cnnNet, axonGuidanceForces)
        {
            HasSomaReachedFinalPosition = true;
        }

        #endregion Instance
    }
}