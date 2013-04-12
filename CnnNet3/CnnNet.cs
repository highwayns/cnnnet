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
        private readonly int _neuronInfluenceRange;

        private int[,] _tableNeurons;
        private double[,] _tableNeuronDesirability;

        #endregion

        #region Instance

        public CnnNet(int tableWide, double neuronDensity, int neuronInfluenceRange)
        {
            _tableWide = tableWide;
            _neuronDensity = neuronDensity;
            _neuronInfluenceRange = neuronInfluenceRange;

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
                        AddDesirability(i, j);
                    }
                }
            }

            PrintAll();
        }

        private void PrintAll()
        {
            Console.WriteLine("_tableNeurons");
            for (int i = 0; i < _tableWide; i++)
            {
                for (int j = 0; j < _tableWide; j++)
                {
                    Console.Write("{0:00} ", _tableNeurons[i, j]);
                }
                Console.WriteLine();
            }

            Console.WriteLine("_tableNeuronDesirability");
            for (int i = 0; i < _tableWide; i++)
            {
                for (int j = 0; j < _tableWide; j++)
                {
                    Console.Write("{0:#.##} ", _tableNeuronDesirability[i, j]);
                }
                Console.WriteLine();
            }
        }

        private void AddDesirability(int x, int y)
        {
            int x_min = Math.Max(x - _neuronInfluenceRange, 0);
            int x_max = Math.Min(x + _neuronInfluenceRange, _tableWide);
            int y_min = Math.Max(y - _neuronInfluenceRange, 0);
            int y_max = Math.Min(y + _neuronInfluenceRange, _tableWide);

            for (int i = x_min; i < x_max; i++)
            {
                for (int j = y_min; j < y_max; j++)
                {
                    var distance = Math.Sqrt( (x - i)^2 + (y - j)^2);

                    _tableNeuronDesirability[i, j] += (_neuronInfluenceRange - distance) * (1.0 / _neuronInfluenceRange);
                }
            }
        }

        #endregion
    }
}
