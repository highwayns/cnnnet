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
        private int _remainingActivations;
        private IEnumerable<Neuron> _inputNeurons;
        private IEnumerable<Neuron> _outputNeurons;

        #endregion

        #region Methods

        public IEnumerable<Neuron> GetActiveNeurons()
        {
            Neuron[] retValue = null;

            if (_remainingActivations > 0)
            {
                var retValueList = _boxPosition == false
                    ? _inputNeurons.Skip(3).Take(3).ToList()
                    : _inputNeurons.Take(3).ToList();

                retValueList.AddRange(_inputNeurons
                    .Except(retValueList)
                    .Where(neuron => neuron.IterationsSinceLastActivation + 1 /*Add 1 because the neuron isn't active in this iteration also*/
                        >= _network.NeuronInputBaseLineActivation));

                retValue = retValueList.Distinct().ToArray();
            }
            else
            {
                retValue =  _inputNeurons
                    .Where(neuron => neuron.IterationsSinceLastActivation + 1  /*Add 1 because the neuron isn't active in this iteration also*/
                        >= _network.NeuronInputBaseLineActivation).ToArray();
            }

            // update box position
            var newBoxPosition = _boxPosition;
            var shouldMoveToPos1 = _outputNeurons.Take(3).Count(neuron => neuron.IsActive) >= 2;
            var shouldMoveToPos2 = _outputNeurons.Skip(3).Take(3).Count(neuron => neuron.IsActive) >= 2;

            if (shouldMoveToPos1 != shouldMoveToPos2)
            {
                if (shouldMoveToPos1)
                {
                    newBoxPosition = false;
                }
                else if (shouldMoveToPos2)
                {
                    newBoxPosition = true;
                }
            }

            if (_boxPosition != newBoxPosition)
            {
                _remainingActivations = _network.NeuronInputMaxConsecutiveActivations;
                _boxPosition = newBoxPosition;
            }
            else if (_remainingActivations > 0)
            {
                _remainingActivations--;
            }

            return retValue;
        }

        #endregion

        #region Instance

        public PushPullBoxActivityGenerator(CnnNet network, 
            IEnumerable<Neuron> inputNeurons, IEnumerable<Neuron> outputNeurons)
        {
            Contract.Requires<ArgumentNullException>(network != null);
            Contract.Requires<ArgumentNullException>(inputNeurons != null);
            Contract.Requires<ArgumentNullException>(outputNeurons != null);

            _network = network;
            _inputNeurons = inputNeurons;
            _outputNeurons = outputNeurons;

            _remainingActivations = _network.NeuronInputMaxConsecutiveActivations;
        }

        #endregion
    }
}
