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

        #region Properties

        public event EventHandler<GuidanceForceScoreAvailableEventArgs> ScoreAvailableEvent;

        #endregion

        #region Methods

        public double[,] GetScore(int refY, int refX, Neuron neuron)
        {
            var result = GetInitialisedResult();

            int minCoordX = Math.Max(refX - GuidanceForceRange, 0);
            int maxCoordX = Math.Min(refX + GuidanceForceRange, Network.Width - 1);

            int minCoordY = Math.Max(refY - GuidanceForceRange, 0);
            int maxCoordY = Math.Min(refY + GuidanceForceRange, Network.Height - 1);

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
                    if ((x == refX && y == refY)
                        /* this ensures that we only check within the range */
                        || (distance = Extensions.GetDistance(refX, refY, x, y)) > GuidanceForceRange)
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

            InvokeScoreAvailableEvent(result);

            return result;
        }

        private void InvokeScoreAvailableEvent(double[,] result)
        {
            var handler = ScoreAvailableEvent;
            if (handler != null)
            {
                handler(this, new GuidanceForceScoreAvailableEventArgs(result));
            }
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
            var result = new double[2 * GuidanceForceRange + 1, 2 * GuidanceForceRange + 1];

            for (int y = 0; y < result.GetLength(0); y++)
            {
                for (int x = 0; x < result.GetLength(1); x++)
                {
                    result[y, x] = -99999;
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
