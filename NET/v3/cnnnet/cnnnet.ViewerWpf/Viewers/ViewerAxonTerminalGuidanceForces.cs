using cnnnet.Lib.GuidanceForces;
using cnnnet.Lib.Neurons;
using System.Linq;
using cnnnet.Lib.GuidanceForces.Axon;

namespace cnnnet.ViewerWpf.Viewers
{
    public class ViewerAxonTerminalGuidanceForces : ViewerBase
    {
        #region Fields
        
        private Neuron _neuron;
        private AxonGuidanceForcesSumEventArgs _latestAxonGuidanceForcesSum;

        #endregion

        #region Properties

        public Neuron Neuron
        {
            get
            {
                return _neuron;
            }
            set
            {
                UnsubscribeFromNeuronEvents(_neuron);
                _neuron = value;
                SubscribeToNeuronEvents(_neuron);

                IsEnabled = _neuron != null;
            }
        }

        private void SubscribeToNeuronEvents(Neuron neuron)
        {
            if (neuron == null)
            {
                return;
            }

            neuron.AxonGuidanceForces.ToList().
                ForEach(guidanceForce => guidanceForce.ScoreAvailableEvent += OnGuidanceForceScoreAvailableEvent);
            neuron.AxonGuidanceForcesSumEvent += OnNeuronAxonGuidanceForcesSumEvent;
        }

        private void UnsubscribeFromNeuronEvents(Neuron neuron)
        {
            if (neuron == null)
            {
                return;
            }

            neuron.AxonGuidanceForces.ToList().
                ForEach(guidanceForce => guidanceForce.ScoreAvailableEvent -= OnGuidanceForceScoreAvailableEvent);
            neuron.AxonGuidanceForcesSumEvent -= OnNeuronAxonGuidanceForcesSumEvent;
        }

        private void OnGuidanceForceScoreAvailableEvent(object sender, GuidanceForceScoreEventArgs e)
        {
        }

        private void OnNeuronAxonGuidanceForcesSumEvent(object sender, AxonGuidanceForcesSumEventArgs e)
        {
            _latestAxonGuidanceForcesSum = e;
        }

        #endregion

        #region Methods

        protected override void UpdateDataInternal(ref byte[,] data)
        {
            if (_latestAxonGuidanceForcesSum == null)
            {
                return;
            }

            double[,] scoresSum = _latestAxonGuidanceForcesSum.Score;

            int dataMinX = (Width - data.GetLength(1))/2;
            int dataMaxX = dataMinX + data.GetLength(1);
            int dataMinY = (Height - data.GetLength(0)) / 2;
            int dataMaxY = dataMinY + data.GetLength(0);

            for (int y = dataMinY; y < dataMaxY; y++)
            {
                for (int x = dataMinX; x < dataMaxX; x++)
                {
                    data[y, x * 3 + Constants.ColorGreenIndex] = (byte)(scoresSum[y - dataMinY, x - dataMinX] * 255);
                }
            }
        }

        #endregion

        #region Instance
        
        public ViewerAxonTerminalGuidanceForces()
            : base(51, 51, 3, true)
        {

        }

        #endregion
    }
}
