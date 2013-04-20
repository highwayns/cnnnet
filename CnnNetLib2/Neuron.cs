using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CnnNetLib2
{
    public class Neuron
    {
        #region Fields

        public int PosX;
        public int PosY;
        public int Id;

        #endregion

        #region Instance

        public Neuron(int id)
        {
            Id = id;
        }

        #endregion
    }
}
