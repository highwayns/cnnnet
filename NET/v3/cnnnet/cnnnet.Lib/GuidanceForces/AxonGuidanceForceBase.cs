using cnnnet.Lib.Neurons;
using cnnnet.Lib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cnnnet.Lib.GuidanceForces
{
    public abstract class AxonGuidanceForceBase : GuidanceForceBase
    {
        #region Methods

        protected override bool PostCheckLocation(int x, int y, Neuron neuron)
        {
            return base.PostCheckLocation(x, y, neuron)
                && GetDistanceFromPreviousWaypoints(x, y, neuron) > Network.AxonMinDistanceToPreviousWaypoints;
        }

        private double GetDistanceFromPreviousWaypoints(int x, int y, Neuron neuron)
        {
            if (neuron.AxonWaypoints.Count == 0)
            {
                return float.MaxValue;
            }

            return neuron.AxonWaypoints.Select(waypoint => Extensions.GetDistance(x, y, waypoint.X, waypoint.Y)).Min();
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