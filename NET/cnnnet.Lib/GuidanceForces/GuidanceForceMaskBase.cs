using cnnnet.Lib.Neurons;
using cnnnet.Lib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cnnnet.Lib.GuidanceForces
{
    public abstract class GuidanceForceMaskBase
    {
        #region Fields
        
        private readonly int _width;
        private readonly int _height;

        #endregion

        #region Methods

        public void Update(int x, int y, Neuron neuron)
        {

        }

        #endregion

        #region Instance

        protected GuidanceForceMaskBase(int width, int height)
        {
            Contract.Requires<ArgumentOutOfRangeException>(width > 0);
            Contract.Requires<ArgumentOutOfRangeException>(height > 0);

            _width = width;
            _height = height;
        }

        #endregion
    }
}
