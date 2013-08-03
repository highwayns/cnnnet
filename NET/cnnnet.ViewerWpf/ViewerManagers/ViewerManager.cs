using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using cnnnet.ViewerWpf.Viewers;

namespace cnnnet.ViewerWpf.ViewerManagers
{
    public class ViewerManager
    {
        #region Fields

        public readonly WriteableBitmap WriteableBitmap;
        public readonly int Width;
        public readonly int Height;

        private readonly List<ViewerBase> _viewers;
        private byte[] _bitmapData;

        private readonly int _stride;

        private readonly Int32Rect _writableBitmapSourceRect;
        private readonly Thread _preRenderThread;

        #endregion

        #region Properties

        public List<ViewerBase> DisplayedViewers
        {
            get;
            private set;
        }

        #endregion

        #region Methods

        public void RegisterViewer(ViewerBase viewer)
        {
            _viewers.Add(viewer);
            DisplayedViewers.Add(viewer);
        }

        public void Update(double elapsed, int mousePosX, int mousePosY, bool leftButtonPressed)
        {
            using (WriteableBitmap.GetBitmapContext())
            {
                UpdateBackground();
                UpdateInternal(elapsed, mousePosX, mousePosY, leftButtonPressed);
            }
        }

        /// <summary>
        /// Perform any other desired transformation on the WriteableBitmap
        /// </summary>
        /// <param name="elapsed"></param>
        /// <param name="mousePosX"></param>
        /// <param name="mousePosY"></param>
        /// <param name="leftButtonPressed"></param>
        protected virtual void UpdateInternal(double elapsed, int mousePosX, int mousePosY, bool leftButtonPressed)
        {
        }

        /// <summary>
        /// Write the pixels computed in the PreRender method from the registered viewers
        /// </summary>
        private void UpdateBackground()
        {
            WriteableBitmap.WritePixels(_writableBitmapSourceRect, _bitmapData, _stride, 0);
        }

        private void PreRender()
        {
            while (true)
            {
                var tmpBitmapData = new byte[Height * Width * Constants.BytesPerPixel];

                var viewersWithData = _viewers.
                    Where(viewer => DisplayedViewers.Contains(viewer)).
                    Select(viewer => new
                    {
                        Viewer = viewer,
                        Data = viewer.GetData()
                    }).ToArray();

                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        int bitmapDataIndex = (y * Width + x) * Constants.BytesPerPixel;

                        tmpBitmapData[bitmapDataIndex + ColorIndex.Red] = 0;
                        tmpBitmapData[bitmapDataIndex + ColorIndex.Green] = 0;
                        tmpBitmapData[bitmapDataIndex + ColorIndex.Blue] = 0;

                        foreach (var viewerWithData in viewersWithData)
                        {
                            var dataWidth = viewerWithData.Data.GetLength(1) / Constants.BytesPerPixel /* Because of color data BGRA */;
                            var dataHeight = viewerWithData.Data.GetLength(0);
                            var skipWidth = (Width - dataWidth) / 2;
                            var skipHeight = (Height - dataHeight) / 2;
                            var dataX = x - skipWidth;
                            var dataY = y - skipHeight;

                            if (0 <= dataX && dataX < dataWidth
                                && 0 <= dataY && dataY < dataHeight)
                            {
                                tmpBitmapData[bitmapDataIndex + ColorIndex.Red] = (byte)Math.Min
                                    (tmpBitmapData[bitmapDataIndex + ColorIndex.Red] + viewerWithData.Data[dataY, dataX * Constants.BytesPerPixel + ColorIndex.Red], 255);

                                tmpBitmapData[bitmapDataIndex + ColorIndex.Green] = (byte)Math.Min
                                    (tmpBitmapData[bitmapDataIndex + ColorIndex.Green] + viewerWithData.Data[dataY, dataX * Constants.BytesPerPixel + ColorIndex.Green], 255);

                                tmpBitmapData[bitmapDataIndex + ColorIndex.Blue] = (byte)Math.Min
                                    (tmpBitmapData[bitmapDataIndex + ColorIndex.Blue] + viewerWithData.Data[dataY, dataX * Constants.BytesPerPixel + ColorIndex.Blue], 255);
                            }
                        }
                    }
                }

                _bitmapData = tmpBitmapData;
                Thread.Sleep(30);
            }
        }

        #endregion

        #region Instance

        public ViewerManager(int width, int height)
        {
            Width = width;
            Height = height;

            WriteableBitmap = BitmapFactory.New(Width, Height);
            Debug.Assert(Constants.BytesPerPixel == (WriteableBitmap.Format.BitsPerPixel + 7) / 8);

            _viewers = new List<ViewerBase>();
            DisplayedViewers = new List<ViewerBase>();

            _stride = WriteableBitmap.PixelWidth * Constants.BytesPerPixel;
            _bitmapData = new byte[Height * Width * Constants.BytesPerPixel];

            _writableBitmapSourceRect = new Int32Rect(0, 0, WriteableBitmap.PixelWidth, WriteableBitmap.PixelHeight);
            
            _preRenderThread = new Thread(PreRender)
            {
                IsBackground = true
            };
            _preRenderThread.Start();
        }

        #endregion
    }
}
