using System;
using System.Collections.Generic;

namespace CnnNetLib2
{
    public abstract class NeuronBase
    {
        #region Fields

        protected CnnNet _cnnNet;
        protected int _id;
        protected bool _isActive;
        protected int _posX;
        protected int _posY;
        protected bool _hasReachedFinalPosition;
        protected int _axonLastCoordX;
        protected int _axonLastCoordY;

        protected double _movedDistance;
        protected int _neuronIterationsLeftBeforeFinalPosition;
        protected int _iterationsSinceLastActivation;

        #endregion

        #region Properties

        public CnnNet CnnNet
        {
            get
            {
                return _cnnNet;
            }
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

        public bool HasReachedFinalPosition
        {
            get
            {
                return _hasReachedFinalPosition;
            }
        }

        #endregion

        #region Methods

        public abstract void Process();

        public void MoveTo(int newPosY, int newPosX)
        {
            _cnnNet.NeuronPositionMap[PosY, PosX] = null;
            _cnnNet.NeuronPositionMap[newPosY, newPosX] = this;

            _posX = newPosX;
            _posY = newPosY;

            OnMoveTo(newPosY, newPosX);
        }

        protected virtual void OnMoveTo(int newPosY, int newPosX)
        {
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

        protected NeuronBase GetNeuronAt(int y, int x)
        {
            return _cnnNet.NeuronPositionMap[y, x];
        }

        protected NeuronBase[] GetNeuronsWithinRange(int range)
        {
            var ret = new List<NeuronBase>();

            int minCoordX = Math.Max(PosX - range, 0);
            int maxCoordX = Math.Min(PosX + range, _cnnNet.Width - 1);

            int minCoordY = Math.Max(PosY - range, 0);
            int maxCoordY = Math.Min(PosY + range, _cnnNet.Height - 1);

            for (int y = minCoordY; y < maxCoordY; y++)
            {
                for (int x = minCoordX; x < maxCoordX; x++)
                {
                    if ((x == PosX && y == PosY)
                        || Extensions.GetDistance(PosX, PosY, x, y) > range)
                    {
                        continue;
                    }

                    var neuron = GetNeuronAt(y, x);

                    if (neuron != null)
                    {
                        ret.Add(neuron);
                    }
                }
            }

            return ret.ToArray();
        }

        protected void AddDesirability()
        {
            AddProportionalRangedValue
                (_axonLastCoordX, _axonLastCoordY,
                _cnnNet.NeuronDesirabilityMap, _cnnNet.Width, _cnnNet.Height,
                _cnnNet.NeuronDesirabilityInfluenceRange,
                _cnnNet.NeuronDesirabilityMaxInfluence);
        }

        protected void AddUndesirability()
        {
            AddProportionalRangedValue
                (PosX, PosY, _cnnNet.NeuronUndesirabilityMap, _cnnNet.Width, _cnnNet.Height,
                _cnnNet.NeuronUndesirabilityInfluenceRange,
                _cnnNet.NeuronUndesirabilityMaxInfluence * Math.Max(1, _iterationsSinceLastActivation / _cnnNet.NeuronUndesirabilityMaxIterationsSinceLastActivation));
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
                        && GetNeuronAt(y, x) == null
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

        #endregion

        #region Instance

        protected NeuronBase(int id, CnnNet cnnNet)
        {
            _id = id;
            _cnnNet = cnnNet;
        }

        #endregion
    }
}
