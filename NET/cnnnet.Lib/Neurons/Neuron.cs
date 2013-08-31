using cnnnet.Lib.GuidanceForces;
using cnnnet.Lib.GuidanceForces.Axon;
using cnnnet.Lib.GuidanceForces.Soma;
using cnnnet.Lib.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

using System.Linq;

namespace cnnnet.Lib.Neurons
{
    [DebuggerDisplay("Id={Id} PosX={PosX} PosY={PosY}")]
    public class Neuron
    {
        #region Fields

        private readonly Random _random;

        public readonly List<NeuronAxonWaypoint> AxonWayPoints;
        private int _activityScore;
        public Point AxonTerminal;
        private CnnNet Network;
        private int IterationsSinceLastActivation;
        private bool _hasAxonReachedFinalPosition;
        private List<DendricSynapse> _synapses;

        #endregion Fields

        #region Properties

        public CnnNet CnnNet
        {
            get
            {
                return Network;
            }
        }

        public event EventHandler<NeuronAxonGuidanceForcesScoreEventArgs> AxonGuidanceForcesScoreEvent;

        public event EventHandler<AxonGuidanceForcesSumEventArgs> AxonGuidanceForcesSumEvent;

        public ReadOnlyCollection<AxonGuidanceForceBase> AxonGuidanceForces
        {
            get;
            private set;
        }

        public ReadOnlyCollection<SomaGuidanceForceBase> SomaGuidanceForces
        {
            get;
            private set;
        }

        public bool HasAxonReachedFinalPosition
        {
            get
            {
                return _hasAxonReachedFinalPosition;
            }
            private set
            {
                _hasAxonReachedFinalPosition = value;
            }
        }

        public bool HasSomaReachedFinalPosition
        {
            get;
            private set;
        }

        public int Id
        {
            get;
            private set;
        }

        public bool GetWasActive(Neuron exclusiveNeuron)
        {
            return Network.NeuronActivityHistory.Any(item => item.Contains(this)
                                                             && item.Contains(exclusiveNeuron) == false);
        }

        /// <summary>
        /// Returns true if the neuron was active in the previous iterationsBack iterations
        /// </summary>
        /// <param name="iterationsBack">From 0 to NeuronActivityHistoryLength - 1. 0 for the most recent iteration (previous) and 'NeuronActivityHistoryLength - 1' for the most distant iteration </param>
        /// <returns></returns>
        private bool GetWasActive(int iterationsBack, Neuron exclusiveNeuron)
        {
            return Network.NeuronActivityHistory[iterationsBack].Contains(this)
                && Network.NeuronActivityHistory[iterationsBack].Contains(exclusiveNeuron) == false;
        }

        /// <summary>
        /// Returns true if the neuron is active in the current iteration
        /// </summary>
        public bool IsActive
        {
            get;
            private set;
        }

        public int ActivityScore
        {
            get
            {
                return _activityScore;
            }
            private set
            {
                _activityScore = value;
            }
        }

        public int PosX
        {
            get;
            private set;
        }

        public int PosY
        {
            get;
            private set;
        }

        public bool IsInputNeuron
        {
            get;
            private set;
        }

        public bool BreakOnProcessCall
        {
            get;
            set;
        }

        public double MovedDistance
        {
            get;
            private set;
        }

        #endregion Properties

        #region Methods

        public void MoveTo(int newPosY, int newPosX)
        {
            MovedDistance += Extensions.GetDistance(newPosX, newPosY, PosX, PosY);

            Network.NeuronPositionMap[PosY, PosX] = null;
            Network.NeuronPositionMap[newPosY, newPosX] = this;

            PosX = newPosX;
            PosY = newPosY;

            OnMoveTo(newPosY, newPosX);
        }

        public void OnMoveTo(int newPosY, int newPosX)
        {
            AxonWayPoints[0] = new NeuronAxonWaypoint(1, this, new Point(newPosX, newPosY));
        }

