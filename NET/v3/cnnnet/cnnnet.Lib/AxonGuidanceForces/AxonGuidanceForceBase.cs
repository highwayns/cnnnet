using cnnnet.Lib.Neurons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cnnnet.Lib.AxonGuidanceForces
{
    public abstract class AxonGuidanceForceBase
    {
        public double[,] GetScore(Neuron neuron, CnnNet network)
        {
            var result = new double[2 * network.AxonGuidanceForceSearchPlainRange, 2 * network.AxonGuidanceForceSearchPlainRange];

            ComputeScoreInternal(neuron, network, result);

            return result;
        }

        protected abstract void ComputeScoreInternal(Neuron neuron, CnnNet network, double[,] scoreMap);
    }
}