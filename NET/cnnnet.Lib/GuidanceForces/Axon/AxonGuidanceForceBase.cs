using cnnnet.Lib.Neurons;
using cnnnet.Lib.Utils;
using System;

using System.Linq;

namespace cnnnet.Lib.GuidanceForces.Axon
{
    public abstract class AxonGuidanceForceBase : GuidanceForceBase
    {
        #region Methods

        protected override bool PreCheckLocation(int x, int y, Neuron neuron)
        {
            return base.PreCheckLocation(x, y, neuron)
                && GetDistanceFromOtherWayPoints(x, y) > Network.AxonMinDistanceToOtherWayPoints;
        }

        private double GetDistanceFromOtherWayPoints(int xPos, int yPos)
        {
            int range = Network.AxonGuidanceForceSearchPlainRange + 1;
            double result = range;

            int xMin = Math.Max(xPos - range, 0);
            int xMax = Math.Min(xPos + range, Network.Width);
            int yMin = Math.Max(yPos - range, 0);
            int yMax = Math.Min(yPos + range, Network.Height);

            for (int y = yMin; y < yMax; y++)
            {
                for (int x = xMin; x < xMax; x++)
                {
                    if (Network.NeuronAxonWayPoints[y, x] == null)
                    {
                        continue;
                    }

                    var distance = Extensions.GetDistance(xPos, yPos, x, y);
                    if (distance < result)
                    {
                        result = distance;
                    }
                }
            }

            return result;
        }

        #endregion

        #region Instance

        protected AxonGuidanceForceBase(CnnNet network)
            : base(network.AxonGuidanceForceSearchPlainRange, network)
        {
            Contract.Requires<ArgumentNullException>(network != null);
        }

        #endregion
    }
}