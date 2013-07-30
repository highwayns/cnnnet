using cnnnet.Lib.Neurons;
using System.Linq;
using System.Collections.Generic;

namespace cnnnet.Lib.ActiveNeuronGenerator
{
    public class SequentialActiveInputNeuronGenerator : IActiveNeuronGenerator
    {
        #region Fields

        private bool _shouldReturnActive = true;
        private readonly List<Neuron> _availableInputNeuron;
        private readonly int _activeNeuronCount;

        private int _currentIndex;

        #endregion Fields

        #region Methods

        public IEnumerable<Neuron> GetActiveNeurons()
        {
            var result = new List<Neuron>();
            _shouldReturnActive = !_shouldReturnActive;

            if (_shouldReturnActive)
            {
                for (int i = 0; i < _activeNeuronCount; i++)
                {
                    result.Add(_availableInputNeuron[(_currentIndex + i) % _availableInputNeuron.Count]);
                }
                _currentIndex = (_currentIndex + _activeNeuronCount) % _availableInputNeuron.Count;
            }

            return result;
        }

        #endregion Methods

        #region Instance

        public SequentialActiveInputNeuronGenerator(IEnumerable<Neuron> availableInputNeuron, int activeNeuronCount)
        {
            _availableInputNeuron = availableInputNeuron.ToList();
            _activeNeuronCount = activeNeuronCount;
        }

        #endregion Instance
    }
}