using cnnnet.Lib.Neurons;
using System;

namespace cnnnet.Lib.GuidanceForces.Axon
{
    public class AxonGuidanceForcesSumEventArgs : EventArgs
    {
        #region Properties

        public Neuron Neuron
        {
            get;
            private set;
        }

        public int RefY
        {
            get;
            private set;
        }

        public int RefX
        {
            get;
            private set;
        }

        public double[,] Score
        {
            get;
            private set;
        }

        #endregion

        #region Instance
        
        public AxonGuidanceForcesSumEventArgs(Neuron neuron, int refY, int refX, double[,] score)
        {
            Neuron = neuron;
            RefY = refY;
            RefX = refX;
            Score = score;
        }
        
        #endregion
    }
}
