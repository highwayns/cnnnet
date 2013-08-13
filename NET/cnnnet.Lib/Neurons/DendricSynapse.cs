using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cnnnet.Lib.Neurons
{
    public class DendricSynapse
    {
        public Neuron PreSynapticNeuron
        {
            get;
            set;
        }

        public double Strength
        {
            get;
            set;
        }
    }
}
