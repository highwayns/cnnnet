using cnnnet.Lib.GuidanceForces;
using cnnnet.Lib.Neurons;
using cnnnet.Lib.Utils;
using System;


namespace cnnnet.ViewerWpf.Viewers
{
    public class ViewerGuidanceForce : ViewerBase
    {
        #region Fields
        
        private readonly GuidanceForceBase _guidanceForce;
        private readonly int _colorIndex;

        private double[,] _latestScore;

        #endregion

        #region Properties

        public Neuron Neuron
        {
            get;
            set;
        }

        #endregion

        #region Methods

        protected override void UpdateDataInternal(ref byte[,] data)
        {
            var score = _latestScore;
            if (score == null)
            {
                return;
            }

            for (int y = 0; y < _guidanceForce.GuidanceForceHeight; y++)
            {
                for (int x = 0; x < _guidanceForce.GuidanceForceWidth; x++)
                {
                    data[y, x * Constants.BytesPerPixel + _colorIndex] = (byte)(Math.Min(Math.Max(score[y, x], 0), 1) * 255);
                }
            }
        }

        private void OnGuidanceForceScoreAvailableEvent(object sender, GuidanceForceScoreEventArgs e)
        {
            var neuron = e.Neuron;
            if (neuron == Neuron)
            {
                _latestScore = (double[,])e.Score.Clone();
            }
        }

        #endregion

        #region Instance

        public ViewerGuidanceForce(GuidanceForceBase guidanceForce, int colorIndex)
            : base(guidanceForce.GuidanceForceWidth, guidanceForce.GuidanceForceHeight, true)
        {
            Contract.Requires<ArgumentNullException>(guidanceForce != null);
            Contract.Requires<ArgumentOutOfRangeException>(0 <= colorIndex && colorIndex <= 2);

            _guidanceForce = guidanceForce;
            _colorIndex = colorIndex;

            _guidanceForce.ScoreAvailableEvent += OnGuidanceForceScoreAvailableEvent;
        }

        #endregion
    }
}
