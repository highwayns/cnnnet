using cnnnet.Lib.Neurons;
using System;
using System.Diagnostics.Contracts;

namespace cnnnet.Lib.GuidanceForces.Soma
{
    public class SomaUndesirabilityMapGuidanceForce : SomaGuidanceForceBase
    {
        #region Methods

        public override double ComputeScoreAtLocation(int x, int y, Neuron neuron)
        {
            return -Network.NeuronUndesirabilityMap[y, x];
        }

        #endregion

        #region Instance

        public SomaUndesirabilityMapGuidanceForce(CnnNet network)
            : base(network)
        {
            Contract.Requires<ArgumentNullException>(network != null);
        }

        #endregion
    }
}
