using System;
using System.Linq;
using System.Threading.Tasks;

namespace CnnNetLib
{
    public class CnnNet
    {
        #region Fields

        private readonly int _tableWide;
        private readonly int _tableHeight;

        private readonly int[,] _tableNeurons;
        private readonly double[,] _tableNeuronDesirability;
        private readonly int _neuronCount;
        private readonly Random _random;

        private int[] _activeNeurons;

        private readonly double _neuronDensity;
        private readonly int _neuronInfluenceRange;
        private readonly double _maxNeuronInfluence;
        private readonly double _desirabilityDecayAmount;
        private readonly double _percentActiveNourons;
        private readonly int _neuronDesirabilityPlainRange;

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

        public void Process()
        {
            _activeNeurons = GetActiveNeurons();

            UpdateDesirability();
            MoveToHigherDesirability();
        }

        private void UpdateDesirability()
        {
            for (int y = 0; y < _tableHeight; y++)
            {
                for (int x = 0; x < _tableWide; x++)
                {
                    if (_tableNeurons[y, x] != 0
                        && _activeNeurons.Contains(_tableNeurons[y, x]))
                    {
                        AddDesirability(y, x);
                    }
                }
            }

            for (int y = 0; y < _tableHeight; y++)
            {
                for (int x = 0; x < _tableWide; x++)
                {
                    DecayDesirability(y, x);
                }
            }
        }

        private void MoveToHigherDesirability()
        {
            for (int y = 0; y < _tableHeight; y++)
            {
                for (int x = 0; x < _tableWide; x++)
                {
                    if (_tableNeurons[y, x] != 0)
                    {
                        MoveNeuronInDesirabilityPlain(y, x);
                    }
                }
            }
        }

        private void MoveNeuronInDesirabilityPlain(int neuronY, int neuronX)
        {
            int minX = Math.Max(0, neuronX - _neuronDesirabilityPlainRange);
            int maxX = Math.Min(_tableWide - 1, neuronX - _neuronDesirabilityPlainRange);
        }

        private int[] GetActiveNeurons()
        {
            var retCount = (int)(_neuronCount * _percentActiveNourons);
            var ret = new int[retCount];
            for (int i = 0; i < retCount; i++)
            {
                ret[i] = _random.Next(_neuronCount);
            }
            return ret;
        }

        private void AddDesirability(int yCenter, int xCenter)
        {
            int xMin = Math.Max(xCenter - _neuronInfluenceRange, 0);
            int xMax = Math.Min(xCenter + _neuronInfluenceRange, _tableWide - 1);
            int yMin = Math.Max(yCenter - _neuronInfluenceRange, 0);
            int yMax = Math.Min(yCenter + _neuronInfluenceRange, _tableHeight - 1);

            for (int y = yMin; y < yMax; y++)
            {
                for (int x = xMin; x < xMax; x++)
                {
                    var distance = Math.Sqrt(Math.Pow(xCenter - x, 2) + Math.Pow(yCenter - y, 2));

                    _tableNeuronDesirability[y, x] =
                        Math.Min(1, _tableNeuronDesirability[y, x] + Math.Max(_neuronInfluenceRange - distance, 0)*(1.0/_neuronInfluenceRange)*_maxNeuronInfluence);
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
            double maxNeuronInfluence, double desirabilityDecayAmount, double percentActiveNourons,
            int neuronDesirabilityPlainRange)
        {
            _tableWide = width;
            _tableHeight = height;

            _neuronDensity = neuronDensity;
            _neuronInfluenceRange = neuronInfluenceRange;
            _maxNeuronInfluence = maxNeuronInfluence;
            _desirabilityDecayAmount = desirabilityDecayAmount;
            _percentActiveNourons = percentActiveNourons;
            _neuronDesirabilityPlainRange = neuronDesirabilityPlainRange;

            _tableNeurons = new int[_tableHeight, _tableWide];
            _tableNeuronDesirability = new double[_tableHeight, _tableWide];
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
