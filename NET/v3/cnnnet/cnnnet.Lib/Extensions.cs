using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace cnnnet.Lib
{
    public static class Extensions
    {
        public static double GetDistance(int x1, int y1, int x2, int y2)
        {
            return x1 == x2 && y1 == y2
                ? 0
                : Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
        }

        public static void InvokeEx<T>(this T @this, Action<T> action) where T : ISynchronizeInvoke
        {
            if (@this.InvokeRequired)
            {
                @this.Invoke(action, new object[] { @this });
            }
            else
            {
                action(@this);
            }
        }

        public static NeuronBase GetNeuronAt(int y, int x, CnnNet network)
        {
            return network.NeuronPositionMap[y, x];
        }

        public static NeuronBase[] GetNeuronsWithinRange(int posX, int posY, CnnNet network, int range)
        {
            var ret = new List<NeuronBase>();

            int minCoordX = Math.Max(posX - range, 0);
            int maxCoordX = Math.Min(posX + range, network.Width - 1);

            int minCoordY = Math.Max(posY - range, 0);
            int maxCoordY = Math.Min(posY + range, network.Height - 1);

            for (int y = minCoordY; y < maxCoordY; y++)
            {
                for (int x = minCoordX; x < maxCoordX; x++)
                {
                    if ((x == posX && y == posY)
                        || Extensions.GetDistance(posX, posY, x, y) > range)
                    {
                        continue;
                    }

                    var neuron = GetNeuronAt(y, x, network);

                    if (neuron != null)
                    {
                        ret.Add(neuron);
                    }
                }
            }

            return ret.ToArray();
        }
    }
}
