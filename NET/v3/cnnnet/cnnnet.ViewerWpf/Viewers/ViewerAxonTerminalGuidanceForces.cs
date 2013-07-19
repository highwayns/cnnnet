using cnnnet.Lib.GuidanceForces;
using cnnnet.Lib.Neurons;
using System.Linq;
using cnnnet.Lib.Utils;

namespace cnnnet.ViewerWpf.Viewers
{
    public class ViewerAxonTerminalGuidanceForces : ViewerBase
    {
        #region Fields
        
        private Neuron _neuron;
        private NeuronAxonGuidanceForcesScoreEventArgs _latestNeuronAxonGuidanceForcesScores;

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
            neuron.AxonGuidanceForcesScoreEvent += OnNeuronAxonGuidanceForcesScoreAvailable;
        }

        private void UnsubscribeFromNeuronEvents(Neuron neuron)
        {
            if (neuron == null)
            {
                return;
            }

            neuron.AxonGuidanceForces.ToList().
                ForEach(guidanceForce => guidanceForce.ScoreAvailableEvent -= OnGuidanceForceScoreAvailableEvent);
            neuron.AxonGuidanceForcesScoreEvent -= OnNeuronAxonGuidanceForcesScoreAvailable;
        }

        private void OnGuidanceForceScoreAvailableEvent(object sender, GuidanceForceScoreEventArgs e)
        {
        }

        private void OnNeuronAxonGuidanceForcesScoreAvailable(object sender, NeuronAxonGuidanceForcesScoreEventArgs e)
        {
            _latestNeuronAxonGuidanceForcesScores = e;
        }

        #endregion

        #region Methods

        protected override void UpdateDataInternal(ref byte[,] data)
        {
            if (_latestNeuronAxonGuidanceForcesScores == null)
            {
                return;
            }

            double[,] scoresSum = _latestNeuronAxonGuidanceForcesScores.Scores.Select(item => item.Score).Sum();

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
            : base(51, 51, 3, false)
        {

        }

        #endregion
    }
}
