using cnnnet.Lib.ActiveNeuronGenerator;
using cnnnet.Lib.AxonGuidanceForces;
using cnnnet.Lib.Neurons;
using cnnnet.Lib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace cnnnet.Lib
{
    public partial class CnnNet
    {
        #region Fields

        private readonly Random _random;
        private readonly object _isProcessingSyncObject;
        private bool _isProcessing;

        private NeuronBase[] _neurons;
        private NeuronInput[] _neuronsInput;

        public readonly int Width;
        public readonly int Height;

        public NeuronBase[,] NeuronPositionMap;
        public double[,] NeuronDesirabilityMap;
        public double[,] NeuronUndesirabilityMap;

        /// <summary>
        /// Neuron activity recorded from the last 'NeuronActivityHistoryLength' iterations
        /// </summary>
        public List<NeuronBase[]> NeuronActivityHistory;
        
        private int _iteration;

        #endregion Fields

        #region Properties

        public IActiveNeuronGenerator ActiveNeuronGenerator
        {
            get;
            set;
        }

        public NeuronBase[] Neurons
        {
            get
            {
                return _neurons;
            }
        }

        public int Iteration
        {
            get { return _iteration; }
        }

        #endregion Properties

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

            #endregion PreCheck

            _iteration++;

            ProcessDetermineActiveNeurons();

            RecordNeuronalActivity();

            foreach (var neuron in _neurons)
            {
                neuron.Process();
            }

            ProcessDecayDesirabilityAndUndesirability();

            #region End

            lock (_isProcessingSyncObject)
            {
                _isProcessing = false;
            }

            #endregion End
        }

        private void RecordNeuronalActivity()
        {
            if (NeuronActivityHistory.Count == NeuronActivityHistoryLength)
            {
                NeuronActivityHistory.RemoveAt(0);
            }

            NeuronActivityHistory.Add(_neurons.Where(neuron => neuron.IsActive).ToArray());
        }

        private void ProcessDetermineActiveNeurons()
        {
            foreach (var neuron in _neuronsInput)
            {
                neuron.SetIsActive(false);
            }

            foreach (int activeNeuronId in ActiveNeuronGenerator.GetActiveNeuronIds())
            {
                _neurons[activeNeuronId].SetIsActive(true);
            }

            var activeComputeNeurons = new List<NeuronCompute>();
            foreach (var neuron in _neurons.OfType<NeuronCompute>().ToList())
            {
                neuron.ActivityScore +=
                    Extensions.GetNeuronsWithAxonTerminalWithinRange(neuron.PosX, neuron.PosY, this, NeuronDendricTreeRange).
                    Where(neuronsWithAxonTerminalWithinRange => neuronsWithAxonTerminalWithinRange.IsActive).Count() * NeuronActivityScoreMultiply;

                if (neuron.ActivityScore >= NeuronIsActiveMinimumActivityScore)
                {
                    // add neuron to activation list
                    activeComputeNeurons.Add(neuron);
                    neuron.ActivityScore = 0;
                }
                else
                {
                    // decay activity score
                    neuron.ActivityScore = Math.Max(0, neuron.ActivityScore - NeuronActivityScoreDecayAmount);
                }
            }

            foreach (var computeNeuron in _neurons.OfType<NeuronCompute>().ToList())
            {
                computeNeuron.SetIsActive(activeComputeNeurons.Contains(computeNeuron));
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
            NeuronPositionMap = new NeuronBase[Height, Width];

            var axonGuidanceForces = new IAxonGuidanceForce[]
                {
                new UndesirabilityMapAxonGuidanceForce(),
                new DesirabilityMapAxonGuidanceForce()
                };

            #region Generate Random Neurons

            var neurons = new List<NeuronBase>();
            for (int i = 0; i < NeuronCount + InputNeuronCount; i++)
            {
                var neuron = i < NeuronCount
                    ? (NeuronBase)(new NeuronCompute(i, this, axonGuidanceForces))
                    : (new NeuronInput(i, this, axonGuidanceForces));

                do
                {
                    neuron.MoveTo(_random.Next(Height), _random.Next(Width));
                }
                while (neurons.Any(n => n.PosX == neuron.PosX && n.PosY == neuron.PosY));

                neurons.Add(neuron);
                NeuronPositionMap[neuron.PosY, neuron.PosX] = neuron;
            }

            _neurons = neurons.ToArray();

            #endregion Generate Random Neurons

            _neuronsInput = neurons.OfType<NeuronInput>().ToArray();

            ActiveNeuronGenerator =
                new SequentialActiveInputNeuronGenerator
                    (_neuronsInput.Select(neuron => neuron.Id).ToArray(),
                     Math.Min(_neuronsInput.Length, 2));
            _iteration = 0;
        }

        #endregion Methods

        #region Instance

        public CnnNet(int width, int height)
        {
            _random = new Random();
            Width = width;
            Height = height;
            NeuronActivityHistory = new List<NeuronBase[]>();

            _isProcessingSyncObject = new object();

            GenerateNetwork();
        }

        #endregion Instance
    }
}