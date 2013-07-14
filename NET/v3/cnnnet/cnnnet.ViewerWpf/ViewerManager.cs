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

        private Thread _updaterThread;
        private bool _updaterThreadStop;

        private WriteableBitmap _writableBitmap;
        private byte[,] bitmapData;
        private readonly int _stride;

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

        /// <summary>
        /// Starts the rendering rutine
        /// </summary>
        public void Start()
        {
            var thread = _updaterThread;

            #region State Check

            if (thread != null)
            {
                throw new InvalidOperationException("The viewer is already started");
            }

            #endregion

            thread = new Thread(new ThreadStart(Update));
            thread.IsBackground = true;
            thread.Start();

            _updaterThread = thread;
        }

        /// <summary>
        /// Ends the rendering rutine
        /// </summary>
        public void Stop()
        {
            var thread = _updaterThread;

            #region State Check

            if (thread == null)
            {
                throw new InvalidOperationException("The viewer is already stoped");
            }

            #endregion

            _updaterThreadStop = true;
        }

        /// <summary>
        /// _updaterThread starting point
        /// </summary>
        private void Update()
        {
            try
            {
                while (_updaterThreadStop == false)
                {
                    var viewersData = _viewers.Select(viewer => viewer.GetData()).ToArray();

                    for (int y = 0; y < _network.Height; y++)
                    {
                        for (int x = 0; x < _network.Width; x++)
                        {
                            bitmapData[y, x * 3 + Constants.ColorRedIndex] = 0;
                            bitmapData[y, x * 3 + Constants.ColorGreenIndex] = 0;
                            bitmapData[y, x * 3 + Constants.ColorBlueIndex] = 0;

                            foreach (var viewerData in viewersData)
                            {
                                bitmapData[y, x * 3 + Constants.ColorRedIndex] += viewerData[y, x * 3 + Constants.ColorRedIndex];
                                bitmapData[y, x * 3 + Constants.ColorGreenIndex] += viewerData[y, x * 3 + Constants.ColorGreenIndex];
                                bitmapData[y, x * 3 + Constants.ColorBlueIndex] += viewerData[y, x * 3 + Constants.ColorBlueIndex];
                            }
                        }
                    }

                    _writableBitmap.Dispatcher.Invoke(() =>
                        _writableBitmap.ForEach((x, y, color) =>
                        Color.FromArgb((byte)255,
                        (byte)(bitmapData[y, x * 3 + Constants.ColorRedIndex]),
                        (byte)(bitmapData[y, x * 3 + Constants.ColorGreenIndex]),
                        (byte)(bitmapData[y, x * 3 + Constants.ColorBlueIndex]))));
                }

                _updaterThreadStop = false;
                _updaterThread = null;
            }
            catch
            {
            }
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
            bitmapData = new byte[_network.Height, _network.Width * 3];

            _stride = _writableBitmap.PixelWidth * (_writableBitmap.Format.BitsPerPixel + 7) / 8;
        }

        #endregion
    }
}
