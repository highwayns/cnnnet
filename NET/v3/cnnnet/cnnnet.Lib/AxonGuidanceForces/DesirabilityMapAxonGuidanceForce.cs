using cnnnet.Lib.Neurons;
using cnnnet.Lib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cnnnet.Lib.AxonGuidanceForces
{
    public class DesirabilityMapAxonGuidanceForce : AxonGuidanceForceBase
    {
        protected override void ComputeScoreInternal(Neuron neuron, CnnNet network, int x, int y, ref double result)
        {
            result = -network.NeuronDesirabilityMap[y, x];
        }

        protected override bool PreCheckLocation(Neuron neuron, CnnNet network, int x, int y)
        {
            return base.PreCheckLocation(neuron, network, x, y)
                && Math.Abs(network.NeuronDesirabilityMap[y, x] - 0.0d) < 0.00001 == false;
        }
    }
}