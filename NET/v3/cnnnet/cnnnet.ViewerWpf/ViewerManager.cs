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
        private byte[] bitmapData;

        private readonly int _stride;
        private readonly int _bytesPerPixel;

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

        public void Update(double elapsed)
        {
            UpdateBackground();
            UpdateNeurons();
        }

        private void UpdateNeurons()
        {
            foreach (var neuron in _network.Neurons)
            {
                _writableBitmap.Dispatcher.Invoke
                    (() => _writableBitmap.Blit(new Rect(neuron.PosX, neuron.PosY, Resources.NeuronIdle.PixelWidth, Resources.NeuronIdle.PixelHeight), Resources.NeuronIdle,
                    new Rect(0, 0, Resources.NeuronIdle.PixelWidth, Resources.NeuronIdle.PixelHeight)));
            }
        }

        private void UpdateBackground()
        {
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

                    bitmapData[bitmapDataIndex + Constants.ColorRedIndex] = 0;
                    bitmapData[bitmapDataIndex + Constants.ColorGreenIndex] = 0;
                    bitmapData[bitmapDataIndex + Constants.ColorBlueIndex] = 0;

                    foreach (var viewerWithData in viewersWithData)
                    {
                        bitmapData[bitmapDataIndex + Constants.ColorRedIndex] += viewerWithData.Data[y, x * viewerWithData.Viewer.BytesPerPixel + Constants.ColorRedIndex];
                        bitmapData[bitmapDataIndex + Constants.ColorGreenIndex] += viewerWithData.Data[y, x * viewerWithData.Viewer.BytesPerPixel + Constants.ColorGreenIndex];
                        bitmapData[bitmapDataIndex + Constants.ColorBlueIndex] += viewerWithData.Data[y, x * viewerWithData.Viewer.BytesPerPixel + Constants.ColorBlueIndex];
                    }
                }
            }

            _writableBitmap.Dispatcher.Invoke(() =>
                _writableBitmap.ForEach((x, y, color) =>
                Color.FromArgb((byte)255,
                (byte)(bitmapData[(y * _network.Width + x) * _bytesPerPixel + Constants.ColorRedIndex]),
                (byte)(bitmapData[(y * _network.Width + x) * _bytesPerPixel + Constants.ColorGreenIndex]),
                (byte)(bitmapData[(y * _network.Width + x) * _bytesPerPixel + Constants.ColorBlueIndex]))));
        }

        public void RegisterViewer(ViewerBase viewer)
        {
            _viewers.Add(viewer);
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

            bitmapData = new byte[_network.Height * _network.Width * _bytesPerPixel];
        }

        #endregion
    }
}
