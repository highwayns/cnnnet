using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cnnnet.Lib.GuidanceForces
{
    public class GuidanceForceScoreAvailableEventArgs : EventArgs
    {
        #region Properties
        
        public double[,] Score
        {
            get;
            private set;
        }

        #endregion

        #region Instance

        public GuidanceForceScoreAvailableEventArgs(double[,] score)
        {
            Score = score;
        }

        #endregion
    }
}