        public void Process()
        {
            if (BreakOnProcessCall)
            {
                Debugger.Break();
            }

            if (IsInputNeuron && Network.Iteration < Network.InputNeuronDelayIterationsBeforeExtendingAxon)
            {
                return;
            }

            bool movedToHigherDesirability = false;
            if (HasSomaReachedFinalPosition)
            {
                ProcessSomaHasReachedFinalPosition();
            }
            else
            {
                #region Neuron searches for better position
                
                if (MovedDistance < Network.MaxNeuronMoveDistance)
                {
                    movedToHigherDesirability = ProcessGuideSoma();
                }

                HasSomaReachedFinalPosition = MovedDistance > Network.MaxNeuronMoveDistance
                    || (MovedDistance > 0 && movedToHigherDesirability == false);

                #endregion Neuron searches for better position
            }

            if (movedToHigherDesirability == false)
            {
                if (HasSomaReachedFinalPosition == false
                    ||
                    (HasAxonReachedFinalPosition
                    && IsActive == false))
                {
                    IterationsSinceLastActivation++;
                    AddUndesirability();

                }

                if (HasAxonReachedFinalPosition
                    && IsActive)
                {
                    IterationsSinceLastActivation = 0;
                    AddDesirability();
                }
            }
        }

        public void SetIsActive(bool isActive)
        {
            IsActive = isActive;
            if (isActive)
            {
                ActivityScore = 0;
                IterationsSinceLastActivation = 0;
            }
        }

        private void AddDesirability()
        {
            AddProportionalRangedValue
                (AxonTerminal.X, AxonTerminal.Y,
                Network.NeuronDesirabilityMap, Network.Width, Network.Height,
                Network.NeuronDesirabilityInfluenceRange,
                Network.NeuronDesirabilityMaxInfluence);
        }

        private void AddProportionalRangedValue(int posX, int posY, double[,] map, int width, int height, int influencedRange, double maxValue)
        {
            int xMin = Math.Max(posX - influencedRange, 0);
            int xMax = Math.Min(posX + influencedRange, width);
            int yMin = Math.Max(posY - influencedRange, 0);
            int yMax = Math.Min(posY + influencedRange, height);

            for (int y = yMin; y < yMax; y++)
            {
                for (int x = xMin; x < xMax; x++)
                {
                    var distance = Extensions.GetDistance(posX, posY, x, y);

                    var influenceByRange = Math.Max(influencedRange - distance, 0);

                    map[y, x] = (float)Math.Min(1, map[y, x] + influenceByRange / influencedRange * maxValue);
                }
            }
        }

        private void AddUndesirability()
        {
            AddProportionalRangedValue
                (PosX, PosY, Network.NeuronUndesirabilityMap, Network.Width, Network.Height,
                Network.NeuronUndesirabilityInfluenceRange,
                Network.NeuronUndesirabilityMaxInfluence * Math.Max(1, IterationsSinceLastActivation / Network.NeuronUndesirabilityMaxIterationsSinceLastActivation));
        }

        private bool ProcessGuideSoma()
        {
            IEnumerable<Point> maxLocations;
            double maxScore;
            SomaGuidanceForces.Select(somaGuidanceForce => somaGuidanceForce.GetScore(PosY, PosX, this)).Sum().
                GetMaxAndLocation(out maxLocations, out maxScore);

            Point maxLocation = maxLocations.ElementAt(_random.Next(maxLocations.Count()));

            maxLocation = new Point
                (Math.Min(Math.Max(maxLocation.X - Network.SomaGuidanceForceSearchPlainRange + PosX, 0), Network.Width - 1),
                Math.Min(Math.Max(maxLocation.Y - Network.SomaGuidanceForceSearchPlainRange + PosY, 0), Network.Height - 1));

            bool result = false;

            double lastMaxLocationScore = SomaGuidanceForces.Select(somaGuidanceForce => somaGuidanceForce.ComputeScoreAtLocation(PosX, PosY, this)).Sum();

            if (maxScore > lastMaxLocationScore)
            {
                MoveTo(maxLocation.Y, maxLocation.X);
                result = true;
            }

            return result;
        }

