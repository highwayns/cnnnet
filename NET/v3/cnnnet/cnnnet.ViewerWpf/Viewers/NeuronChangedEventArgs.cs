using cnnnet.Lib.Neurons;
using System;

namespace cnnnet.ViewerWpf.Viewers
{
    public class NeuronChangedEventArgs : EventArgs
    {
        #region Properties
        
        public Neuron Neuron
        {
            get;
            private set;
        }

        #endregion

        #region Instance

        public NeuronChangedEventArgs(Neuron neuron)
        {
            Neuron = neuron;
        }

        #endregion
    }
}
