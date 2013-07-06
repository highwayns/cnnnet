using cnnnet.Lib.Neurons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cnnnet.Lib.AxonGuidanceForces
{
    public class FollowExistingAxonGuidanceForce : AxonGuidanceForceBase
    {
        protected override void ComputeScoreInternal(Neuron neuron, CnnNet network, int x, int y, ref double result)
        {
            result = double.MinValue;
        }
    }
}
