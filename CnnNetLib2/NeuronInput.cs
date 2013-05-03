using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace CnnNetLib2
{
    public class NeuronInput : NeuronBase
    {
        #region Fields

        private bool _axonFinalPositionReached;

        /// <summary>
        /// Y = Item1
        /// X = Item2
        /// </summary>
        public readonly List<Tuple<int, int>> AxonWaypoints;

        #endregion

        #region Methods

        public void SetIsActive(bool isActive)
        {
            _isActive = isActive;
        }

        public override void Process()
        {
            if (_axonFinalPositionReached)
            {
                if (IsActive)
                {
                    AddDesirability();
                }
            }
            else
            {
                // navigate axon to higher undesirability
                //_axonFinalPositionReached = ProcessGuideAxon() == false
                //                            && AxonWaypoints.Count > 0;

                ProcessGuideAxon();
            }
        }

        private bool ProcessGuideAxon()
        {
            int minCoordX = Math.Max(PosX - _cnnNet.AxonHigherUndesirabilitySearchPlainRange, 0);
            int maxCoordX = Math.Min(PosX + _cnnNet.AxonHigherUndesirabilitySearchPlainRange, _cnnNet.Width - 1);

            int minCoordY = Math.Max(PosY - _cnnNet.AxonHigherUndesirabilitySearchPlainRange, 0);
            int maxCoordY = Math.Min(PosY + _cnnNet.AxonHigherUndesirabilitySearchPlainRange, _cnnNet.Height - 1);

            int lastPosX = AxonWaypoints.Last().Item2;
            int lastPosY = AxonWaypoints.Last().Item1;

            int maxUndesirabX = lastPosX;
            int maxUndesirabY = lastPosY;
            double maxUndesirability = _cnnNet.NeuronUndesirabilityMap[maxUndesirabY, maxUndesirabX];
            Trace.WriteLine(maxUndesirability);

            bool axonMoved = false;
            bool undesirabilityFound = false;

            for (int y = minCoordY; y < maxCoordY; y++)
            {
                for (int x = minCoordX; x < maxCoordX; x++)
                {
                    if (Math.Abs(_cnnNet.NeuronUndesirabilityMap[y, x] - 0.0d) < 0.00001)
                    {
                        continue;
                    }

                    undesirabilityFound = true;

                    if ((x == PosX && y == PosY)
                        || (x == maxUndesirabX && y == maxUndesirabY)
                        || (x == lastPosX && y == lastPosY))
                    {
                        continue;
                    }

                    if (_cnnNet.NeuronUndesirabilityMap[y, x] > maxUndesirability
                        && GetNeuronAt(y, x) == null
                        && Extensions.GetDistance(lastPosX, lastPosY, x, y) <= _cnnNet.AxonHigherUndesirabilitySearchPlainRange /* this ensures that we only check within the range */
                        && DistanceFromPreviousWaypoints(y, x) > _cnnNet.AxonMinDistanceToPreviousWaypoints)
                    {
                        axonMoved = true;
                        maxUndesirabX = x;
                        maxUndesirabY = y;
                        maxUndesirability = _cnnNet.NeuronUndesirabilityMap[y, x];
                    }
                }
            }

            if (undesirabilityFound == false)
            {
                return false;
            }

            if (axonMoved)
            {
                AxonWaypoints.Add(new Tuple<int, int>(maxUndesirabY, maxUndesirabX));
            }

            return axonMoved;
        }

        private double DistanceFromPreviousWaypoints(int y, int x)
        {
            if (AxonWaypoints.Count == 0)
            {
                return float.MaxValue;
            }

            return AxonWaypoints.Select(waypoint => Extensions.GetDistance(x, y, waypoint.Item2, waypoint.Item1)).Min();
        }

        protected override void OnMoveTo(int newPosY, int newPosX)
        {
            base.OnMoveTo(newPosY, newPosX);

            AxonWaypoints[0] = new Tuple<int, int>(newPosY, newPosX);
        }

        #endregion

        #region Instance

        public NeuronInput(int id, CnnNet cnnNet)
            : base(id, cnnNet)
        {
            _hasReachedFinalPosition = true;
            AxonWaypoints = new List<Tuple<int, int>>
            {
                new Tuple<int, int>(PosY, PosX)
            };
        }

        #endregion
    }
}
