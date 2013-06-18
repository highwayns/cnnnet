﻿using System;
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
    }
}