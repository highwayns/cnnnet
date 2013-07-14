using cnnnet.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace cnnnet.ViewerWpf
{
    public class ViewerDesirability : Viewer
    {
        #region Fields
        
        private CnnNet _network;

        #endregion

        #region Methods

        protected override Color[,] GetData()
        {
            var desirabilityMap = (double[,])_network.NeuronDesirabilityMap.Clone();
            var result = new Color[_network.Height, _network.Width];

            for (int y = 0; y < _network.Height; y++)
            {
                for (int x = 0; x < _network.Width; x++)
                {
                    result[y, x] = Color.FromRgb((byte)0, (byte)(desirabilityMap[y, x] * 255), (byte)0);
                }
            }

            return result;
        }

        #endregion

        #region Instance

        public ViewerDesirability(CnnNet network)
            : base(network.Width, network.Height)
        {
            _network = network;
        }

        #endregion
    }
}
