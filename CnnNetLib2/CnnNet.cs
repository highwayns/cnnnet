using System;
using System.Collections.Generic;
using System.Linq;

namespace CnnNetLib2
{
    public class CnnNet
    {
        #region Fields

        private readonly Random _random;
        private readonly object _isProcessingSyncObject;
        private bool _isProcessing;

        private Neuron[] _neurons;
        private Neuron[] _activeNeurons;
        private Neuron[] _inputNeurons;

        public readonly int Width;
        public readonly int Height;

        public double[,] NeuronDesirabilityMap;

        #endregion

        #region Properties

        public NetworkParameters NetworkParameters
        {
            get;
            private set;
        }

        public IActiveNeuronGenerator ActiveNeuronGenerator
        {
            get;
            set;
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

            foreach (var neuron in _neurons)
            {
                neuron.Process();
            }

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
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    NeuronDesirabilityMap[y, x] = Math.Max(0, NeuronDesirabilityMap[y, x] - NetworkParameters.DesirabilityDecayAmount);
                }
            }
        }

        public void SetNetworkParameters(NetworkParameters networkParameters)
        {
            lock (_isProcessingSyncObject)
            {
                NetworkParameters = networkParameters;
            }
        }

        public void GenerateNetwork()
        {
            NeuronDesirabilityMap = new double[Height, Width];

            #region Generate Random Neurons

            var neurons = new List<Neuron>();
            for (int i = 0; i < NetworkParameters.NeuronCount; i++)
            {
                var neuron = new Neuron(i, this);

                do
                {
                    neuron.PosY = _random.Next(Height);
                    neuron.PosX = _random.Next(Width);
                }
                while (neurons.Any(n => n.PosX == neuron.PosX && n.PosY == neuron.PosY));

                neurons.Add(neuron);
            }

            _neurons = neurons.ToArray();

            #endregion

            #region Generate Input Neurons

            var inputNeurons = new List<Neuron>();
            for (int i = 0; i < NetworkParameters.InputNeuronCount; i++)
            {
                Neuron neuron;
                do
                {
                    neuron = neurons[_random.Next(_neurons.Length)];
                } 
                while (inputNeurons.Any(inpNeuron => inpNeuron == neuron));
                neuron.HasReachedFinalPosition = true;
                inputNeurons.Add(neuron);
            }
            _inputNeurons = inputNeurons.ToArray();

            #endregion

            ActiveNeuronGenerator =
                new SequentialActiveInputNeuronGenerator
                    (InputNeurons.Select(inpNeuron => inpNeuron.Id).ToArray(),
                     Math.Min(InputNeurons.Length, 2));
        }

        #endregion

        #region Instance

        public CnnNet(int width, int height, NetworkParameters networkParameters)
        {
            _random=new Random();
            Width = width;
            Height = height;

            _isProcessingSyncObject = new object();

            SetNetworkParameters(networkParameters);

            GenerateNetwork();
        }

        #endregion
    }
}
