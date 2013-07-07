using cnnnet.Lib.Neurons;
using cnnnet.Lib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cnnnet.Lib.GuidanceForces
{
    public abstract class GuidanceForceBase
    {
        #region Fields

        protected readonly int GuidanceForceRange;
        protected readonly CnnNet Network;

        #endregion

        #region Methods

        public double[,] GetScore(Neuron neuron)
        {
            var result = GetInitialisedResult();

            var location = neuron.AxonWaypoints.Last();

            int minCoordX = Math.Max(location.X - GuidanceForceRange, 0);
            int maxCoordX = Math.Min(location.X + GuidanceForceRange, Network.Width - 1);

            int minCoordY = Math.Max(location.Y - GuidanceForceRange, 0);
            int maxCoordY = Math.Min(location.Y + GuidanceForceRange, Network.Height - 1);

            for (int y = minCoordY; y < maxCoordY; y++)
            {
                for (int x = minCoordX; x < maxCoordX; x++)
                {
                    if (PreCheckLocation(x, y, neuron) == false)
                    {
                        continue;
                    }

                    // only check in the desired radius
                    var distance = 0.0d;
                    if ((x == location.X && y == location.Y)
                        || (distance = Extensions.GetDistance(location.X, location.Y, x, y)) > GuidanceForceRange /* this ensures that we only check within the range */)
                    {
                        continue;
                    }

                    if (PostCheckLocation(x, y, neuron) == false)
                    {
                        continue;
                    }

                    result[y - minCoordY, x - minCoordX] = ComputeScoreAtLocation(x, y, neuron);
                }
            }

            return result;
        }

        /// <summary>
        /// Perform VERY FAST checks here
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        protected virtual bool PreCheckLocation(int x, int y, Neuron neuron)
        {
            return true;
        }

        /// <summary>
        /// Perform SLOW checks here
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        protected virtual bool PostCheckLocation(int x, int y, Neuron neuron)
        {
            return true;
        }

        public abstract double ComputeScoreAtLocation(int x, int y, Neuron neuron);

        private double[,] GetInitialisedResult()
        {
            var result = new double[2 * GuidanceForceRange, 2 * GuidanceForceRange];

            for (int y = 0; y < result.GetLength(0); y++)
            {
                for (int x = 0; x < result.GetLength(1); x++)
                {
                    result[y, x] = -1000;
                }
            }

            return result;
        }

        #endregion

        #region Instance

        protected GuidanceForceBase(int guidanceForceRange, CnnNet network)
        {
            GuidanceForceRange = guidanceForceRange;
            Network = network;
        }

        #endregion
    }
}
