using cnnnet.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace cnnnet.ViewerWpf
{
    public class ViewerUndesirability : ViewerBase
    {
        #region Fields
        
        private CnnNet _network;
        private byte[,] data;

        #endregion

        #region Properties

        public override int BytesPerPixel
        {
            get
            {
                return 3;
            }
        }

        #endregion

        #region Methods

        public override byte[,] GetData()
        {
            var desirabilityMap = (double[,])_network.NeuronUndesirabilityMap.Clone();

            for (int y = 0; y < _network.Height; y++)
            {
                for (int x = 0; x < _network.Width; x++)
                {
                    data[y, x * 3 + Constants.ColorRedIndex] = (byte)(desirabilityMap[y, x] * 255);
                }
            }

            return data;
        }

        #endregion

        #region Instance

        public ViewerUndesirability(CnnNet network)
        {
            _network = network;
            data = new byte[_network.Height, _network.Width * 3];
        }

        #endregion
    }
}
