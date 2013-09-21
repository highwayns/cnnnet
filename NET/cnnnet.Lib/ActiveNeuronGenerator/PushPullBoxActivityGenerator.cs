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

        private bool _boxPosition;
        private IEnumerable<Tuple<Neuron, Neuron>> _inputOutputNeuronBindings;

        #endregion

        #region Methods

        public IEnumerable<Neuron> GetActiveNeurons()
        {
            // update box position
            if (_inputOutputNeuronBindings.Take(3).All(binding => binding.Item2.IsActive))
            {
                _boxPosition = false;
            }
            else if (_inputOutputNeuronBindings.Skip(3).Take(3).All(binding => binding.Item2.IsActive))
            {
                _boxPosition = true;
            }

            // return
            return _boxPosition == false
                ? _inputOutputNeuronBindings.Skip(3).Take(3).Select(binding => binding.Item1).ToArray()
                : _inputOutputNeuronBindings.Take(3).Select(binding => binding.Item1).ToArray();
        }

        #endregion

        #region Instance

        public PushPullBoxActivityGenerator(IEnumerable<Tuple<Neuron, Neuron>> inputOutputNeuronBindings)
        {
            Contract.Requires<ArgumentNullException>(inputOutputNeuronBindings != null);

            _inputOutputNeuronBindings = inputOutputNeuronBindings;
        }

        #endregion
    }
}
