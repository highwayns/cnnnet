using cnnnet.Lib.Neurons;
using System;
using System.Diagnostics.Contracts;

namespace cnnnet.Lib.GuidanceForces.Axon
{
    public class AxonFollowExistingGuidanceForce : AxonGuidanceForceBase
    {
        #region Methods

        public override double ComputeScoreAtLocation(int x, int y, Neuron neuron)
        {
            return 0;
        }

        #endregion

        #region Instance

        public AxonFollowExistingGuidanceForce(CnnNet network)
            : base(network)
        {
            Contract.Requires<ArgumentNullException>(network != null);
        }

        #endregion
    }
}
