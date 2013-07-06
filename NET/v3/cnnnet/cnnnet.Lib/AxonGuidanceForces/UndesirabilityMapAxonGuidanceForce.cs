using cnnnet.Lib.Neurons;
using cnnnet.Lib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cnnnet.Lib.AxonGuidanceForces
{
    public class UndesirabilityMapAxonGuidanceForce : AxonGuidanceForceBase
    {
        public override double ComputeScoreAtLocation(Neuron neuron, CnnNet network, Point location)
        {
            return network.NeuronUndesirabilityMap[location.Y, location.X];
        }

        protected override bool PreCheckLocation(Neuron neuron, CnnNet network, int x, int y)
        {
            return base.PreCheckLocation(neuron, network, x, y)
                && Math.Abs(network.NeuronUndesirabilityMap[y, x] - 0.0d) < 0.00001 == false;
        }
    }
}