using cnnnet.Lib.Neurons;
using cnnnet.Lib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cnnnet.Lib.GuidanceForces
{
    public class FollowExistingAxonGuidanceForce : AxonGuidanceForceBase
    {
        #region Methods

        public override double ComputeScoreAtLocation(int x, int y, Neuron neuron)
        {
            return 0;
        }

        #endregion

        #region Instance

        public FollowExistingAxonGuidanceForce(CnnNet network)
            : base(network)
        {
        }

        #endregion
    }
}