        private bool ProcessGuideAxon()
        {
            NeuronAxonWaypoint lastAxonWaypoint = AxonWayPoints.Last();
            Point lastMaxLocation = lastAxonWaypoint.Waypoint;

            Point maxLocation;
            double maxScore;
            var guidanceForceScores =
                AxonGuidanceForces.Select
                (axonGuidanceForce => new GuidanceForceScoreEventArgs(axonGuidanceForce, 
                    lastMaxLocation.Y, lastMaxLocation.X, this, 
                    axonGuidanceForce.GetScore(lastMaxLocation.Y, lastMaxLocation.X, this))).ToArray();

            var guidanceForceScoresSum = guidanceForceScores.Select(guidanceForceScore => guidanceForceScore.Score).Sum();

            IEnumerable<Point> maxLocations;
            guidanceForceScoresSum.GetMaxAndLocation(out maxLocations, out maxScore);

            bool result = false;
            if (maxScore > 0)
            {
                maxLocation = maxLocations.ElementAt(_random.Next(maxLocations.Count()));

                maxLocation = new Point
                    (Math.Min(Math.Max(maxLocation.X - Network.AxonGuidanceForceSearchPlainRange + lastMaxLocation.X, 0), Network.Width - 1),
                    Math.Min(Math.Max(maxLocation.Y - Network.AxonGuidanceForceSearchPlainRange + lastMaxLocation.Y, 0), Network.Height - 1));

                double lastMaxLocationScore = AxonGuidanceForces.Select(axonGuidanceForce => axonGuidanceForce.ComputeScoreAtLocation(lastMaxLocation.X, lastMaxLocation.Y, this)).Sum();

                if (maxScore > lastMaxLocationScore
                    || AxonWayPoints.Count == 1)
                {
                    var newAxonWaypoint = new NeuronAxonWaypoint(lastAxonWaypoint.Id + 1, this, maxLocation);
                    AxonWayPoints.Add(newAxonWaypoint);
                    Network.NeuronAxonWayPoints[maxLocation.Y, maxLocation.X] = newAxonWaypoint;
                    result = true;
                }

                InvokeAxonGuidanceForcesScoreEvent(guidanceForceScores);
                InvokeAxonGuidanceForcesSumEvent(guidanceForceScoresSum);
            }

            return result;
        }

        private void InvokeAxonGuidanceForcesScoreEvent(IEnumerable<GuidanceForceScoreEventArgs> args)
        {
            var handler = AxonGuidanceForcesScoreEvent;
            if (handler != null)
            {
                handler(this, new NeuronAxonGuidanceForcesScoreEventArgs(args));
            }
        }

        private void InvokeAxonGuidanceForcesSumEvent(double[,] score)
        {
            var handler = AxonGuidanceForcesSumEvent;
            if (handler != null)
            {
                handler(this, new AxonGuidanceForcesSumEventArgs(this, 
                    AxonWayPoints.Last().Waypoint.Y, AxonWayPoints.Last().Waypoint.X, score));
            }
        }

