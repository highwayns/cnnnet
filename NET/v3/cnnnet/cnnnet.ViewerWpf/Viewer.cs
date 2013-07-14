using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace cnnnet.ViewerWpf
{
    /// <summary>
    /// Class used for rendering a region
    /// </summary>
    public abstract class Viewer
    {
        #region Fields

        private Thread _updaterThread;
        private WriteableBitmap _writableBitmap;
        private bool _updaterThreadStop;

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
            while (_updaterThreadStop == false)
            {
                var data = GetData();
                _writableBitmap.Dispatcher.Invoke(() => _writableBitmap.ForEach((x, y, color) => data[y, x]));
            }

            _updaterThreadStop = false;
            _updaterThread = null;
        }

        protected abstract Color[,] GetData();

        #endregion

        #region Instance

        /// <summary>
        /// Creates a viewer for a region (Image control)
        /// </summary>
        /// <param name="width">The region width</param>
        /// <param name="height">The region height</param>
        public Viewer(int width, int height)
        {
            _writableBitmap = BitmapFactory.New(width, height);
        }

        #endregion
    }
}
