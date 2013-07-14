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
        private ViewerDesirability _viewerDesirability;

        #endregion

        #region Methods

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            _network = new CnnNet(NetworkWidth, NetworkHeight);
            _viewerDesirability = new ViewerDesirability(_network);

            _viewerDesirability.Start();
            img.Source = _viewerDesirability.WriteableBitmap;
            _networkProcessThread.Start();
        }

        private void NetworkProcessThreadStart()
        {
            while (true)
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
