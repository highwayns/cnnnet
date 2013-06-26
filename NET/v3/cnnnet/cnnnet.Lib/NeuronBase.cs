using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace cnnnet.Lib
{
    public abstract class NeuronBase
    {
        #region Fields

        public readonly List<Point> AxonWaypoints;
        public int ActivityScore;
        public Point AxonTerminal;
        protected CnnNet _cnnNet;
        protected int _id;
        protected bool _isActive;
        protected int _iterationsSinceLastActivation;
        protected double _movedDistance;
        protected int _posX;
        protected int _posY;

        #endregion Fields

        #region Properties

        public CnnNet CnnNet
        {
            get
            {
                return _cnnNet;
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
            _cnnNet.NeuronPositionMap[PosY, PosX] = null;
            _cnnNet.NeuronPositionMap[newPosY, newPosX] = this;

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
                if (_movedDistance < _cnnNet.MaxNeuronMoveDistance)
                {
                    movedToHigherDesirability = ProcessMoveToHigherDesirability();
                }

                HasSomaReachedFinalPosition = _movedDistance > _cnnNet.MaxNeuronMoveDistance
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
                _cnnNet.NeuronDesirabilityMap, _cnnNet.Width, _cnnNet.Height,
                _cnnNet.NeuronDesirabilityInfluenceRange,
                _cnnNet.NeuronDesirabilityMaxInfluence);
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
                (PosX, PosY, _cnnNet.NeuronUndesirabilityMap, _cnnNet.Width, _cnnNet.Height,
                _cnnNet.NeuronUndesirabilityInfluenceRange,
                _cnnNet.NeuronUndesirabilityMaxInfluence * Math.Max(1, _iterationsSinceLastActivation / _cnnNet.NeuronUndesirabilityMaxIterationsSinceLastActivation));
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
            double minDistance = _cnnNet.NeuronDesirabilityInfluenceRange + 1;

            int xMin = Math.Max(referenceX - _cnnNet.MinDistanceBetweenNeurons, 0);
            int xMax = Math.Min(referenceX + _cnnNet.MinDistanceBetweenNeurons, _cnnNet.Width - 1);
            int yMin = Math.Max(referenceY - _cnnNet.MinDistanceBetweenNeurons, 0);
            int yMax = Math.Min(referenceY + _cnnNet.MinDistanceBetweenNeurons, _cnnNet.Height - 1);

            for (int y = yMin; y <= yMax; y++)
            {
                for (int x = xMin; x < xMax; x++)
                {
                    if (_cnnNet.NeuronPositionMap[y, x] != null
                        && _cnnNet.NeuronPositionMap[y, x] != this)
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

            int minCoordX = Math.Max(PosX - _cnnNet.NeuronDesirabilityInfluenceRange, 0);
            int maxCoordX = Math.Min(PosX + _cnnNet.NeuronDesirabilityInfluenceRange, _cnnNet.Width - 1);

            int minCoordY = Math.Max(PosY - _cnnNet.NeuronDesirabilityInfluenceRange, 0);
            int maxCoordY = Math.Min(PosY + _cnnNet.NeuronDesirabilityInfluenceRange, _cnnNet.Height - 1);

            int maxDesirabX = PosX;
            int maxDesirabY = PosY;
            double maxDesirabMovedDistance = 0;
            double maxDesirability = _cnnNet.NeuronDesirabilityMap[PosY, PosX];

            for (int y = minCoordY; y < maxCoordY; y++)
            {
                for (int x = minCoordX; x < maxCoordX; x++)
                {
                    if (x == PosX && y == PosY)
                    {
                        continue;
                    }

                    if (_cnnNet.NeuronDesirabilityMap[y, x] > maxDesirability
                        && Extensions.GetNeuronAt(y, x, _cnnNet) == null
                        && _movedDistance + Extensions.GetDistance(PosX, PosY, x, y) < _cnnNet.MaxNeuronMoveDistance
                        && GetDistanceToNearestNeuron(y, x) >= _cnnNet.MinDistanceBetweenNeurons
                        && Extensions.GetDistance(PosX, PosY, x, y) <= _cnnNet.NeuronDesirabilityInfluenceRange /* this ensures that we only check within the range */)
                    {
                        maxDesirabX = x;
                        maxDesirabY = y;
                        maxDesirability = _cnnNet.NeuronDesirabilityMap[y, x];
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
            int lastPosX = AxonWaypoints.Last().X;
            int lastPosY = AxonWaypoints.Last().Y;

            int minCoordX = Math.Max(lastPosX - _cnnNet.AxonHigherUndesirabilitySearchPlainRange, 0);
            int maxCoordX = Math.Min(lastPosX + _cnnNet.AxonHigherUndesirabilitySearchPlainRange, _cnnNet.Width - 1);

            int minCoordY = Math.Max(lastPosY - _cnnNet.AxonHigherUndesirabilitySearchPlainRange, 0);
            int maxCoordY = Math.Min(lastPosY + _cnnNet.AxonHigherUndesirabilitySearchPlainRange, _cnnNet.Height - 1);

            int maxUndesirabX = lastPosX;
            int maxUndesirabY = lastPosY;

            double maxUndesirability = _cnnNet.NeuronUndesirabilityMap[maxUndesirabY, maxUndesirabX];

            bool axonMoved = false;

            var record = false;
            var recordList = new List<double>();

            for (int y = minCoordY; y < maxCoordY; y++)
            {
                for (int x = minCoordX; x < maxCoordX; x++)
                {
                    // undesirability at position [y, x] is 0 (zero)
                    if (Math.Abs(_cnnNet.NeuronUndesirabilityMap[y, x] - 0.0d) < 0.00001)
                    {
                        continue;
                    }

                    var distance = 0.0d;
                    if ((x == PosX && y == PosY)
                        || (x == maxUndesirabX && y == maxUndesirabY)
                        || (x == lastPosX && y == lastPosY)
                        || Extensions.GetNeuronAt(y, x, _cnnNet) != null
                        || (distance = Extensions.GetDistance(lastPosX, lastPosY, x, y)) > _cnnNet.AxonHigherUndesirabilitySearchPlainRange /* this ensures that we only check within the range */)
                    {
                        continue;
                    }

                    if (record)
                    {
                        recordList.Add(_cnnNet.NeuronUndesirabilityMap[y, x]);
                    }

                    if (_cnnNet.NeuronUndesirabilityMap[y, x] > _cnnNet.NeuronUndesirabilityMap[maxUndesirabY, maxUndesirabX]
                        && DistanceFromPreviousWaypoints(y, x) >= _cnnNet.AxonMinDistanceToPreviousWaypoints)
                    {
                        axonMoved = true;
                        maxUndesirabX = x;
                        maxUndesirabY = y;
                        maxUndesirability = _cnnNet.NeuronUndesirabilityMap[y, x];
                    }
                }
            }

            if (axonMoved)
            {
                AxonWaypoints.Add(new Point()
                {
                    X = maxUndesirabX,
                    Y = maxUndesirabY
                });
            }

            return axonMoved;
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

        protected NeuronBase(int id, CnnNet cnnNet)
        {
            _id = id;
            _cnnNet = cnnNet;
            AxonWaypoints = new List<Point>
            {
                new Point
                {
                    X = PosX,
                    Y = PosY
                }
            };
        }

        #endregion Instance
    }
}