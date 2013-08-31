using cnnnet.Lib.Utils;
using System;

namespace cnnnet.ViewerWpf.Viewers
{
    public abstract class ViewerBase
    {
        #region Fields
        
        public readonly int Width;
        public readonly int Height;

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
            var data = new byte[Height, Width * Constants.BytesPerPixel];

            if (IsEnabled)
            {
                UpdateDataInternal(ref data);
            }

            return data;
        }

        protected abstract void UpdateDataInternal(ref byte[,] data);

        #endregion

        #region Instance

        protected ViewerBase(int width, int height, bool isEnabled)
        {
            Contract.Requires<ArgumentOutOfRangeException>(width > 0);
            Contract.Requires<ArgumentOutOfRangeException>(height > 0);

            Width = width;
            Height = height;
            IsEnabled = isEnabled;
        }

        #endregion
    }
}
