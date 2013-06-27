using cnnnet.Lib.AxonGuidanceForces;
using System.Collections.Generic;

namespace cnnnet.Lib.Neurons
{
    public class NeuronCompute : NeuronBase
    {
        #region Instance

        public NeuronCompute(int id, CnnNet cnnNet, IEnumerable<IAxonGuidanceForce> axonGuidanceForces)
            : base(id, cnnNet, axonGuidanceForces)
        {
        }

        #endregion Instance
    }
}