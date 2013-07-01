using cnnnet.Lib.Neurons;
using cnnnet.Lib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cnnnet.Lib.AxonGuidanceForces
{
    public class DesirabilityMapAxonGuidanceForce : IAxonGuidanceForce
    {
        public double[,] GetScore(Neuron neuron, CnnNet network)
        {
            var result = new double[2 * network.AxonGuidanceForceSearchPlainRange, 2 * network.AxonGuidanceForceSearchPlainRange];

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
                    if (Math.Abs(network.NeuronDesirabilityMap[y, x] - 0.0d) < 0.00001)
                    {
                        continue;
                    }

                    var distance = 0.0d;
                    if ((x == neuron.PosX && y == neuron.PosY)
                        || (x == lastPosX && y == lastPosY)
                        || Extensions.GetNeuronAt(y, x, network) != null
                        || (distance = Extensions.GetDistance(lastPosX, lastPosY, x, y)) > network.AxonGuidanceForceSearchPlainRange /* this ensures that we only check within the range */)
                    {
                        continue;
                    }

                    if (GetDistanceFromPreviousWaypoints(y, x, neuron) >= network.AxonMinDistanceToPreviousWaypoints)
                    {
                        result[y - minCoordY, x - minCoordX] = -network.NeuronDesirabilityMap[y, x];
                    }
                }
            }

            return result;
        }

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