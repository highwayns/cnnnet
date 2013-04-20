using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CnnNetLib2
{
    public class CnnNet
    {
        #region Fields

        private readonly int _width;
        private readonly int _height;
        private readonly Random _random;

        private double[,] _neuronDesirabilityMap;

        private Neuron[] _neurons;
        private Neuron[] _activeNeurons;
        private Neuron[] _inputNeurons;

        private readonly object _isProcessingSyncObject;

        private int _neuronCount;
        private int _neuronInfluenceRange;
        private double _maxNeuronInfluence;
        private double _desirabilityDecayAmount;
        private int _neuronDesirabilityPlainRange;
        private int _minDistanceBetweenNeurons;
        private int _inputNeuronCount;
        private bool _inputNeuronsMoveToHigherDesirability;
        private int _maxNeuronMoveDistance;

        private bool _isProcessing;

        #endregion

        #region Properties

        public IActiveNeuronGenerator ActiveNeuronGenerator
        {
            get;
            set;
        }

        public double[,] NeuronDesirabilityMap
        {
            get
            {
                return _neuronDesirabilityMap;
            }
        }

        public Neuron[] Neurons
        {
            get
            {
                return _neurons;
            }
        }

        public Neuron[] ActiveNeurons
        {
            get
            {
                return _activeNeurons;
            }
        }

        public Neuron[] InputNeurons
        {
            get
            {
                return _inputNeurons;
            }
        }

        #endregion

        #region Methods

        public void Process()
        {
            #region PreCheck

            lock (_isProcessingSyncObject)
            {
                if (_isProcessing)
                {
                    return;
                }
                _isProcessing = true;
            }

            #endregion

            ProcessDecayDesirability();

            ProcessDetermineActiveNeurons();

            #region End

            lock (_isProcessingSyncObject)
            {
                _isProcessing = false;
            }

            #endregion
        }

        private void ProcessDetermineActiveNeurons()
        {
            var activeNeuronIds = ActiveNeuronGenerator.GetActiveNeuronIds();

            _activeNeurons = new Neuron[activeNeuronIds.Length];

            for (int i = 0; i < activeNeuronIds.Length; i++)
            {
                _activeNeurons[i] = _neurons[activeNeuronIds[i]];
            }
        }

        private void ProcessDecayDesirability()
        {
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    _neuronDesirabilityMap[y, x] = Math.Max(0, _neuronDesirabilityMap[y, x] - _desirabilityDecayAmount);
                }
            }
        }

        public void SetNetworkParameters(NetworkParameters networkParameters)
        {
            lock (_isProcessingSyncObject)
            {
                _neuronCount = networkParameters.NeuronCount;
                _neuronInfluenceRange = networkParameters.NeuronInfluenceRange;
                _maxNeuronInfluence = networkParameters.MaxNeuronInfluence;
                _desirabilityDecayAmount = networkParameters.DesirabilityDecayAmount;
                _neuronDesirabilityPlainRange = networkParameters.NeuronDesirabilityPlainRange;
                _minDistanceBetweenNeurons = networkParameters.MinDistanceBetweenNeurons;
                _inputNeuronCount = networkParameters.InputNeuronCount;
                _inputNeuronsMoveToHigherDesirability = networkParameters.InputNeuronsMoveToHigherDesirability;
                _maxNeuronMoveDistance = networkParameters.MaxNeuronMoveDistance;
            }
        }

        private void GenerateNetwork()
        {
            var neurons = new List<Neuron>();
            for (int i = 0; i < _neuronCount; i++)
            {
                var neuron = new Neuron(i);

                do
                {
                    neuron.PosY = _random.Next(_height);
                    neuron.PosX = _random.Next(_width);
                }
                while (neurons.Any(n => n.PosX == neuron.PosX && n.PosY == neuron.PosY));

                neurons.Add(neuron);
            }

            _neurons = neurons.ToArray();
        }

        #endregion

        #region Instance

        public CnnNet(int width, int height, NetworkParameters networkParameters)
        {
            _random=new Random();
            _width = width;
            _height = height;

            _neuronDesirabilityMap = new double[_height, _width];
            _isProcessingSyncObject = new object();

            ActiveNeuronGenerator = new RandomActiveNeuronGenerator(_neuronCount, 0.1);

            SetNetworkParameters(networkParameters);

            GenerateNetwork();
        }

        #endregion
    }
}
