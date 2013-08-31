using cnnnet.Lib.Neurons;
using cnnnet.Lib.Utils;
using System;


namespace cnnnet.Lib.GuidanceForces.Soma
{
    public class SomaDesirabilityMapGuidanceForce : SomaGuidanceForceBase
    {
        #region Methods

        public override double ComputeScoreAtLocation(int x, int y, Neuron neuron)
        {
            return Network.NeuronDesirabilityMap[y, x];
        }

        #endregion

        #region Instance

        public SomaDesirabilityMapGuidanceForce(CnnNet network)
            : base(network)
        {
            Contract.Requires<ArgumentNullException>(network != null);
        }

        #endregion
    }
}
