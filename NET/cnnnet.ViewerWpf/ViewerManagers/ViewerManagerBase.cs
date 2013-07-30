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
    public abstract class ViewerManagerBase
    {
        #region Fields

        public readonly WriteableBitmap WriteableBitmap;

        protected readonly int Width;
        protected readonly int Height;

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

                            tmpBitmapData[bitmapDataIndex + Constants.ColorRedIndex] = 0;
                            tmpBitmapData[bitmapDataIndex + Constants.ColorGreenIndex] = 0;
                            tmpBitmapData[bitmapDataIndex + Constants.ColorBlueIndex] = 0;

                            foreach (var viewerWithData in viewersWithData)
                            {
                                // TODO: correct error here

                                tmpBitmapData[bitmapDataIndex + Constants.ColorRedIndex] += viewerWithData.Data[y, x * Constants.BytesPerPixel + Constants.ColorRedIndex];
                                tmpBitmapData[bitmapDataIndex + Constants.ColorGreenIndex] += viewerWithData.Data[y, x * Constants.BytesPerPixel + Constants.ColorGreenIndex];
                                tmpBitmapData[bitmapDataIndex + Constants.ColorBlueIndex] += viewerWithData.Data[y, x * Constants.BytesPerPixel + Constants.ColorBlueIndex];
                            }
                        }
                    }

                    _bitmapData = tmpBitmapData;

                    Thread.Sleep(30);
                }
            }
            // ReSharper disable EmptyGeneralCatchClause
            #pragma warning disable 168
            catch (Exception ex)
            #pragma warning restore 168
            // ReSharper restore EmptyGeneralCatchClause
            {
                Debugger.Break();
            }
        }

        #endregion

        #region Instance

        protected ViewerManagerBase(int width, int height)
        {
            Width = width;
            Height = height;

            _viewers = new List<ViewerBase>();
            DisplayedViewers = new List<ViewerBase>();

            WriteableBitmap = BitmapFactory.New(Width, Height);
            Debug.Assert(Constants.BytesPerPixel == (WriteableBitmap.Format.BitsPerPixel + 7) / 8);

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
