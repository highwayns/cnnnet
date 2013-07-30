using cnnnet.Lib.Neurons;

namespace cnnnet.Lib.GuidanceForces.Soma
{
    public class SomaUndesirabilityMapGuidanceForce : SomaGuidanceForceBase
    {
        #region Methods

        public override double ComputeScoreAtLocation(int x, int y, Neuron neuron)
        {
            return -Network.NeuronUndesirabilityMap[y, x];
        }

        #endregion

        #region Instance

        public SomaUndesirabilityMapGuidanceForce(CnnNet network)
            : base(network)
        {
        }

        #endregion
    }
}
