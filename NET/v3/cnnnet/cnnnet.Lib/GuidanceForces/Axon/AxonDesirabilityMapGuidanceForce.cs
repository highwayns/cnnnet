using cnnnet.Lib.Neurons;
using System;

namespace cnnnet.Lib.GuidanceForces.Axon
{
    public class AxonDesirabilityMapGuidanceForce : AxonGuidanceForceBase
    {
        #region Methods
        
        public override double ComputeScoreAtLocation(int x, int y, Neuron neuron)
        {
            return -Network.NeuronDesirabilityMap[y, x];
        }

        protected override bool PreCheckLocation(int x, int y, Neuron neuron)
        {
            return base.PreCheckLocation(x, y, neuron)
                && Math.Abs(Network.NeuronDesirabilityMap[y, x] - 0.0d) < 0.00001 == false;
        }

        #endregion

        #region Instance

        public AxonDesirabilityMapGuidanceForce(CnnNet network)
            : base(network)
        {
        }

        #endregion
    }
}