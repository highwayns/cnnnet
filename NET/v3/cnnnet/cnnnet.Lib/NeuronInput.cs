using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace cnnnet.Lib
{
    public class NeuronInput : NeuronBase
    {
        #region Fields

        /// <summary>
        /// Y = Item1
        /// X = Item2
        /// </summary>
        public readonly List<Point> AxonWaypoints;

        #endregion

        #region Methods

        public void SetIsActive(bool isActive)
        {
            _isActive = isActive;
        }

        public override void Process()
        {
            if (HasAxonReachedFinalPosition)
            {
                if (IsActive)
                {
                    AddDesirability();
                }
            }
            else
            {
                // navigate axon to higher undesirability
                HasAxonReachedFinalPosition = ProcessGuideAxon() == false
                                            && AxonWaypoints.Count > 1;

                if (HasAxonReachedFinalPosition)
                {
                    _axonLastCoordX = AxonWaypoints.Last().X;
                    _axonLastCoordY = AxonWaypoints.Last().Y;
                }
            }
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

            //int minCoordX = Math.Max(maxUndesirabX - _cnnNet.AxonHigherUndesirabilitySearchPlainRange, 0);
            //int maxCoordX = Math.Min(maxUndesirabX + _cnnNet.AxonHigherUndesirabilitySearchPlainRange, _cnnNet.Width - 1);

            //int minCoordY = Math.Max(maxUndesirabY - _cnnNet.AxonHigherUndesirabilitySearchPlainRange, 0);
            //int maxCoordY = Math.Min(maxUndesirabY + _cnnNet.AxonHigherUndesirabilitySearchPlainRange, _cnnNet.Height - 1);
            
            double maxUndesirability = _cnnNet.NeuronUndesirabilityMap[maxUndesirabY, maxUndesirabX];
            Trace.WriteLine(maxUndesirability);

            bool axonMoved = false;

            var record = false;
            var recordList = new List<double>();

            for (int y = minCoordY; y < maxCoordY; y++)
            {
                for (int x = minCoordX; x < maxCoordX; x++)
                {
                    if (Math.Abs(_cnnNet.NeuronUndesirabilityMap[y, x] - 0.0d) < 0.00001)
                    {
                        continue;
                    }

                    var distance = 0.0d;
                    if ((x == PosX && y == PosY)
                        || (x == maxUndesirabX && y == maxUndesirabY)
                        || (x == lastPosX && y == lastPosY)
                        || GetNeuronAt(y, x) != null
                        || (distance = Extensions.GetDistance(lastPosX, lastPosY, x, y)) > _cnnNet.AxonHigherUndesirabilitySearchPlainRange /* this ensures that we only check within the range */)
                    {
                        continue;
                    }

                    if (record)
                    {
                        recordList.Add(_cnnNet.NeuronUndesirabilityMap[y, x]);
                    }
                    
                    if (//_cnnNet.NeuronUndesirabilityMap[y, x] > maxUndesirability
                        _cnnNet.NeuronUndesirabilityMap[y, x] > _cnnNet.NeuronUndesirabilityMap[maxUndesirabY, maxUndesirabX]
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

        private double DistanceFromPreviousWaypoints(int y, int x)
        {
            if (AxonWaypoints.Count == 0)
            {
                return float.MaxValue;
            }

            return AxonWaypoints.Select(waypoint => Extensions.GetDistance(x, y, waypoint.X, waypoint.Y)).Min();
        }

        protected override void OnMoveTo(int newPosY, int newPosX)
        {
            base.OnMoveTo(newPosY, newPosX);

            AxonWaypoints[0] = new Point()
            {
                X = newPosX,
                Y = newPosY
            };
        }

        #endregion

        #region Instance

        public NeuronInput(int id, CnnNet cnnNet)
            : base(id, cnnNet)
        {
            HasSomaReachedFinalPosition = true;
            AxonWaypoints = new List<Point>
            {
                new Point
                {
                    X = PosX,
                    Y = PosY
                }
            };
        }

        #endregion
    }
}
