using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cnnnet2
{
    public class CnnNet
    {
        #region Fields

        private readonly int _tableWide;
        private readonly double _neuronDensity;

        private int[,] _tableNeurons;
        private double[,] _tableNeuronDesirability;

        #endregion

        #region Instance

        public CnnNet(int tableWide, double neuronDensity)
        {
            _tableWide = tableWide;
            _neuronDensity = neuronDensity;

            _tableNeurons = new int[_tableWide, _tableWide];
            _tableNeuronDesirability = new double[_tableWide, _tableWide];

            var random = new Random();
            int neuronId = 1;

            for (int i = 0; i < tableWide; i++)
            {
                for (int j = 0; j < tableWide; j++)
                {
                    if (random.NextDouble() <= _neuronDensity)
                    {
                        _tableNeurons[i, j] = neuronId++;
                    }
                }
            }
        }

        #endregion
    }
}
