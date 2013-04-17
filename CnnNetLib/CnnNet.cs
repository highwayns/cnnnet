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

        private int[,] _tableNeurons;
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
        private readonly int _minDistanceBetweenNeurons;

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
                        && _activeNeurons.Any(activeNeuron => _tableNeurons[y, x] == activeNeuron))
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
            var auxTableNeurons = new int[_tableNeurons.GetLength(0), _tableNeurons.GetLength(1)];

            for (int y = 0; y < _tableHeight; y++)
            {
                for (int x = 0; x < _tableWide; x++)
                {
                    if (_tableNeurons[y, x] != 0)
                    {
                        MoveNeuronInDesirabilityPlain(y, x, auxTableNeurons);
                    }
                }
            }

            _tableNeurons = auxTableNeurons;
        }

        private void MoveNeuronInDesirabilityPlain(int neuronY, int neuronX, int[,] auxTableNeurons)
        {
            int minCoordX = Math.Max(neuronX - _neuronDesirabilityPlainRange, 0);
            int maxCoordX = Math.Min(neuronX + _neuronDesirabilityPlainRange, _tableWide - 1);

            int minCoordY = Math.Max(neuronY - _neuronDesirabilityPlainRange, 0);
            int maxCoordY = Math.Min(neuronY + _neuronDesirabilityPlainRange, _tableHeight - 1);

            int maxDesirabX = neuronX;
            int maxDesirabY = neuronY;
            double maxDesirability = _tableNeuronDesirability[neuronY, neuronX];

            for (int y = minCoordY; y < maxCoordY; y++)
            {
                for (int x = minCoordX; x < maxCoordX; x++)
                {
                    if (x == neuronX && y == neuronY)
                    {
                        continue;
                    }

                    if (_tableNeuronDesirability[y, x] > maxDesirability
                        && _tableNeurons[y, x] == 0
                        && DistanceToNearestNeuron(y, x) > _minDistanceBetweenNeurons)
                    {
                        maxDesirabX = x;
                        maxDesirabY = y;
                        maxDesirability = _tableNeuronDesirability[y, x];
                    }
                }
            }

            auxTableNeurons[maxDesirabY, maxDesirabX] = _tableNeurons[neuronY, neuronX];
        }

        private double DistanceToNearestNeuron(int neuronY, int neuronX)
        {
            double distanceToNearestNeuron = double.MaxValue;

            int xMin = Math.Max(neuronX - _minDistanceBetweenNeurons, 0);
            int xMax = Math.Min(neuronX + _minDistanceBetweenNeurons, _tableWide - 1);
            int yMin = Math.Max(neuronY - _minDistanceBetweenNeurons, 0);
            int yMax = Math.Min(neuronY + _minDistanceBetweenNeurons, _tableHeight - 1);

            for (int y = yMin; y < yMax; y++)
            {
                for (int x = xMin; x < xMax; x++)
                {
                    if (_tableNeurons[y, x] != 0)
                    {
                        var distance = Math.Sqrt(Math.Pow(neuronX - x, 2) + Math.Pow(neuronY - y, 2));
                        if (distanceToNearestNeuron > distance)
                        {
                            distanceToNearestNeuron = distance;
                        }
                    }
                }
            }

            return distanceToNearestNeuron;
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
            int neuronDesirabilityPlainRange, int minDistanceBetweenNeurons)
        {
            _tableWide = width;
            _tableHeight = height;

            _neuronDensity = neuronDensity;
            _neuronInfluenceRange = neuronInfluenceRange;
            _maxNeuronInfluence = maxNeuronInfluence;
            _desirabilityDecayAmount = desirabilityDecayAmount;
            _percentActiveNourons = percentActiveNourons;
            _neuronDesirabilityPlainRange = neuronDesirabilityPlainRange;
            _minDistanceBetweenNeurons = minDistanceBetweenNeurons;

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