        /// <summary>
        /// Processes the HasReachedFinalPosition = true branch
        /// </summary>
        private void ProcessSomaHasReachedFinalPosition()
        {
            if (HasAxonReachedFinalPosition)
            {
                if (IsInputNeuron)
                {
                    return;
                }

                #region Determine if neuron is active

                SetIsActive(false);

                #region Add new neurons to synapses

                _synapses.AddRange
                    (Extensions.GetNeuronsWithAxonTerminalWithinRange(PosX, PosY, Network, Network.NeuronDendricTreeRange).
                    Where(neuron => _synapses.Any(synapse => synapse.PreSynapticNeuron == neuron) == false).
                    Select(neuron => new DendricSynapse
                    {
                        PreSynapticNeuron = neuron,
                        Strength = 0.0f
                    }));

                #endregion

                if (IterationsSinceLastActivation >= Network.NeuronActivityIdleIterations)
                {
                    #region Increase ActivityScore with active synapses count

                    // Filter only the connected synapses to previously active neurons
                    var prevConnectedActiveSynapses =
                        _synapses.Where(synapse => synapse.Strength >= Network.NeuronSynapseConnectedMinimumStrength
                                                   && synapse.PreSynapticNeuron.GetWasActive(this)).ToList();

                    // increase neuron activity by the amount of active synapses
                    ActivityScore = prevConnectedActiveSynapses.Count();

                    #endregion

                    #region Determine if neuron is active

                    if (ActivityScore >= Network.NeuronIsActiveMinimumActivityScore)
                    {
                        // add neuron to activation list
                        SetIsActive(true);
                        ActivityScore = 0;
                        prevConnectedActiveSynapses.ForEach(synapse => synapse.Strength = Math.Min(synapse.Strength + Network.NeuronSynapseStrengthChangeAmount, 1));
                    }
                    else
                    {
                        #region increase the strength of every synapse that fires

                        _synapses.Where(synapse => synapse.PreSynapticNeuron.GetWasActive(0, this)).ToList().
                            ForEach(synapse => synapse.Strength = Math.Min(synapse.Strength + Network.NeuronSynapseStrengthChangeAmount, 1));

                        #endregion
                    }

                    #endregion
                }
                else
                {
                    #region decrease the strength of synapses that fire after the spike (during NeuronActivityIdleIterations)

                    for (int iterationBack = 0; iterationBack < Network.NeuronActivityIdleIterations - IterationsSinceLastActivation; iterationBack++)
                    {
                        _synapses.Where(synapse => synapse.PreSynapticNeuron.GetWasActive(iterationBack, this)).ToList().
                            ForEach(synapse => synapse.Strength = Math.Max(synapse.Strength - Network.NeuronSynapseStrengthChangeAmount, 0));
                    }

                    #endregion
                }

                #endregion
            }
            else
            {
                #region navigate axon to higher undesirability

                HasAxonReachedFinalPosition = ProcessGuideAxon() == false
                                            && AxonWayPoints.Count > 1;

                if (HasAxonReachedFinalPosition
                    && AxonTerminal == null)
                {
                    AxonTerminal = new Point(AxonWayPoints.Last().Waypoint.X, AxonWayPoints.Last().Waypoint.Y);
                }

                #endregion
            }
        }

        public void ResetMovedDistance()
        {
            MovedDistance = 0;
        }

        #endregion Methods

        #region Instance

        public Neuron(int id, CnnNet cnnNet, 
            IEnumerable<AxonGuidanceForceBase> axonGuidanceForces, 
            IEnumerable<SomaGuidanceForceBase> somaGuidanceForces,
            bool isInputNeuron = false)
        {
            Contract.Requires<ArgumentException>(id >= 0);
            Contract.Requires<ArgumentNullException>(cnnNet != null);
            Contract.Requires<ArgumentNullException>(axonGuidanceForces != null);
            Contract.Requires<ArgumentNullException>(somaGuidanceForces != null);

            Id = id;
            Network = cnnNet;
            AxonWayPoints = new List<NeuronAxonWaypoint>
            {
                new NeuronAxonWaypoint(1, this, new Point(PosX, PosY))
            };
            AxonGuidanceForces = new ReadOnlyCollection<AxonGuidanceForceBase>(axonGuidanceForces.ToList());
            SomaGuidanceForces = new ReadOnlyCollection<SomaGuidanceForceBase>(somaGuidanceForces.ToList());

            HasSomaReachedFinalPosition = isInputNeuron;
            IsInputNeuron = isInputNeuron;

            _synapses = new List<DendricSynapse>();
            _random = new Random();
        }

        #endregion Instance
    }
}