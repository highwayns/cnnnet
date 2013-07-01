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
        private readonly double _percentActiveNourons;
        private readonly Random _random;

        #endregion Fields

        #region Methods

        public IEnumerable<Neuron> GetActiveNeurons()
        {
            var retCount = (int)(_availableNeurons.Count * _percentActiveNourons);
            var result = new Neuron[retCount];
            for (int i = 0; i < retCount; i++)
            {
                result[i] = _availableNeurons[_random.Next(_availableNeurons.Count)];
            }
            return result;
        }

        #endregion Methods

        #region Instance

        public RandomActiveNeuronGenerator(IEnumerable<Neuron> availableNeurons, double percentActiveNourons)
        {
            _availableNeurons = availableNeurons.ToList();
            _percentActiveNourons = percentActiveNourons;
            _random = new Random();
        }

        #endregion Instance
    }
}