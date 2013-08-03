using cnnnet.Lib;

namespace cnnnet.ViewerWpf.Viewers
{
    public class ViewerNetworkUndesirability : ViewerBase
    {
        #region Fields
        
        private readonly CnnNet _network;

        #endregion

        #region Methods

        protected override void UpdateDataInternal(ref byte[,] data)
        {
            var desirabilityMap = (double[,])_network.NeuronUndesirabilityMap.Clone();

            for (int y = 0; y < _network.Height; y++)
            {
                for (int x = 0; x < _network.Width; x++)
                {
                    data[y, x * Constants.BytesPerPixel + Constants.ColorRedIndex] = (byte)(desirabilityMap[y, x] * 255);
                }
            }
        }

        #endregion

        #region Instance

        public ViewerNetworkUndesirability(CnnNet network)
            : base(network.Width, network.Height, true)
        {
            _network = network;
        }

        #endregion
    }
}
