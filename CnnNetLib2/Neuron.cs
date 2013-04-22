using System;
using System.Linq;

namespace CnnNetLib2
{
    public class Neuron
    {
        #region Fields

        public int Id;
        private readonly CnnNet _cnnNet;

        public int PosX;
        public int PosY;

        private double _movedDistance;

        #endregion

        #region Methods

        public void Process()
        {
            if (_cnnNet.ActiveNeurons.Any(an => an == this))
            {
                AddDesirability();
            }

            if ((_cnnNet.InputNeuronsMoveToHigherDesirability
                 || _cnnNet.InputNeurons.All(inpNeuron => inpNeuron != this))
                &&
                _movedDistance < _cnnNet.MaxNeuronMoveDistance)
            {
                ProcessMoveToHigherDesirability();
            }
        }

        private void AddDesirability()
        {
            int xMin = Math.Max(PosX - _cnnNet.NeuronInfluenceRange, 0);
            int xMax = Math.Min(PosX + _cnnNet.NeuronInfluenceRange, _cnnNet.Width);
            int yMin = Math.Max(PosY - _cnnNet.NeuronInfluenceRange, 0);
            int yMax = Math.Min(PosY + _cnnNet.NeuronInfluenceRange, _cnnNet.Height);

            for (int y = yMin; y < yMax; y++)
            {
                for (int x = xMin; x < xMax; x++)
                {
                    var distance = Extensions.GetDistance(PosX, PosY, x, y);

                    var influenceByRange = Math.Max(_cnnNet.NeuronInfluenceRange - distance, 0);

                    _cnnNet.NeuronDesirabilityMap[y, x] =
                        Math.Min(1, _cnnNet.NeuronDesirabilityMap[y, x] + influenceByRange / _cnnNet.NeuronInfluenceRange * _cnnNet.MaxNeuronInfluence);
                }
            }
        }

        private void ProcessMoveToHigherDesirability()
        {
            int minCoordX = Math.Max(PosX - _cnnNet.NeuronDesirabilityPlainRange, 0);
            int maxCoordX = Math.Min(PosX + _cnnNet.NeuronDesirabilityPlainRange, _cnnNet.Width - 1);

            int minCoordY = Math.Max(PosY - _cnnNet.NeuronDesirabilityPlainRange, 0);
            int maxCoordY = Math.Min(PosY + _cnnNet.NeuronDesirabilityPlainRange, _cnnNet.Height - 1);

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
                        && GetDistanceToNearestNeuron(y, x) >= _cnnNet.MinDistanceBetweenNeurons)
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
            }
        }

        private void MoveTo(int newPosY, int newPosX)
        {
            PosX = newPosX;
            PosY = newPosY;
        }

        private double GetDistanceToNearestNeuron(int referenceY, int referenceX)
        {
            double distanceToNearestNeuron = _cnnNet.NeuronDesirabilityPlainRange + 1;

            int xMin = Math.Max(referenceX - _cnnNet.MinDistanceBetweenNeurons, 0);
            int xMax = Math.Min(referenceX + _cnnNet.MinDistanceBetweenNeurons, _cnnNet.Width - 1);
            int yMin = Math.Max(referenceY - _cnnNet.MinDistanceBetweenNeurons, 0);
            int yMax = Math.Min(referenceY + _cnnNet.MinDistanceBetweenNeurons, _cnnNet.Height - 1);

            var distances = _cnnNet.Neurons.Where(neuron =>
                xMin <= neuron.PosX
                && neuron.PosX <= xMax
                && yMin <= neuron.PosY
                && neuron.PosY <= yMax).
                Select(neuron => Extensions.GetDistance(referenceX, referenceY, neuron.PosX, neuron.PosY)).ToArray();

            return distances.Any()
                       ? distances.Min()
                       : distanceToNearestNeuron;
        }

        private Neuron GetNeuronAt(int y, int x)
        {
            return _cnnNet.Neurons.FirstOrDefault(neuron => neuron.PosX == x && neuron.PosY == y);
        }

        #endregion

        #region Instance

        public Neuron(int id, CnnNet cnnNet)
        {
            Id = id;
            _cnnNet = cnnNet;
        }

        #endregion
    }
}
