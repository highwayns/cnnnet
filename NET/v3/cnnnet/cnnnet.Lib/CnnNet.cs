using cnnnet.Lib.ActiveNeuronGenerator;
using cnnnet.Lib.GuidanceForces;
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

        private Neuron[] _neurons;
        private Neuron[] _neuronsInput;

        public readonly int Width;
        public readonly int Height;

        public Neuron[,] NeuronPositionMap;
        public double[,] NeuronDesirabilityMap;
        public double[,] NeuronUndesirabilityMap;
        public NeuronAxonWaypoint[,] NeuronAxonWaypoints;

        /// <summary>
        /// Neuron activity recorded from the last 'NeuronActivityHistoryLength' iterations
        /// The last item in the list represents the most recent neuron activity
        /// </summary>
        public List<Neuron[]> NeuronActivityHistory;
        
        private int _iteration;

        #endregion Fields

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

            ProcessResetActiveNeurons();

            foreach (var neuron in _neurons)
            {
                neuron.Process();
            }

            RecordNeuronalActivity();

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

        private void ProcessResetActiveNeurons()
        {
            foreach (var neuron in _neuronsInput)
            {
                neuron.SetIsActive(false);
            }

            foreach (var activeNeuron in ActiveNeuronGenerator.GetActiveNeurons())
            {
                activeNeuron.SetIsActive(true);
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
            NeuronPositionMap = new Neuron[Height, Width];
            NeuronAxonWaypoints = new NeuronAxonWaypoint[Height, Width];

            var axonGuidanceForces = new AxonGuidanceForceBase[]
                {
                new UndesirabilityMapAxonGuidanceForce(this),
                new DesirabilityMapAxonGuidanceForce(this)
                };

            #region Generate Random Neurons

            var neurons = new List<Neuron>();
            for (int i = 0; i < NeuronCount + InputNeuronCount; i++)
            {
                var neuron = new Neuron(i, this, axonGuidanceForces, i >= NeuronCount);

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

            _neuronsInput = neurons.GetRange(NeuronCount,InputNeuronCount).ToArray();

            ActiveNeuronGenerator = new SequentialActiveInputNeuronGenerator(_neuronsInput, Math.Min(_neuronsInput.Length, 2));
            _iteration = 0;
        }

        public void RegisterAxonWaypoints(Neuron neuron, IEnumerable<Point> axonWaypoints)
        {
            var axonWaypointsList = axonWaypoints.ToList();

            for (int axonWaypointIndex = 0; axonWaypointIndex < axonWaypointsList.Count; axonWaypointIndex++)
            {
                var axonWaypoint = axonWaypointsList[axonWaypointIndex];
                NeuronAxonWaypoints[axonWaypoint.Y, axonWaypoint.X] = new NeuronAxonWaypoint(axonWaypointIndex,neuron, axonWaypoint);
            }
        }

        #endregion Methods

        #region Instance

        public CnnNet(int width, int height)
        {
            _random = new Random();
            Width = width;
            Height = height;
            NeuronActivityHistory = new List<Neuron[]>();

            _isProcessingSyncObject = new object();

            GenerateNetwork();
        }

        #endregion Instance
    }
}