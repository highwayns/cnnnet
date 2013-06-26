﻿using cnnnet.Lib.Neurons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cnnnet.Lib.AxonGuidanceForces
{
    public interface IAxonGuidanceForce
    {
        double[,] GetScore(NeuronBase neuron, CnnNet network);
    }
}