using cnnnet.Lib.ActiveNeuronGenerator;
using cnnnet.Lib.GuidanceForces.Axon;
using cnnnet.Lib.GuidanceForces.Soma;
using cnnnet.Lib.Neurons;
using cnnnet.Lib.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
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
        public NeuronAxonWaypoint[,] NeuronAxonWayPoints;

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

        public IEnumerable<AxonGuidanceForceBase> AxonGuidanceForces
        {
            get;
            private set;
        }

        public IEnumerable<SomaGuidanceForceBase> SomaGuidanceForces
        {
            get;
            private set;
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

        private IEnumerable<SomaGuidanceForceBase> GetSomaGuidanceForces()
        {
            return new SomaGuidanceForceBase[]
                {
                new SomaDesirabilityMapGuidanceForce(this)
                //new SomaUndesirabilityMapGuidanceForce(this)
                };
        }

        private IEnumerable<AxonGuidanceForceBase> GetAxonGuidanceForces()
        {
            return new AxonGuidanceForceBase[]
            {
                new AxonUndesirabilityMapGuidanceForce(this),
                new AxonDesirabilityMapGuidanceForce(this)
            };
        }

        public void GenerateNetwork()
        {
            NeuronDesirabilityMap = new double[Height, Width];
            NeuronUndesirabilityMap = new double[Height, Width];
            NeuronPositionMap = new Neuron[Height, Width];
            NeuronAxonWayPoints = new NeuronAxonWaypoint[Height, Width];

            AxonGuidanceForces = GetAxonGuidanceForces();
            SomaGuidanceForces = GetSomaGuidanceForces();

            #region Generate Random Neurons

            var neurons = new List<Neuron>();
            for (int index = 0; index < NeuronCount + InputNeuronCount; index++)
            {
                var neuron = new Neuron(index, this, AxonGuidanceForces, SomaGuidanceForces, index >= NeuronCount);

                do
                {
                    neuron.MoveTo(_random.Next(Height), _random.Next(Width));
                }
                while (neurons.Any(n => n.PosX == neuron.PosX && n.PosY == neuron.PosY)
                    || Extensions.GetDistanceToNearestNeuron(neuron.PosY, neuron.PosX, neuron, this) <= MinDistanceBetweenNeurons);

                neuron.ResetMovedDistance();
                neurons.Add(neuron);
                NeuronPositionMap[neuron.PosY, neuron.PosX] = neuron;
            }

            _neurons = neurons.ToArray();

            #endregion Generate Random Neurons

            _neuronsInput = neurons.GetRange(NeuronCount, InputNeuronCount).ToArray();

            ActiveNeuronGenerator = new SequentialActiveInputNeuronGenerator(_neuronsInput, Math.Min(_neuronsInput.Length, 4));
            _iteration = 0;
        }

        public void RestartNetwork()
        {
            NeuronDesirabilityMap = new double[Height, Width];
            NeuronUndesirabilityMap = new double[Height, Width];
            NeuronPositionMap = new Neuron[Height, Width];
            NeuronAxonWayPoints = new NeuronAxonWaypoint[Height, Width];

            #region Generate Random Neurons

            var oldNeurons = _neurons.ToList();

            var neurons = new List<Neuron>();
            for (int index = 0; index < oldNeurons.Count; index++)
            {
                var neuron = new Neuron(index, this, AxonGuidanceForces, SomaGuidanceForces, index >= NeuronCount);

                neuron.MoveTo(oldNeurons[index].PosY, oldNeurons[index].PosX);

                neuron.ResetMovedDistance();
                neurons.Add(neuron);
                NeuronPositionMap[neuron.PosY, neuron.PosX] = neuron;
            }

            _neurons = neurons.ToArray();

            #endregion Generate Random Neurons

            _neuronsInput = neurons.GetRange(NeuronCount, InputNeuronCount).ToArray();

            ActiveNeuronGenerator = new SequentialActiveInputNeuronGenerator(_neuronsInput, Math.Min(_neuronsInput.Length, 4));
            _iteration = 0;
        }

        public void RegisterAxonWayPoints(Neuron neuron, IEnumerable<Point> axonWayPoints)
        {
            Contract.Requires<ArgumentNullException>(neuron != null);
            Contract.Requires<ArgumentNullException>(axonWayPoints != null);
            Contract.Requires<ArgumentException>(axonWayPoints.Any());

            var axonWayPointsList = axonWayPoints.ToList();

            for (int axonWayPointIndex = 0; axonWayPointIndex < axonWayPointsList.Count; axonWayPointIndex++)
            {
                var axonWayPoint = axonWayPointsList[axonWayPointIndex];
                NeuronAxonWayPoints[axonWayPoint.Y, axonWayPoint.X] = new NeuronAxonWaypoint(axonWayPointIndex, neuron, axonWayPoint);
            }
        }

        #endregion Methods

        #region Instance

        public CnnNet(int width, int height)
        {
            Contract.Requires<ArgumentOutOfRangeException>(width > 0);
            Contract.Requires<ArgumentOutOfRangeException>(height > 0);
            
            Width = width;
            Height = height;
            _random = new Random();
            NeuronActivityHistory = new List<Neuron[]>();

            _isProcessingSyncObject = new object();

            GenerateNetwork();
        }

        #endregion Instance
    }
}