using cnnnet.Lib.Neurons;
using cnnnet.Lib.Utils;
using System;


namespace cnnnet.Lib.GuidanceForces.Soma
{
    public abstract class SomaGuidanceForceBase : GuidanceForceBase
    {
        #region Methods

        protected override bool PreCheckLocation(int x, int y, Neuron neuron)
        {
            return base.PreCheckLocation(x, y, neuron)
                && (x != neuron.PosX || y != neuron.PosY)
                && Extensions.GetNeuronAt(y, x, Network) == null
                && neuron.MovedDistance + Extensions.GetDistance(neuron.PosX, neuron.PosY, x, y) < Network.MaxNeuronMoveDistance
                && Extensions.GetDistanceToNearestNeuron(y, x, neuron, Network) > Network.MinDistanceBetweenNeurons;
        }

        #endregion

        #region Instance

        protected SomaGuidanceForceBase(CnnNet network)
            : base(network.SomaGuidanceForceSearchPlainRange, network)
        {
            Contract.Requires<ArgumentNullException>(network != null);
        }

        #endregion
    }
}
