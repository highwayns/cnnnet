using cnnnet.Lib.Neurons;
using System;
using System.Diagnostics.Contracts;

namespace cnnnet.Lib.GuidanceForces.Axon
{
    public class AxonUndesirabilityMapGuidanceForce : AxonGuidanceForceBase
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

        public AxonUndesirabilityMapGuidanceForce(CnnNet network)
            : base(network)
        {
            Contract.Requires<ArgumentNullException>(network != null);
        }

        #endregion
    }
}