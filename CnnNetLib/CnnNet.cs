using System;
using System.Linq;

namespace CnnNetLib
{
    public class CnnNet
    {
        #region Fields

        private readonly int _tableWide;
        private readonly int _tableHeight;

        private int[,] _tableNeurons;
        private readonly double[,] _tableNeuronDesirability;
        private readonly int[] _inputNeuronIds;
        private readonly double[] _neuronIdsMovedDistance;
        
        private readonly Random _random;

        private int[] _activeNeurons;

        private int _neuronInfluenceRange;
        private double _maxNeuronInfluence;
        private double _desirabilityDecayAmount;
        private double _neuronDensity;
        private int _neuronDesirabilityPlainRange;
        private int _minDistanceBetweenNeurons;
        private int _inputNeuronCount;
        private bool _inputNeuronsMoveToHigherDesirability;
        private int _maxNeuronMoveDistance;

        private bool _isProcessing;
        private readonly object _isProcessingSyncObject;

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

        public int[] InputNeuronIds
        {
            get
            {
                return _inputNeuronIds;
            }
        }

        public int NeuronCount
        {
            get;
            private set;
        }

        public IActiveNeuronGenerator ActiveNeuronGenerator
        {
            get;
            set;
        }

        #endregion

        #region Methods

        public void Process()
        {
            lock (_isProcessingSyncObject)
            {
                if (_isProcessing)
                {
                    return;
                }
                _isProcessing = true;
            }

            _activeNeurons = ActiveNeuronGenerator.GetActiveNeuronIds();

            ProcessUpdateDesirability();
            ProcessEnforceMinDistanceBetweenNeurons();
            ProcessMoveToHigherDesirability();

            lock (_isProcessingSyncObject)
            {
                _isProcessing = false;
            }
        }

        private void ProcessEnforceMinDistanceBetweenNeurons()
        {
            
        }

        private void ProcessUpdateDesirability()
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

        private void ProcessMoveToHigherDesirability()
        {
            var auxTableNeurons = new int[_tableNeurons.GetLength(0), _tableNeurons.GetLength(1)];

            for (int y = 0; y < _tableHeight; y++)
            {
                for (int x = 0; x < _tableWide; x++)
                {
                    if (_tableNeurons[y, x] != 0)
                    {
                        if (_inputNeuronsMoveToHigherDesirability
                            || _inputNeuronIds.All(inputNeuronId => inputNeuronId != _tableNeurons[y, x]))
                        {
                            MoveNeuronInDesirabilityPlain(y, x, auxTableNeurons);
                        }
                        else
                        {
                            auxTableNeurons[y, x] = _tableNeurons[y, x];
                        }
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
                        && _neuronIdsMovedDistance[_tableNeurons[neuronY, neuronX] - 1] < _maxNeuronMoveDistance
                        && GetDistanceToNearestNeuron(x, y, auxTableNeurons) >= _minDistanceBetweenNeurons)
                    {
                        maxDesirabX = x;
                        maxDesirabY = y;
                        maxDesirability = _tableNeuronDesirability[y, x];
                        _neuronIdsMovedDistance[_tableNeurons[neuronY, neuronX] - 1] += GetDistance(neuronX, neuronY, x, y);
                    }
                }
            }

            auxTableNeurons[maxDesirabY, maxDesirabX] = _tableNeurons[neuronY, neuronX];
        }

        private double GetDistanceToNearestNeuron(int referenceX, int referenceY, int[,] auxTableNeurons)
        {
            double distanceToNearestNeuron = _neuronDesirabilityPlainRange + 1;

            int xMin = Math.Max(referenceX - _minDistanceBetweenNeurons, 0);
            int xMax = Math.Min(referenceX + _minDistanceBetweenNeurons, _tableWide - 1);
            int yMin = Math.Max(referenceY - _minDistanceBetweenNeurons, 0);
            int yMax = Math.Min(referenceY + _minDistanceBetweenNeurons, _tableHeight - 1);

            for (int y = yMin; y < yMax; y++)
            {
                for (int x = xMin; x < xMax; x++)
                {
                    if (auxTableNeurons[y, x] != 0)
                    {
                        var distance = GetDistance(referenceX, referenceY, x, y);
                        if (distanceToNearestNeuron > distance)
                        {
                            distanceToNearestNeuron = distance;
                        }
                    }
                }
            }

            return distanceToNearestNeuron;
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
                    var distance = GetDistance(xCenter, yCenter, x, y);

                    _tableNeuronDesirability[y, x] =
                        Math.Min(1, _tableNeuronDesirability[y, x] + Math.Max(_neuronInfluenceRange - distance, 0)*(1.0/_neuronInfluenceRange)*_maxNeuronInfluence);
                }
            }
        }

        private void DecayDesirability(int y, int x)
        {
            _tableNeuronDesirability[y, x] = Math.Max(0, _tableNeuronDesirability[y, x] - _desirabilityDecayAmount);
        }

        private double GetDistance(int x1, int y1, int x2, int y2)
        {
            return Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
        }

        private void SetNetworkParameters(NetworkParameters networkParameters)
        {
            lock (_isProcessingSyncObject)
            {
                _neuronInfluenceRange = networkParameters.NeuronInfluenceRange;
                _maxNeuronInfluence = networkParameters.MaxNeuronInfluence;
                _desirabilityDecayAmount = networkParameters.DesirabilityDecayAmount;
                _neuronDensity = networkParameters.NeuronDensity;
                _neuronDesirabilityPlainRange = networkParameters.NeuronDesirabilityPlainRange;
                _minDistanceBetweenNeurons = networkParameters.MinDistanceBetweenNeurons;
                _inputNeuronCount = networkParameters.InputNeuronCount;
                _inputNeuronsMoveToHigherDesirability = networkParameters.InputNeuronsMoveToHigherDesirability;
                _maxNeuronMoveDistance = networkParameters.MaxNeuronMoveDistance;
            }
        }

        #endregion

        #region Instance

        public CnnNet(int width, int height, NetworkParameters networkParameters)
        {
            _tableWide = width;
            _tableHeight = height;

            _isProcessingSyncObject = new object();

            SetNetworkParameters(networkParameters);

            _tableNeurons = new int[_tableHeight, _tableWide];
            _tableNeuronDesirability = new double[_tableHeight, _tableWide];
            _inputNeuronIds = new int[_inputNeuronCount];
            _random = new Random();

            #region Generate random neurons

            // generate all neurons
            int neuronId = 1;
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

            NeuronCount = neuronId;

            // determine input neurons
            for (int i = 0; i < _inputNeuronCount; i++)
            {
                int inputNeuronId;
                while (_inputNeuronIds.Contains(inputNeuronId = _random.Next(1, NeuronCount + 1)))
                {
                }
                _inputNeuronIds[i] = inputNeuronId;
            }

            #endregion

            _neuronIdsMovedDistance = new double[NeuronCount];
        }

        #endregion
    }
}
