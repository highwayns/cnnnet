using cnnnet.Lib.Neurons;
using cnnnet.Lib.Utils;
using System.Linq;

namespace cnnnet.Lib.GuidanceForces.Axon
{
    public abstract class AxonGuidanceForceBase : GuidanceForceBase
    {
        #region Methods

        protected override bool PostCheckLocation(int x, int y, Neuron neuron)
        {
            return base.PostCheckLocation(x, y, neuron)
                && GetDistanceFromPreviousWayPoints(x, y, neuron) > Network.AxonMinDistanceToPreviousWayPoints;
        }

        private double GetDistanceFromPreviousWayPoints(int x, int y, Neuron neuron)
        {
            if (neuron.AxonWayPoints.Count == 0)
            {
                return float.MaxValue;
            }

            return neuron.AxonWayPoints.
                Select(wayPoint => Extensions.GetDistance(x, y, wayPoint.X, wayPoint.Y)).Min();
        }

        #endregion

        #region Instance

        protected AxonGuidanceForceBase(CnnNet network)
            : base(network.AxonGuidanceForceSearchPlainRange, network)
        {
        }

        #endregion
    }
}