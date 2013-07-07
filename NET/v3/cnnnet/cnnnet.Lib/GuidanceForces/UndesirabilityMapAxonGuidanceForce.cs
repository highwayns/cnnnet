using cnnnet.Lib.Neurons;
using cnnnet.Lib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cnnnet.Lib.GuidanceForces
{
    public class UndesirabilityMapAxonGuidanceForce : AxonGuidanceForceBase
    {
        #region Methods

        public override double ComputeScoreAtLocation(int x, int y, Neuron neuron)
        {
            return Network.NeuronUndesirabilityMap[y, x];
        }

        protected override bool PreCheckLocation(int x, int y, Neuron neuron)
        {
            return base.PreCheckLocation(x, y, neuron)
                && Math.Abs(Network.NeuronUndesirabilityMap[y, x] - 0.0d) < 0.00001 == false;
        }

        #endregion

        #region Instance

        public UndesirabilityMapAxonGuidanceForce(CnnNet network)
            : base(network)
        {

        }

        #endregion
    }
}