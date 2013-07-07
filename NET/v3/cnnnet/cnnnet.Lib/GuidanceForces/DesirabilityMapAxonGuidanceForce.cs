using cnnnet.Lib.Neurons;
using cnnnet.Lib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cnnnet.Lib.GuidanceForces
{
    public class DesirabilityMapAxonGuidanceForce : AxonGuidanceForceBase
    {
        #region Methods
        
        public override double ComputeScoreAtLocation(int x, int y, Neuron neuron)
        {
            return -Network.NeuronDesirabilityMap[y, x];
        }

        protected override bool PreCheckLocation(int x, int y, Neuron neuron)
        {
            return base.PreCheckLocation(x, y, neuron)
                && Math.Abs(Network.NeuronDesirabilityMap[y, x] - 0.0d) < 0.00001 == false;
        }

        #endregion

        #region Instance

        public DesirabilityMapAxonGuidanceForce(CnnNet network)
            : base(network)
        {
        }

        #endregion
    }
}