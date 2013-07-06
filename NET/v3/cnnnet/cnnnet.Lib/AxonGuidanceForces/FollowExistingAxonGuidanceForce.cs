using cnnnet.Lib.Neurons;
using cnnnet.Lib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cnnnet.Lib.AxonGuidanceForces
{
    public class FollowExistingAxonGuidanceForce : AxonGuidanceForceBase
    {
        public override double ComputeScoreAtLocation(Neuron neuron, CnnNet network, Point location)
        {
            return 0;
        }
    }
}
