using cnnnet.Lib.Neurons;
using System.Collections.Generic;
namespace cnnnet.Lib.ActiveNeuronGenerator
{
    public interface IActiveNeuronGenerator
    {
        IEnumerable<Neuron> GetActiveNeurons();
    }
}