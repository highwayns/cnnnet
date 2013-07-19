using System.Windows.Media.Imaging;

namespace cnnnet.ViewerWpf
{
    public static class Resources
    {
        #region Fields
        
        public static readonly WriteableBitmap NeuronActive;
        public static readonly WriteableBitmap NeuronHover;
        public static readonly WriteableBitmap NeuronIdle;
        public static readonly WriteableBitmap NeuronInputActive;
        public static readonly WriteableBitmap NeuronInputIdle;
        public static readonly WriteableBitmap NeuronSelected;

        #endregion

        #region Instance
        
        static Resources()
        {
            NeuronActive = Utils.LoadBitmap(@"Images\neuronActive.png");
            NeuronHover = Utils.LoadBitmap(@"Images\neuronHover.png");
            NeuronIdle = Utils.LoadBitmap(@"Images\neuronIdle.png");
            NeuronInputActive = Utils.LoadBitmap(@"Images\neuronInputActive.png");
            NeuronInputIdle = Utils.LoadBitmap(@"Images\neuronInputIdle.png");
            NeuronSelected = Utils.LoadBitmap(@"Images\neuronSelected.png");
        }

        #endregion
    }
}
