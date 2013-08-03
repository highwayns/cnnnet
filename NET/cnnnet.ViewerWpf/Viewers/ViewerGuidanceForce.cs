using cnnnet.Lib.GuidanceForces;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cnnnet.ViewerWpf.Viewers
{
    public class ViewerGuidanceForce : ViewerBase
    {
        #region Fields
        
        private readonly GuidanceForceBase _guidanceForce;
        private GuidanceForceScoreEventArgs _latestGuidanceForceScoreEventArgs;

        #endregion

        #region Methods

        protected override void UpdateDataInternal(ref byte[,] data)
        {
            var guidanceForceScoreEventArgs = _latestGuidanceForceScoreEventArgs;
            if (guidanceForceScoreEventArgs == null)
            {
                return;
            }

            var score = guidanceForceScoreEventArgs.Score;

            for (int y = 0; y < _guidanceForce.GuidanceForceHeight; y++)
            {
                for (int x = 0; x < _guidanceForce.GuidanceForceWidth; x++)
                {
                    data[y, x] = (byte)(Math.Min(Math.Max(score[y, x], 0), 1) * 255);
                }
            }
        }

        private void OnGuidanceForceScoreAvailableEvent(object sender, GuidanceForceScoreEventArgs e)
        {
            _latestGuidanceForceScoreEventArgs = e;
        }

        #endregion

        #region Instance

        public ViewerGuidanceForce(GuidanceForceBase guidanceForce)
            : base(guidanceForce.GuidanceForceWidth, guidanceForce.GuidanceForceHeight, true)
        {
            Contract.Requires<ArgumentNullException>(guidanceForce != null);

            _guidanceForce = guidanceForce;
            _guidanceForce.ScoreAvailableEvent += OnGuidanceForceScoreAvailableEvent;
        }

        #endregion
    }
}
