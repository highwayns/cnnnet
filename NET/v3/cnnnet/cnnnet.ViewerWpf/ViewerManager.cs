using cnnnet.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace cnnnet.ViewerWpf
{
    public class ViewerManager
    {
        #region Fields

        private CnnNet _network;
        private readonly List<ViewerBase> _viewers;

        private WriteableBitmap _writableBitmap;
        private byte[] _bitmapData;

        private readonly int _stride;
        private readonly int _bytesPerPixel;

        private Int32Rect _writableBitmapSourceRect;
        private Rect _neuronIconDestRect;
        private Rect _neuronIconSourceRect;

        private Thread _preRenderThread;

        #endregion

        #region Properties

        public WriteableBitmap WriteableBitmap
        {
            get
            {
                return _writableBitmap;
            }
        }

        #endregion

        #region Methods

        public void RegisterViewer(ViewerBase viewer)
        {
            _viewers.Add(viewer);
        }

        public void Update(double elapsed)
        {
            using (_writableBitmap.GetBitmapContext())
            using (Resources.NeuronIdle.GetBitmapContext())
            {
                UpdateBackground();
                UpdateNeurons();
            }
        }

        private void UpdateNeurons()
        {
            foreach (var neuron in _network.Neurons)
            {
                _neuronIconDestRect.X = neuron.PosX;
                _neuronIconDestRect.Y = neuron.PosY;

                _writableBitmap.Blit(_neuronIconDestRect, Resources.NeuronIdle, _neuronIconSourceRect);
            }
        }

        private void UpdateBackground()
        {
            _writableBitmap.WritePixels(_writableBitmapSourceRect, _bitmapData, _stride, 0);
        }

        private void PreRender()
        {
            try
            {
                while (true)
                {
                    byte[] tmpBitmapData = new byte[_network.Height * _network.Width * _bytesPerPixel];

                    var viewersWithData = _viewers.Select(viewer => new
                    {
                        Viewer = viewer,
                        Data = viewer.GetData()
                    }).ToArray();

                    for (int y = 0; y < _network.Height; y++)
                    {
                        for (int x = 0; x < _network.Width; x++)
                        {
                            int bitmapDataIndex = (y * _network.Width + x) * _bytesPerPixel;

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
                    };

                    _bitmapData = tmpBitmapData;

                    Thread.Sleep(30);
                }
            }
            catch
            {
            }
        }

        #endregion

        #region Instance

        public ViewerManager(CnnNet network)
        {
            _network = network;
            _viewers = new List<ViewerBase>();
            _writableBitmap = BitmapFactory.New(_network.Width, _network.Height);

            _bytesPerPixel = (_writableBitmap.Format.BitsPerPixel + 7) / 8;
            _stride = _writableBitmap.PixelWidth * _bytesPerPixel;

            _bitmapData = new byte[_network.Height * _network.Width * _bytesPerPixel];

            _writableBitmapSourceRect = new Int32Rect(0, 0, _writableBitmap.PixelWidth, _writableBitmap.PixelHeight);
            _neuronIconDestRect = new Rect(0, 0, Resources.NeuronIdle.PixelWidth, Resources.NeuronIdle.PixelHeight);
            _neuronIconSourceRect = new Rect(0, 0, Resources.NeuronIdle.PixelWidth, Resources.NeuronIdle.PixelHeight);

            _preRenderThread = new Thread(PreRender);
            _preRenderThread.IsBackground = true;
            _preRenderThread.Start();
        }

        #endregion
    }
}
