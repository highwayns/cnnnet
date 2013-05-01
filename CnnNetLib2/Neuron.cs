using System;
using System.Collections.Generic;

namespace CnnNetLib2
{
    public class Neuron
    {
        #region Fields

        public int Id;
        private readonly CnnNet _cnnNet;

        public bool IsActive;
        public bool IsInputNeuron;

        public int PosX;
        public int PosY;
        public bool HasReachedFinalPosition;

        private double _movedDistance;
        private int _neuronIterationsLeftBeforeFinalPosition;
        private int _iterationsSinceLastActivation;

        #endregion

        #region Methods

        public void Process()
        {
            if (HasReachedFinalPosition)
            {
                #region Increase region desirability if neuron fires

                if (IsActive)
                {
                    AddDesirability();
                    _iterationsSinceLastActivation = 0;
                }

                #endregion

                #region Else increase region UN-desirability

                else
                {
                    _iterationsSinceLastActivation++;
                    AddUndesirability();
                }

                #endregion
            }
            else
            {
                #region Neuron searches for better position

                if ((_cnnNet.InputNeuronsMoveToHigherDesirability
                     || IsInputNeuron == false)
                    &&
                    _movedDistance < _cnnNet.MaxNeuronMoveDistance)
                {
                    _neuronIterationsLeftBeforeFinalPosition =
                        ProcessMoveToHigherDesirability()
                        ? _cnnNet.NeuronIterationCountBeforeFinalPosition
                        : _neuronIterationsLeftBeforeFinalPosition - 1;

                    if (_neuronIterationsLeftBeforeFinalPosition == 0)
                    {
                        HasReachedFinalPosition = true;
                    }
                }

                #endregion
            }
        }

        private void AddDesirability()
        {
            AddProportionalRangedValue
                (_cnnNet.NeuronDesirabilityMap, _cnnNet.Width, _cnnNet.Height,
                _cnnNet.NeuronDesirabilityInfluenceRange, 
                _cnnNet.NeuronDesirabilityMaxInfluence);
        }

        private void AddUndesirability()
        {
            AddProportionalRangedValue
                (_cnnNet.NeuronUndesirabilityMap, _cnnNet.Width, _cnnNet.Height,
                _cnnNet.NeuronUndesirabilityInfluenceRange,
                _cnnNet.NeuronUndesirabilityMaxInfluence * Math.Max(1, _iterationsSinceLastActivation / _cnnNet.NeuronUndesirabilityMaxIterationsSinceLastActivation));
        }

        private void AddProportionalRangedValue(double[,] map, int width, int height, int influencedRange, double maxValue)
        {
            int xMin = Math.Max(PosX - influencedRange, 0);
            int xMax = Math.Min(PosX + influencedRange, width);
            int yMin = Math.Max(PosY - influencedRange, 0);
            int yMax = Math.Min(PosY + influencedRange, height);

            for (int y = yMin; y < yMax; y++)
            {
                for (int x = xMin; x < xMax; x++)
                {
                    var distance = Extensions.GetDistance(PosX, PosY, x, y);

                    var influenceByRange = Math.Max(influencedRange - distance, 0);

                    map[y, x] = Math.Min(1, map[y, x] + influenceByRange / influencedRange * maxValue);
                }
            }
        }

        private bool ProcessMoveToHigherDesirability()
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

        private void MoveTo(int newPosY, int newPosX)
        {
            _cnnNet.NeuronPositionMap[PosY, PosX] = null;
            _cnnNet.NeuronPositionMap[newPosY, newPosX] = this;

            PosX = newPosX;
            PosY = newPosY;
        }

        /// <summary>
        /// Remember to optimize this by using spiral matrix processing
        /// http://pastebin.com/4EYJvv5X
        /// </summary>
        /// <param name="referenceY"></param>
        /// <param name="referenceX"></param>
        /// <returns></returns>
        private double GetDistanceToNearestNeuron(int referenceY, int referenceX)
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

        private Neuron GetNeuronAt(int y, int x)
        {
            return _cnnNet.NeuronPositionMap[y, x];
        }

        private Neuron[] GetNeuronsWithinRange(int range)
        {
            var ret = new List<Neuron>();

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

        #endregion

        #region Instance

        public Neuron(int id, CnnNet cnnNet)
        {
            Id = id;
            _cnnNet = cnnNet;
            _neuronIterationsLeftBeforeFinalPosition = _cnnNet.NeuronIterationCountBeforeFinalPosition;
        }

        #endregion
    }
}
