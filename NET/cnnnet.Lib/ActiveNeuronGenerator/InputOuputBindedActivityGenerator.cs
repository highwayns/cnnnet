using cnnnet.Lib.Neurons;
using cnnnet.Lib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cnnnet.Lib.ActiveNeuronGenerator
{
    public class InputOuputBindedActivityGenerator : IActiveNeuronGenerator
    {
        #region Fields
        
        private IEnumerable<Tuple<Neuron, Neuron>> _inputOutputNeuronBindings;

        #endregion

        #region Methods

        public IEnumerable<Neuron> GetActiveNeurons()
        {
            return _inputOutputNeuronBindings
                .Where(binding => binding.Item2.IsActive)
                .Select(binding => binding.Item1).ToArray();
        }

        #endregion

        #region Instance

        public InputOuputBindedActivityGenerator(IEnumerable<Tuple<Neuron, Neuron>> inputOutputNeuronBindings)
        {
            Contract.Requires<ArgumentNullException>(inputOutputNeuronBindings != null);

            _inputOutputNeuronBindings = inputOutputNeuronBindings;
        }

        #endregion
    }
}
