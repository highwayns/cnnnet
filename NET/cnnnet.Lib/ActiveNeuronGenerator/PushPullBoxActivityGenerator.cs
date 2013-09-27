using cnnnet.Lib.Neurons;
using cnnnet.Lib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cnnnet.Lib.ActiveNeuronGenerator
{
    public class PushPullBoxActivityGenerator : IActiveNeuronGenerator
    {
        #region Fields

        private CnnNet _network;
        private bool _boxPosition;
        private IEnumerable<Tuple<Neuron, Neuron>> _inputOutputNeuronBindings;
        private int _remainingActivations;

        #endregion

        #region Methods

        public IEnumerable<Neuron> GetActiveNeurons()
        {
            Neuron[] retValue = null;

            if (_remainingActivations > 0)
            {
                var activeNeuronBindings = _boxPosition == false
                    ? _inputOutputNeuronBindings.Skip(3).Take(3).ToArray()
                    : _inputOutputNeuronBindings.Take(3).ToArray();

                var retValueList = activeNeuronBindings.Select(activeNeuronBinding => activeNeuronBinding.Item1).ToList();

                retValueList.AddRange(_inputOutputNeuronBindings
                    .Except(activeNeuronBindings)
                    .Select(inactiveNeuronBinding => inactiveNeuronBinding.Item1)
                    .Where(neuron => neuron.IterationsSinceLastActivation >= _network.NeuronInputBaseLineActivation));

                retValue = retValueList.Distinct().ToArray();
            }
            else
            {
                retValue =  _inputOutputNeuronBindings
                    .Select(inputOutputNeuronBinding => inputOutputNeuronBinding.Item1)
                    .Where(neuron => neuron.IterationsSinceLastActivation >= _network.NeuronInputBaseLineActivation).ToArray();
            }

            // update box position
            var newBoxPosition = _boxPosition;
            if (_inputOutputNeuronBindings.Take(3).All(binding => retValue.Contains(binding.Item2)))
            {
                newBoxPosition = false;
            }
            else if (_inputOutputNeuronBindings.Skip(3).Take(3).All(binding => retValue.Contains(binding.Item2)))
            {
                newBoxPosition = true;
            }

            if (newBoxPosition != _boxPosition)
            {
                _remainingActivations = _network.NeuronInputMaxConsecutiveActivations;
            }
            else
            {
                _remainingActivations--;
            }

            return retValue;
        }

        #endregion

        #region Instance

        public PushPullBoxActivityGenerator(CnnNet network, IEnumerable<Tuple<Neuron, Neuron>> inputOutputNeuronBindings)
        {
            Contract.Requires<ArgumentNullException>(network != null);
            Contract.Requires<ArgumentNullException>(inputOutputNeuronBindings != null);

            _network = network;
            _inputOutputNeuronBindings = inputOutputNeuronBindings;

            _remainingActivations = _network.NeuronInputMaxConsecutiveActivations;
        }

        #endregion
    }
}
