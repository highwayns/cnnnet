using System;
using System.Collections.Generic;
using System.Linq;

namespace CnnNetLib2
{
    public partial class CnnNet
    {
        #region Fields

        private readonly Random _random;
        private readonly object _isProcessingSyncObject;
        private bool _isProcessing;

        private Neuron[] _neurons;
        private Neuron[] _inputNeurons;

        public readonly int Width;
        public readonly int Height;

        public double[,] NeuronDesirabilityMap;
        public double[,] NeuronUndesirabilityMap;

        #endregion

        #region Properties

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

            ProcessDecayDesirabilityAndUndesirability();

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
            var activeInputNeuronIds = ActiveNeuronGenerator.GetActiveNeuronIds();

            foreach (var neuron in _neurons)
            {
                neuron.IsActive = false;
            }

            for (int i = 0; i < activeInputNeuronIds.Length; i++)
            {
                _neurons[activeInputNeuronIds[i]].IsActive = true;
            }
        }

        private void ProcessDecayDesirabilityAndUndesirability()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    NeuronDesirabilityMap[y, x] = Math.Max(0, NeuronDesirabilityMap[y, x] - DesirabilityDecayAmount);
                    NeuronUndesirabilityMap[y, x] = Math.Max(0, NeuronUndesirabilityMap[y, x] - UndesirabilityDecayAmount);
                }
            }
        }

        public void GenerateNetwork()
        {
            NeuronDesirabilityMap = new double[Height, Width];
            NeuronUndesirabilityMap = new double[Height, Width];

            #region Generate Random Neurons

            var neurons = new List<Neuron>();
            for (int i = 0; i < NeuronCount; i++)
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
            for (int i = 0; i < InputNeuronCount; i++)
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

        public CnnNet(int width, int height)
        {
            _random=new Random();
            Width = width;
            Height = height;

            _isProcessingSyncObject = new object();

            GenerateNetwork();
        }

        #endregion
    }
}
