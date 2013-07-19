namespace cnnnet.ViewerWpf.Viewers
{
    public abstract class ViewerBase
    {
        #region Fields
        
        protected readonly int Width;
        protected readonly int Height;
        public readonly int BytesPerPixel;

        #endregion

        #region Properties

        public bool IsEnabled
        {
            get;
            set;
        }

        #endregion

        #region Methods

        public byte[,] GetData()
        {
            var data = new byte[Height, Width * BytesPerPixel];

            if (IsEnabled)
            {
                UpdateDataInternal(ref data);
            }

            return data;
        }

        protected abstract void UpdateDataInternal(ref byte[,] data);

        #endregion

        #region Instance

        protected ViewerBase(int width, int height, int bytesPerPixel, bool isEnabled)
        {
            Width = width;
            Height = height;
            BytesPerPixel = bytesPerPixel;
            IsEnabled = isEnabled;
        }

        #endregion
    }
}
