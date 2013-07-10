using cnnnet.Lib.Neurons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        }

        #endregion
    }
}
