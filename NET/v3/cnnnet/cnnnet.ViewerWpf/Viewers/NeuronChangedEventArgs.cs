using cnnnet.Lib.Neurons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
