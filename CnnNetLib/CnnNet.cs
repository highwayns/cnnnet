using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CnnNetLib
{
    public class CnnNet
    {
        #region Fields

        private readonly int _tableWide;
        private readonly int _tableHeight;

        private readonly double _neuronDensity;
        private readonly int _neuronInfluenceRange;
        private readonly double _maxNeuronInfluence;

        private int[,] _tableNeurons;
        private double[,] _tableNeuronDesirability;

        #endregion

        #region Methods

        public int[,] GetTableNeurons()
        {
            return _tableNeurons;
        }

        public double[,] GetTableNeuronDesirability()
        {
            return _tableNeuronDesirability;
        }

        private void AddDesirability(int x_center, int y_center)
        {
            int x_min = Math.Max(x_center - _neuronInfluenceRange, 0);
            int x_max = Math.Min(x_center + _neuronInfluenceRange, _tableWide);
            int y_min = Math.Max(y_center - _neuronInfluenceRange, 0);
            int y_max = Math.Min(y_center + _neuronInfluenceRange, _tableHeight);

            for (int y = y_min; y < y_max; y++)
            {
                for (int x = x_min; x < x_max; x++)
                {
                    var distance = Math.Sqrt(Math.Pow(x_center - x, 2) + Math.Pow(y_center - y, 2));

                    _tableNeuronDesirability[y, x] =
                        Math.Min(1, _tableNeuronDesirability[y, x] + Math.Max(_neuronInfluenceRange - distance, 0) * (1.0 / _neuronInfluenceRange) * _maxNeuronInfluence);
                }
            }
        }

        public void ProcessNext()
        {
            for (int y = 0; y < _tableHeight; y++)
            {
                for (int x = 0; x < _tableWide; x++)
                {
                    if (_tableNeurons[y, x] != 0)
                    {

                    }

                }
            }
        }

        #endregion

        #region Instance

        public CnnNet(int width, int height, double neuronDensity, int neuronInfluenceRange, double maxNeuronInfluence)
        {
            _tableWide = width;
            _tableHeight = height;

            _neuronDensity = neuronDensity;
            _neuronInfluenceRange = neuronInfluenceRange;
            _maxNeuronInfluence = maxNeuronInfluence;

            _tableNeurons = new int[_tableHeight, _tableWide];
            _tableNeuronDesirability = new double[_tableHeight, _tableWide];

            var random = new Random();
            int neuronId = 1;

            for (int y = 0; y < _tableHeight; y++)
            {
                for (int x = 0; x < _tableWide; x++)
                {
                    if (random.NextDouble() <= _neuronDensity)
                    {
                        _tableNeurons[y, x] = neuronId++;
                        AddDesirability(x, y);
                    }
                }
            }
        }

        #endregion
    }
}
