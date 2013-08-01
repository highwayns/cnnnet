using cnnnet.Lib.Neurons;
using System;

namespace cnnnet.Lib.GuidanceForces
{
    public class GuidanceForceScoreEventArgs : EventArgs
    {
        #region Properties

        public GuidanceForceBase GuidanceForce
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

        public Neuron Neuron
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

        public GuidanceForceScoreEventArgs
            (GuidanceForceBase guidanceForce, int refY, int refX, Neuron neuron, double[,] score)
        {
            GuidanceForce = guidanceForce;
            RefY = refY;
            RefX = refX;
            Neuron = neuron;
            Score = score;
        }

        #endregion
    }
}
