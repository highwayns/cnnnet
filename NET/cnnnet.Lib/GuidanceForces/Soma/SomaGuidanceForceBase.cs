using cnnnet.Lib.Neurons;
using cnnnet.Lib.Utils;
using System;
using System.Diagnostics.Contracts;

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
                && neuron.MovedDistance + Extensions.GetDistance(neuron.PosX, neuron.PosY, x, y) < Network.MaxNeuronMoveDistance;
        }

        protected override bool PostCheckLocation(int x, int y, Neuron neuron)
        {
            return base.PostCheckLocation(x, y, neuron)
                && GetDistanceToNearestNeuron(y, x, neuron) > Network.MinDistanceBetweenNeurons;
        }

        /// <summary>
        /// Remember to optimize this by using spiral matrix processing
        /// http://pastebin.com/4EYJvv5X
        /// </summary>
        /// <param name="referenceY"></param>
        /// <param name="referenceX"></param>
        /// <param name="neuron"></param>
        /// <returns></returns>
        private double GetDistanceToNearestNeuron(int referenceY, int referenceX, Neuron neuron)
        {
            double minDistance = Network.MinDistanceBetweenNeurons + 1;

            int xMin = Math.Max(referenceX - Network.MinDistanceBetweenNeurons, 0);
            int xMax = Math.Min(referenceX + Network.MinDistanceBetweenNeurons, Network.Width - 1);
            int yMin = Math.Max(referenceY - Network.MinDistanceBetweenNeurons, 0);
            int yMax = Math.Min(referenceY + Network.MinDistanceBetweenNeurons, Network.Height - 1);

            for (int y = yMin; y <= yMax; y++)
            {
                for (int x = xMin; x < xMax; x++)
                {
                    if (Network.NeuronPositionMap[y, x] != null
                        && Network.NeuronPositionMap[y, x] != neuron)
                    {
                        var distance = Extensions.GetDistance(referenceX, referenceY, x, y);
                        if (minDistance > distance)
                        {
                            minDistance = distance;
                        }
                    }
                }
            }

            return minDistance;
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
