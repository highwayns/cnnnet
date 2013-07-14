using cnnnet.Lib;
using System;
using System.Collections.Generic;
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

        #endregion

        #region Methods

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            _network = new CnnNet(NetworkWidth, NetworkHeight);

            _viewerManager = new ViewerManager(_network);
            _viewerManager.RegisterViewer(new ViewerDesirability(_network));
            _viewerManager.RegisterViewer(new ViewerUndesirability(_network));

            _viewerManager.Start();

            img.Source = _viewerManager.WriteableBitmap;
            _networkProcessThread.Start();
        }

        private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _closeRequested = true;
            _viewerManager.Stop();
        }

        private void NetworkProcessThreadStart()
        {
            while (_closeRequested == false)
            {
                _network.Process();
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
