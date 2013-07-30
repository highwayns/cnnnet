﻿namespace cnnnet.ViewerWpf.Viewers
{
    public abstract class ViewerBase
    {
        #region Fields
        
        protected readonly int Width;
        protected readonly int Height;

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
            Width = width;
            Height = height;
            IsEnabled = isEnabled;
        }

        #endregion
    }
}
