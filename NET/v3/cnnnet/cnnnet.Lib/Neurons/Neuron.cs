using cnnnet.Lib.GuidanceForces;
using cnnnet.Lib.GuidanceForces.Soma;
using cnnnet.Lib.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace cnnnet.Lib.Neurons
{
    public class Neuron
    {
        #region Fields

        public readonly List<Point> AxonWaypoints;
        private int _activityScore;
        public Point AxonTerminal;
        protected CnnNet _network;
        protected int _id;
        protected bool _isActive;
        protected int _iterationsSinceLastActivation;
        protected double _movedDistance;
        protected int _posX;
        protected int _posY;
        private bool _hasAxonReachedFinalPosition;

        protected IEnumerable<AxonGuidanceForceBase> AxonGuidanceForces;
        protected IEnumerable<SomaGuidanceForceBase> SomaGuidanceForces;

        #endregion Fields

        #region Properties

        public CnnNet CnnNet
        {
            get
            {
                return _network;
            }
        }

        public bool HasAxonReachedFinalPosition
        {
            get
            {
                return _hasAxonReachedFinalPosition;
            }
            protected set
            {
                _hasAxonReachedFinalPosition = value;
                if (_hasAxonReachedFinalPosition)
                {
                    _network.RegisterAxonWaypoints(this, AxonWaypoints);
                }
            }
        }

        public bool HasSomaReachedFinalPosition
        {
            get;
            private set;
        }

        public int Id
        {
            get
            {
                return _id;
            }
        }

        public bool WasActive
        {
            get
            {
                return _network.NeuronActivityHistory.Last().Contains(this);
            }
        }

        /// <summary>
        /// Returns true if the neuron is active in the current iteration
        /// </summary>
        public bool IsActive
        {
            get
            {
                return _isActive;
            }
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
            get
            {
                return _posX;
            }
        }

        public int PosY
        {
            get
            {
                return _posY;
            }
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
            get
            {
                return _movedDistance;
            }
        }

        #endregion Properties

        #region Methods

        public void MoveTo(int newPosY, int newPosX)
        {
            _movedDistance += Extensions.GetDistance(newPosX, newPosY, _posX, _posY);

            _network.NeuronPositionMap[PosY, PosX] = null;
            _network.NeuronPositionMap[newPosY, newPosX] = this;

            _posX = newPosX;
            _posY = newPosY;

            OnMoveTo(newPosY, newPosX);
        }

        public void OnMoveTo(int newPosY, int newPosX)
        {
            AxonWaypoints[0] = new Point(newPosX, newPosY);
        }

        public void Process()
        {
            if (BreakOnProcessCall)
            {
                Debugger.Break();
            }

            if (HasSomaReachedFinalPosition)
            {
                ProcessSomaHasReachedFinalPosition();
            }
            else
            {
                #region Neuron searches for better position

                bool movedToHigherDesirability = false;
                if (_movedDistance < _network.MaxNeuronMoveDistance)
                {
                    movedToHigherDesirability = ProcessGuideSoma();
                }

                HasSomaReachedFinalPosition = _movedDistance > _network.MaxNeuronMoveDistance
                    || (_movedDistance > 0 && movedToHigherDesirability == false);

                _iterationsSinceLastActivation++;
                AddUndesirability();

                #endregion Neuron searches for better position
            }
        }

        public void SetIsActive(bool isActive)
        {
            _isActive = isActive;
            _activityScore = 0;
        }

        protected void AddDesirability()
        {
            AddProportionalRangedValue
                (AxonTerminal.X, AxonTerminal.Y,
                _network.NeuronDesirabilityMap, _network.Width, _network.Height,
                _network.NeuronDesirabilityInfluenceRange,
                _network.NeuronDesirabilityMaxInfluence);
        }

        protected void AddProportionalRangedValue(int coordX, int coordY, double[,] map, int width, int height, int influencedRange, double maxValue)
        {
            int xMin = Math.Max(coordX - influencedRange, 0);
            int xMax = Math.Min(coordX + influencedRange, width);
            int yMin = Math.Max(coordY - influencedRange, 0);
            int yMax = Math.Min(coordY + influencedRange, height);

            for (int y = yMin; y < yMax; y++)
            {
                for (int x = xMin; x < xMax; x++)
                {
                    var distance = Extensions.GetDistance(coordX, coordY, x, y);

                    var influenceByRange = Math.Max(influencedRange - distance, 0);

                    map[y, x] = (float)Math.Min(1, map[y, x] + influenceByRange / influencedRange * maxValue);
                }
            }
        }

        protected void AddUndesirability()
        {
            AddProportionalRangedValue
                (PosX, PosY, _network.NeuronUndesirabilityMap, _network.Width, _network.Height,
                _network.NeuronUndesirabilityInfluenceRange,
                _network.NeuronUndesirabilityMaxInfluence * Math.Max(1, _iterationsSinceLastActivation / _network.NeuronUndesirabilityMaxIterationsSinceLastActivation));
        }

        protected bool ProcessGuideSoma()
        {
            Point maxLocation;
            double maxScore;
            SomaGuidanceForces.Select(somaGuidanceForce => somaGuidanceForce.GetScore(PosY, PosX, this)).Sum().
                GetMaxAndLocation(out maxLocation, out maxScore);

            maxLocation = new Point
                (Math.Min(Math.Max(maxLocation.X - _network.SomaGuidanceForceSearchPlainRange + PosX, 0), _network.Width - 1),
                Math.Min(Math.Max(maxLocation.Y - _network.SomaGuidanceForceSearchPlainRange + PosY, 0), _network.Height - 1));

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
            Point lastMaxLocation = AxonWaypoints.Last();

            Point maxLocation;
            double maxScore;
            AxonGuidanceForces.Select
                (axonGuidanceForce => axonGuidanceForce.GetScore(lastMaxLocation.Y, lastMaxLocation.X, this)).Sum().
                GetMaxAndLocation(out maxLocation, out maxScore);

            maxLocation = new Point
                (Math.Min(Math.Max(maxLocation.X - _network.AxonGuidanceForceSearchPlainRange + lastMaxLocation.X, 0), _network.Width - 1),
                Math.Min(Math.Max(maxLocation.Y - _network.AxonGuidanceForceSearchPlainRange + lastMaxLocation.Y, 0), _network.Height - 1));

            bool result = false;

            double lastMaxLocationScore = AxonGuidanceForces.Select(axonGuidanceForce => axonGuidanceForce.ComputeScoreAtLocation(lastMaxLocation.X, lastMaxLocation.Y, this)).Sum();

            if (maxScore > lastMaxLocationScore
                || AxonWaypoints.Count == 1)
            {
                AxonWaypoints.Add(maxLocation);
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Processes the HasReachedFinalPosition = true branch
        /// </summary>
        private void ProcessSomaHasReachedFinalPosition()
        {
            if (HasAxonReachedFinalPosition)
            {
                #region Determine if neuron is active

                if(_isActive == false)
                {
                    _activityScore +=
                        Extensions.GetNeuronsWithAxonTerminalWithinRange(PosX, PosY, _network, _network.NeuronDendricTreeRange).
                        Where(neuronsWithAxonTerminalWithinRange => neuronsWithAxonTerminalWithinRange.WasActive).Count() 
                        * _network.NeuronActivityScoreMultiply;

                    if (_activityScore >= _network.NeuronIsActiveMinimumActivityScore)
                    {
                        // add neuron to activation list
                        SetIsActive(true);
                    }
                    else
                    {
                        // decay activity score
                        ActivityScore = Math.Max(0, ActivityScore - _network.NeuronActivityScoreDecayAmount);
                    }
                }

                #endregion

                #region Increase region desirability if neuron fires

                if (IsActive)
                {
                    AddDesirability();
                    _iterationsSinceLastActivation = 0;
                }

                #endregion Increase region desirability if neuron fires

                #region Else increase region UN-desirability

                else
                {
                    _iterationsSinceLastActivation++;
                    AddUndesirability();
                }

                #endregion Else increase region UN-desirability
            }
            else
            {
                #region navigate axon to higher undesirability

                HasAxonReachedFinalPosition = ProcessGuideAxon() == false
                                            && AxonWaypoints.Count > 1;

                if (HasAxonReachedFinalPosition
                    && AxonTerminal == null)
                {
                    AxonTerminal = new Point(AxonWaypoints.Last().X, AxonWaypoints.Last().Y);
                }

                #endregion
            }
        }

        public void ResetMovedDistance()
        {
            _movedDistance = 0;
        }

        #endregion Methods

        #region Instance

        public Neuron(int id, CnnNet cnnNet, 
            IEnumerable<AxonGuidanceForceBase> axonGuidanceForces, 
            IEnumerable<SomaGuidanceForceBase> somaGuidanceForces,
            bool isInputNeuron = false)
        {
            _id = id;
            _network = cnnNet;
            AxonWaypoints = new List<Point>
            {
                new Point(PosX, PosY)
            };
            AxonGuidanceForces = axonGuidanceForces;
            SomaGuidanceForces = somaGuidanceForces;

            HasSomaReachedFinalPosition = isInputNeuron;
            IsInputNeuron = isInputNeuron;
        }

        #endregion Instance
    }
}