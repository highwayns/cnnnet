using cnnnet.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using cnnnet.ViewerWpf.Viewers;

namespace cnnnet.ViewerWpf.ViewerManagers
{
    public abstract class ViewerManagerBase
    {
        #region Fields

        public readonly WriteableBitmap WriteableBitmap;

        protected readonly CnnNet Network;
        private readonly List<ViewerBase> _viewers;
        private byte[] _bitmapData;

        private readonly int _stride;
        private readonly int _bytesPerPixel;

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

        protected abstract void UpdateInternal(double elapsed, int mousePosX, int mousePosY, bool leftButtonPressed);

        private void UpdateBackground()
        {
            WriteableBitmap.WritePixels(_writableBitmapSourceRect, _bitmapData, _stride, 0);
        }

        private void PreRender()
        {
            try
            {
                while (true)
                {
                    var tmpBitmapData = new byte[Network.Height * Network.Width * _bytesPerPixel];

                    var viewersWithData = _viewers.
                        Where(viewer => DisplayedViewers.Contains(viewer)).
                        Select(viewer => new
                        {
                            Viewer = viewer,
                            Data = viewer.GetData()
                        }).ToArray();

                    for (int y = 0; y < Network.Height; y++)
                    {
                        for (int x = 0; x < Network.Width; x++)
                        {
                            int bitmapDataIndex = (y * Network.Width + x) * _bytesPerPixel;

                            tmpBitmapData[bitmapDataIndex + Constants.ColorRedIndex] = 0;
                            tmpBitmapData[bitmapDataIndex + Constants.ColorGreenIndex] = 0;
                            tmpBitmapData[bitmapDataIndex + Constants.ColorBlueIndex] = 0;

                            foreach (var viewerWithData in viewersWithData)
                            {
                                tmpBitmapData[bitmapDataIndex + Constants.ColorRedIndex] += viewerWithData.Data[y, x * viewerWithData.Viewer.BytesPerPixel + Constants.ColorRedIndex];
                                tmpBitmapData[bitmapDataIndex + Constants.ColorGreenIndex] += viewerWithData.Data[y, x * viewerWithData.Viewer.BytesPerPixel + Constants.ColorGreenIndex];
                                tmpBitmapData[bitmapDataIndex + Constants.ColorBlueIndex] += viewerWithData.Data[y, x * viewerWithData.Viewer.BytesPerPixel + Constants.ColorBlueIndex];
                            }
                        }
                    }

                    _bitmapData = tmpBitmapData;

                    Thread.Sleep(30);
                }
            }
            // ReSharper disable EmptyGeneralCatchClause
            catch (Exception)
            // ReSharper restore EmptyGeneralCatchClause
            {
            }
        }

        #endregion

        #region Instance

        protected ViewerManagerBase(CnnNet network)
        {
            Network = network;
            _viewers = new List<ViewerBase>();
            DisplayedViewers = new List<ViewerBase>();
            WriteableBitmap = BitmapFactory.New(Network.Width, Network.Height);

            _bytesPerPixel = (WriteableBitmap.Format.BitsPerPixel + 7) / 8;
            _stride = WriteableBitmap.PixelWidth * _bytesPerPixel;

            _bitmapData = new byte[Network.Height * Network.Width * _bytesPerPixel];

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
