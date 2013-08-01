using cnnnet.Lib.Neurons;
using cnnnet.Lib.Utils;
using System;

namespace cnnnet.Lib.GuidanceForces
{
    public abstract class GuidanceForceBase
    {
        #region Fields

        protected readonly int GuidanceForceRange;
        protected readonly CnnNet Network;

        #endregion

        #region Properties

        public event EventHandler<GuidanceForceScoreEventArgs> ScoreAvailableEvent;

        #endregion

        #region Methods

        public double[,] GetScore(int refY, int refX, Neuron neuron)
        {
            var result = GetInitializationResult();

            int minX = Math.Max(refX - GuidanceForceRange, 0);
            int maxX = Math.Min(refX + GuidanceForceRange, Network.Width - 1);

            int minY = Math.Max(refY - GuidanceForceRange, 0);
            int maxY = Math.Min(refY + GuidanceForceRange, Network.Height - 1);

            for (int y = minY; y < maxY; y++)
            {
                for (int x = minX; x < maxX; x++)
                {
                    if (PreCheckLocation(x, y, neuron) == false)
                    {
                        continue;
                    }

                    // only check in the desired radius
                    if ((x == refX && y == refY)
                        /* this ensures that we only check within the range */
                        || (Extensions.GetDistance(refX, refY, x, y)) > GuidanceForceRange)
                    {
                        continue;
                    }

                    if (PostCheckLocation(x, y, neuron) == false)
                    {
                        continue;
                    }

                    result[y - minY, x - minX] = ComputeScoreAtLocation(x, y, neuron);
                }
            }

            InvokeScoreAvailableEvent(refY, refX, neuron, result);

            return result;
        }

        private void InvokeScoreAvailableEvent(int refY, int refX, Neuron neuron, double[,] score)
        {
            var handler = ScoreAvailableEvent;
            if (handler != null)
            {
                handler(this, new GuidanceForceScoreEventArgs(this, refY, refX, neuron, score));
            }
        }

        /// <summary>
        /// Perform VERY FAST checks here
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="neuron">The neuron context to check</param>
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
        /// <param name="neuron">The neuron context to check</param>
        /// <returns></returns>
        protected virtual bool PostCheckLocation(int x, int y, Neuron neuron)
        {
            return true;
        }

        public abstract double ComputeScoreAtLocation(int x, int y, Neuron neuron);

        private double[,] GetInitializationResult()
        {
            var result = new double[2 * GuidanceForceRange + 1, 2 * GuidanceForceRange + 1];

            for (int y = 0; y < result.GetLength(0); y++)
            {
                for (int x = 0; x < result.GetLength(1); x++)
                {
                    result[y, x] = 0;
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
