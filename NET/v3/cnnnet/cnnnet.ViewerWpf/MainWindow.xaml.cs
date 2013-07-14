using cnnnet.Lib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace cnnnet.ViewerWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Fields

        private const int NetworkWidth = 800;
        private const int NetworkHeight = 600;

        private Thread _networkProcessThread;
        private CnnNet _network;

        private ViewerManager _viewerManager;
        private bool _closeRequested;

        private DateTime lastUpdate = DateTime.Now;
        private Stopwatch _stopwatch = Stopwatch.StartNew();
        private double _lowestFrameTime;
        private double _lastTime;

        #endregion

        #region Methods

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            _network = new CnnNet(NetworkWidth, NetworkHeight);

            _viewerManager = new ViewerManager(_network);
            _viewerManager.RegisterViewer(new ViewerDesirability(_network));
            _viewerManager.RegisterViewer(new ViewerUndesirability(_network));

            image.Source = _viewerManager.WriteableBitmap;
            CompositionTarget.Rendering += OnCompositionTargetRendering;

            _networkProcessThread.Start();
        }

        private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _closeRequested = true;
        }

        private void NetworkProcessThreadStart()
        {
            while (_closeRequested == false)
            {
                _network.Process();
            }
        }

        private void OnCompositionTargetRendering(object sender, EventArgs e)
        {
            var mousePosition = Mouse.GetPosition(image);

            // Wrap updates in a GetContext call, to prevent invalidation and nested locking/unlocking during this block
            // NOTE: This is not strictly necessary for the SL version as this is a WPF feature, however we include it here for completeness and to show
            // a similar API to WPF
            using (_viewerManager.WriteableBitmap.GetBitmapContext())
            {
                _viewerManager.WriteableBitmap.Clear(Colors.Black);

                double elapsed = (DateTime.Now - lastUpdate).TotalSeconds;
                lastUpdate = DateTime.Now;
                _viewerManager.Update(elapsed, (int)mousePosition.X, (int)mousePosition.Y,
                    Mouse.LeftButton == MouseButtonState.Pressed);

                double timeNow = _stopwatch.ElapsedMilliseconds;
                double elapsedMilliseconds = timeNow - _lastTime;
                _lowestFrameTime = Math.Min(_lowestFrameTime, elapsedMilliseconds);
                FpsCounter.Text = string.Format("FPS: {0:0.0} / Max: {1:0.0}", 1000.0 / elapsedMilliseconds, 1000.0 / _lowestFrameTime);
                _lastTime = timeNow;
            }
        }

        #endregion

        #region Instance

        public MainWindow()
        {
            InitializeComponent();

            _networkProcessThread = new Thread(NetworkProcessThreadStart);
            _networkProcessThread.IsBackground = true;
        }

        #endregion
    }
}
