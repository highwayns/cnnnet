using cnnnet.Lib.Neurons;
using System;
using System.Linq;
using System.Collections.Generic;

namespace cnnnet.Lib.ActiveNeuronGenerator
{
    public class RandomActiveNeuronGenerator : IActiveNeuronGenerator
    {
        #region Fields

        private readonly List<Neuron> _availableNeurons;
        private readonly double _percentActiveNeurons;
        private readonly Random _random;

        #endregion Fields

        #region Methods

        public IEnumerable<Neuron> GetActiveNeurons()
        {
            var resultCount = (int)(_availableNeurons.Count * _percentActiveNeurons);
            var result = new Neuron[resultCount];
            for (int i = 0; i < resultCount; i++)
            {
                result[i] = _availableNeurons[_random.Next(_availableNeurons.Count)];
            }
            return result;
        }

        #endregion Methods

        #region Instance

        public RandomActiveNeuronGenerator(IEnumerable<Neuron> availableNeurons, double percentActiveNeurons)
        {
            _availableNeurons = availableNeurons.ToList();
            _percentActiveNeurons = percentActiveNeurons;
            _random = new Random();
        }

        #endregion Instance
    }
}