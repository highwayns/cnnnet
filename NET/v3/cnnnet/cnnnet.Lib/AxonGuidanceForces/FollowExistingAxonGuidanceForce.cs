using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cnnnet.Lib.AxonGuidanceForces
{
    public class FollowExistingAxonGuidanceForce : AxonGuidanceForceBase
    {
        protected override void ComputeScoreInternal(Neurons.Neuron neuron, CnnNet network, double[,] scoreMap)
        {
        }
    }
}
