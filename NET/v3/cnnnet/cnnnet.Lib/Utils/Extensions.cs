﻿using cnnnet.Lib.Neurons;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace cnnnet.Lib.Utils
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

        public static NeuronBase[] GetNeuronsWithAxonTerminalWithinRange(int posX, int posY, CnnNet network, int range)
        {
            int minCoordX = Math.Max(posX - range, 0);
            int maxCoordX = Math.Min(posX + range, network.Width - 1);

            int minCoordY = Math.Max(posY - range, 0);
            int maxCoordY = Math.Min(posY + range, network.Height - 1);

            return network.Neurons.Where
                (neuron => neuron.PosX != posX
                           && neuron.PosY != posY
                           && neuron.HasAxonReachedFinalPosition
                           && minCoordX <= neuron.AxonTerminal.X && neuron.AxonTerminal.X <= maxCoordX
                           && minCoordY <= neuron.AxonTerminal.Y && neuron.AxonTerminal.Y <= maxCoordY
                           && GetDistance(neuron.AxonTerminal.X, neuron.AxonTerminal.Y, posX, posY) <= range).ToArray();
        }

        public static Point GetMaxLocation(this double[,] map)
        {
            Point result = new Point();

            for (int y = 1; y < map.GetLength(0); y++)
            {
                for (int x = 0; x < map.GetLength(1); x++)
                {
                    if (map[y, x] > map[result.Y, result.X])
                    {
                        result.X = x;
                        result.Y = y;
                    }
                }
            }

            return result;
        }
    }
}