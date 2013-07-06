using cnnnet.Lib.Neurons;
using cnnnet.Lib.Utils;
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
            var result = GetInitialisedResult(network.AxonGuidanceForceSearchPlainRange);

            int lastPosX = neuron.AxonWaypoints.Last().X;
            int lastPosY = neuron.AxonWaypoints.Last().Y;

            int minCoordX = Math.Max(lastPosX - network.AxonGuidanceForceSearchPlainRange, 0);
            int maxCoordX = Math.Min(lastPosX + network.AxonGuidanceForceSearchPlainRange, network.Width - 1);

            int minCoordY = Math.Max(lastPosY - network.AxonGuidanceForceSearchPlainRange, 0);
            int maxCoordY = Math.Min(lastPosY + network.AxonGuidanceForceSearchPlainRange, network.Height - 1);

            for (int y = minCoordY; y < maxCoordY; y++)
            {
                for (int x = minCoordX; x < maxCoordX; x++)
                {
                    // Desirability at position [y, x] is 0 (zero)
                    if (PreCheckLocation(neuron, network, x, y) == false)
                    {
                        continue;
                    }

                    // only check in the desired radius
                    var distance = 0.0d;
                    if ((x == neuron.PosX && y == neuron.PosY)
                        || (x == lastPosX && y == lastPosY)
                        || Extensions.GetNeuronAt(y, x, network) != null
                        || (distance = Extensions.GetDistance(lastPosX, lastPosY, x, y)) > network.AxonGuidanceForceSearchPlainRange /* this ensures that we only check within the range */
                        || GetDistanceFromPreviousWaypoints(y, x, neuron) < network.AxonMinDistanceToPreviousWaypoints)
                    {
                        continue;
                    }

                    result[y - minCoordY, x - minCoordX] = ComputeScoreAtLocation(neuron, network, new Point(x, y));
                }
            }

            return result;
        }

        private double[,] GetInitialisedResult(int range)
        {
            var result = new double[2 * range, 2 * range];

            for (int y = 0; y < result.GetLength(0); y++)
            {
                for (int x = 0; x < result.GetLength(1); x++)
                {
                    result[y, x] = -1000;
                }
            }

            return result;
        }

        protected virtual bool PreCheckLocation(Neuron neuron, CnnNet network, int x, int y)
        {
            return true;
        }

        public abstract double ComputeScoreAtLocation(Neuron neuron, CnnNet network, Point location);

        private double GetDistanceFromPreviousWaypoints(int y, int x, Neuron neuron)
        {
            if (neuron.AxonWaypoints.Count == 0)
            {
                return float.MaxValue;
            }

            return neuron.AxonWaypoints.Select(waypoint => Extensions.GetDistance(x, y, waypoint.X, waypoint.Y)).Min();
        }
    }
}