using cnnnet.Lib.GuidanceForces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cnnnet.Lib.Neurons
{
    public class NeuronAxonGuidanceForcesScoreEventArgs : EventArgs
    {
        #region Properties
        
        public IEnumerable<GuidanceForceScoreEventArgs> Scores
        {
            get;
            private set;
        }

        #endregion

        #region Instance

        public NeuronAxonGuidanceForcesScoreEventArgs(IEnumerable<GuidanceForceScoreEventArgs> scores)
        {
            Scores = scores;
        }

        #endregion
    }
}
