using cnnnet.Lib;
using cnnnet.Lib.Neurons;
using cnnnet.Lib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace cnnnet.ViewerWpf.Viewers
{
    public class ViewerManager
    {
        #region Fields

        private Neuron _neuronSelected;
        private Neuron _neuronHover;

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

        public event EventHandler<NeuronChangedEventArgs> NeuronSelectedChanged;
        public event EventHandler<NeuronChangedEventArgs> NeuronHoverChanged;

        public WriteableBitmap WriteableBitmap
        {
            get
            {
                return _writableBitmap;
            }
        }

        public Neuron NeuronSelected
        {
            get
            {
                return _neuronSelected;
            }
            private set
            {
                _neuronSelected = value;
                OnNeuronSelectedChanged(value);
            }
        }

        public Neuron NeuronHover
        {
            get
            {
                return _neuronHover;
            }
            private set
            {
                _neuronHover = value;
                OnNeuronHoverChanged(value);
            }
        }

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
            using (_writableBitmap.GetBitmapContext())
            using (Resources.NeuronActive.GetBitmapContext())
            using (Resources.NeuronHover.GetBitmapContext())
            using (Resources.NeuronIdle.GetBitmapContext())
            using (Resources.NeuronInputActive.GetBitmapContext())
            using (Resources.NeuronInputIdle.GetBitmapContext())
            using (Resources.NeuronSelected.GetBitmapContext())
            {
                UpdateBackground();
                UpdateHoverAndSelectedNeuron(mousePosX, mousePosY, leftButtonPressed);
                UpdateNeurons();
            }
        }

        private void UpdateHoverAndSelectedNeuron(int mousePosX, int mousePosY, bool leftButtonPressed)
        {
            NeuronHover = Extensions.GetClosestNeuronsWithinRange(mousePosX, mousePosY, _network, 10);

            if (NeuronHover != null)
            {
                if (leftButtonPressed)
                {
                    NeuronSelected = NeuronHover;
                }
            }
        }

        private void UpdateNeurons()
        {
            foreach (var neuron in _network.Neurons)
            {
                #region Draw Soma

                var neuronIcon = neuron.IsActive
                                      ? neuron.IsInputNeuron ? Resources.NeuronInputActive : Resources.NeuronActive
                                      : neuron.IsInputNeuron ? Resources.NeuronInputIdle : Resources.NeuronIdle;

                _neuronIconDestRect.X = neuron.PosX - neuronIcon.PixelWidth / 2;
                _neuronIconDestRect.Y = neuron.PosY - neuronIcon.PixelHeight / 2;

                _writableBitmap.Blit(_neuronIconDestRect, neuronIcon, _neuronIconSourceRect);

                if (neuron == NeuronSelected)
                {
                    _writableBitmap.Blit(_neuronIconDestRect, Resources.NeuronSelected, _neuronIconSourceRect);
                } 
                else if (neuron == NeuronHover)
                {
                    _writableBitmap.Blit(_neuronIconDestRect, Resources.NeuronHover, _neuronIconSourceRect);
                }
                

                #endregion

                #region Draw Axon

                _writableBitmap.DrawPolyline
                    (neuron.AxonWayPoints.SelectMany
                    (axonWaypoint => new[] { axonWaypoint.X, axonWaypoint.Y }).ToArray(),
                    neuron == NeuronSelected ? Colors.Blue : Colors.White);

                #endregion
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

                    var viewersWithData = _viewers.
                        Where(viewer => DisplayedViewers.Contains(viewer)).
                        Select(viewer => new
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

        private void OnNeuronSelectedChanged(Neuron value)
        {
            var handler = NeuronSelectedChanged;
            if (handler != null)
            {
                handler(this, new NeuronChangedEventArgs(value));
            }
        }

        private void OnNeuronHoverChanged(Neuron value)
        {
            var handler = NeuronHoverChanged;
            if (handler != null)
            {
                handler(this, new NeuronChangedEventArgs(value));
            }
        }

        #endregion

        #region Instance

        public ViewerManager(CnnNet network)
        {
            _network = network;
            _viewers = new List<ViewerBase>();
            DisplayedViewers = new List<ViewerBase>();
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
