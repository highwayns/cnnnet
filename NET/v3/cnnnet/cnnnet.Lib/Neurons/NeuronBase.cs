﻿using cnnnet.Lib.AxonGuidanceForces;
using cnnnet.Lib.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace cnnnet.Lib.Neurons
{
    public abstract class NeuronBase
    {
        #region Fields

        public readonly List<Point> AxonWaypoints;
        public int ActivityScore;
        public Point AxonTerminal;
        protected CnnNet _network;
        protected int _id;
        protected bool _isActive;
        protected int _iterationsSinceLastActivation;
        protected double _movedDistance;
        protected int _posX;
        protected int _posY;
        protected IEnumerable<IAxonGuidanceForce> AxonGuidanceForces;

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
            get;
            protected set;
        }

        public bool HasSomaReachedFinalPosition
        {
            get;
            protected set;
        }

        public int Id
        {
            get
            {
                return _id;
            }
        }

        public bool IsActive
        {
            get
            {
                return _isActive;
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

        #endregion Properties

        #region Methods

        public void MoveTo(int newPosY, int newPosX)
        {
            _network.NeuronPositionMap[PosY, PosX] = null;
            _network.NeuronPositionMap[newPosY, newPosX] = this;

            _posX = newPosX;
            _posY = newPosY;

            OnMoveTo(newPosY, newPosX);
        }

        public void OnMoveTo(int newPosY, int newPosX)
        {
            AxonWaypoints[0] = new Point()
            {
                X = newPosX,
                Y = newPosY
            };

            OnMoveToInternal(newPosY, newPosX);
        }

        public void Process()
        {
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
                    movedToHigherDesirability = ProcessMoveToHigherDesirability();
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

        /// <summary>
        /// Remember to optimize this by using spiral matrix processing
        /// http://pastebin.com/4EYJvv5X
        /// </summary>
        /// <param name="referenceY"></param>
        /// <param name="referenceX"></param>
        /// <returns></returns>
        protected double GetDistanceToNearestNeuron(int referenceY, int referenceX)
        {
            double minDistance = _network.NeuronDesirabilityInfluenceRange + 1;

            int xMin = Math.Max(referenceX - _network.MinDistanceBetweenNeurons, 0);
            int xMax = Math.Min(referenceX + _network.MinDistanceBetweenNeurons, _network.Width - 1);
            int yMin = Math.Max(referenceY - _network.MinDistanceBetweenNeurons, 0);
            int yMax = Math.Min(referenceY + _network.MinDistanceBetweenNeurons, _network.Height - 1);

            for (int y = yMin; y <= yMax; y++)
            {
                for (int x = xMin; x < xMax; x++)
                {
                    if (_network.NeuronPositionMap[y, x] != null
                        && _network.NeuronPositionMap[y, x] != this)
                    {
                        var distance = Extensions.GetDistance(referenceX, referenceY, x, y);
                        if (minDistance > distance)
                        {
                            minDistance = distance;
                        }
                    }
                }
            }

            return minDistance;
        }

        protected virtual void OnMoveToInternal(int newPosY, int newPosX)
        {
        }

        protected bool ProcessMoveToHigherDesirability()
        {
            bool ret = false;

            int minCoordX = Math.Max(PosX - _network.NeuronDesirabilityInfluenceRange, 0);
            int maxCoordX = Math.Min(PosX + _network.NeuronDesirabilityInfluenceRange, _network.Width - 1);

            int minCoordY = Math.Max(PosY - _network.NeuronDesirabilityInfluenceRange, 0);
            int maxCoordY = Math.Min(PosY + _network.NeuronDesirabilityInfluenceRange, _network.Height - 1);

            int maxDesirabX = PosX;
            int maxDesirabY = PosY;
            double maxDesirabMovedDistance = 0;
            double maxDesirability = _network.NeuronDesirabilityMap[PosY, PosX];

            for (int y = minCoordY; y < maxCoordY; y++)
            {
                for (int x = minCoordX; x < maxCoordX; x++)
                {
                    if (x == PosX && y == PosY)
                    {
                        continue;
                    }

                    if (_network.NeuronDesirabilityMap[y, x] > maxDesirability
                        && Extensions.GetNeuronAt(y, x, _network) == null
                        && _movedDistance + Extensions.GetDistance(PosX, PosY, x, y) < _network.MaxNeuronMoveDistance
                        && GetDistanceToNearestNeuron(y, x) >= _network.MinDistanceBetweenNeurons
                        && Extensions.GetDistance(PosX, PosY, x, y) <= _network.NeuronDesirabilityInfluenceRange /* this ensures that we only check within the range */)
                    {
                        maxDesirabX = x;
                        maxDesirabY = y;
                        maxDesirability = _network.NeuronDesirabilityMap[y, x];
                        maxDesirabMovedDistance = Extensions.GetDistance(PosX, PosY, x, y);
                    }
                }
            }

            if (PosX != maxDesirabX
                && PosY != maxDesirabY)
            {
                MoveTo(maxDesirabY, maxDesirabX);
                _movedDistance += maxDesirabMovedDistance;

                ret = true;
            }

            return ret;
        }

        private double DistanceFromPreviousWaypoints(int y, int x)
        {
            if (AxonWaypoints.Count == 0)
            {
                return float.MaxValue;
            }

            return AxonWaypoints.Select(waypoint => Extensions.GetDistance(x, y, waypoint.X, waypoint.Y)).Min();
        }

        private bool ProcessGuideAxon()
        {
            Point lastMaxLocation = AxonWaypoints.Last();

            Point maxLocation = AxonGuidanceForces.
                Select(axonGuidanceForce => axonGuidanceForce.GetScore(this, _network)).Sum().GetMaxLocation();

            maxLocation.X += Math.Max(lastMaxLocation.X - _network.AxonGuidanceForceSearchPlainRange, 0);
            maxLocation.Y += Math.Max(lastMaxLocation.Y - _network.AxonGuidanceForceSearchPlainRange, 0);

            bool result = false;

            if (_network.NeuronUndesirabilityMap[maxLocation.Y, maxLocation.X] > _network.NeuronUndesirabilityMap[lastMaxLocation.Y, lastMaxLocation.X])
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
                // navigate axon to higher undesirability
                if (HasAxonReachedFinalPosition == false)
                {
                    HasAxonReachedFinalPosition = ProcessGuideAxon() == false
                                                && AxonWaypoints.Count > 1;
                }

                if (HasAxonReachedFinalPosition
                    && AxonTerminal.X == 0
                    && AxonTerminal.Y == 0)
                {
                    AxonTerminal = new Point
                    {
                        X = AxonWaypoints.Last().X,
                        Y = AxonWaypoints.Last().Y
                    };
                }
            }
        }

        #endregion Methods

        #region Instance

        protected NeuronBase(int id, CnnNet cnnNet, IEnumerable<IAxonGuidanceForce> axonGuidanceForces)
        {
            _id = id;
            _network = cnnNet;
            AxonWaypoints = new List<Point>
            {
                new Point
                {
                    X = PosX,
                    Y = PosY
                }
            };
            AxonGuidanceForces = axonGuidanceForces;
        }

        #endregion Instance
    }
}