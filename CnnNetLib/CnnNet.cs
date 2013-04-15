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

        private int[,] _tableNeurons;
        private double[,] _tableNeuronDesirability;
        private int[] _activeNeurons;

        private int _neuronCount;
        private Random _random;

        private readonly ParallelOptions _parallelOptions;

        private readonly double _neuronDensity;
        private readonly int _neuronInfluenceRange;
        private readonly double _maxNeuronInfluence;
        private readonly double _desirabilityDecayAmount;
        private readonly double _percentActiveNourons;

        #endregion

        #region Properties

        public int[,] TableNeurons
        {
            get
            {
                return _tableNeurons;
            }
        }

        public int[] ActiveNeurons
        {
            get
            {
                return _activeNeurons;
            }
        }

        public double[,] TableNeuronDesirability
        {
            get
            {
                return _tableNeuronDesirability;
            }
        }

        #endregion

        #region Methods

        public void ProcessNext()
        {
            _activeNeurons = GetActiveNeurons();

            //Parallel.For(0, _tableHeight, _parallelOptions, y =>
            for (int y = 0; y < _tableHeight; y++)
            {
                for (int x = 0; x < _tableWide; x++)
                {
                    if (_activeNeurons.Contains(_tableNeurons[y, x]))
                    {
                        AddDesirability(y, x);
                    }
                    DecayDesirability(y, x);
                }
            }
            //});
        }

        private int[] GetActiveNeurons()
        {
            var retCount = (int)(_neuronCount * _percentActiveNourons);
            int[] ret = new int[retCount];
            for (int i = 0; i < retCount; i++)
            {
                ret[i] = _random.Next(_neuronCount);
            }
            return ret;
        }

        private void AddDesirability(int y_center, int x_center)
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

        private void DecayDesirability(int y, int x)
        {
            _tableNeuronDesirability[y, x] = Math.Max(0, _tableNeuronDesirability[y, x] - _desirabilityDecayAmount);
        }

        #endregion

        #region Instance

        public CnnNet(int width, int height, double neuronDensity, int neuronInfluenceRange, 
            double maxNeuronInfluence, double desirabilityDecayAmount, double percentActiveNourons)
        {
            _tableWide = width;
            _tableHeight = height;

            _neuronDensity = neuronDensity;
            _neuronInfluenceRange = neuronInfluenceRange;
            _maxNeuronInfluence = maxNeuronInfluence;
            _desirabilityDecayAmount = desirabilityDecayAmount;
            _percentActiveNourons = percentActiveNourons;

            _tableNeurons = new int[_tableHeight, _tableWide];
            _tableNeuronDesirability = new double[_tableHeight, _tableWide];
            _parallelOptions = new ParallelOptions()
            {
                MaxDegreeOfParallelism = 3
            };

            _random = new Random();
            int neuronId = 1;

            // generate random neurons
            for (int y = 0; y < _tableHeight; y++)
            {
                for (int x = 0; x < _tableWide; x++)
                {
                    if (_random.NextDouble() <= _neuronDensity)
                    {
                        _tableNeurons[y, x] = neuronId++;
                    }
                }
            }

            _neuronCount = neuronId;
        }

        #endregion
    }
}
