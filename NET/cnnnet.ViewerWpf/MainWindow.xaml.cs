using cnnnet.Lib;
using cnnnet.Lib.Neurons;
using cnnnet.ViewerWpf.ViewerManagers;
using cnnnet.ViewerWpf.Viewers;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace cnnnet.ViewerWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        #region Fields

        private Thread _networkProcessThread;
        private CnnNet _network;
        private Neuron _selectedNeuron;

        private ViewerManagerNetwork _viewerManager;
        private ViewerNetworkDesirability _viewerDesirability;
        private ViewerNetworkUndesirability _viewerUndesirability;

        private ViewerManagerAxonTerminal _viewerManagerAxonTerminal;
        private ViewerAxonTerminalGuidanceForces _viewerAxonTerminalGuidanceForces;

        private bool _closeRequested;

        private DateTime _lastUpdate = DateTime.Now;
        private readonly Stopwatch _stopWatch = Stopwatch.StartNew();
        private double _lowestFrameTime;
        private double _lastTime;

        #endregion

        #region Properties

        public event PropertyChangedEventHandler PropertyChanged;

        public Neuron SelectedNeuron
        {
            get
            {
                return _selectedNeuron;
            }
            private set
            {
                if (_selectedNeuron == value)
                {
                    return;
                }

                _selectedNeuron = value;
                NotifyPropertyChanged();
            }
        }

        public CnnNet Network
        {
            get
            {
                return _network;
            }
            private set
            {
                if (_network == value)
                {
                    return;
                }

                _network = value;
                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Methods

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            Network = new CnnNet(Constants.NetworkWidth, Constants.NetworkHeight);

            _viewerManager = new ViewerManagerNetwork(Network);
            _viewerManager.RegisterViewer(_viewerDesirability = new ViewerNetworkDesirability(Network));
            _viewerManager.RegisterViewer(_viewerUndesirability = new ViewerNetworkUndesirability(Network));
            _viewerManager.NeuronSelectedChanged += OnViewerManagerNeuronSelectedChanged;

            _viewerManagerAxonTerminal = new ViewerManagerAxonTerminal();
            _viewerAxonTerminalGuidanceForces = new ViewerAxonTerminalGuidanceForces();
            //_viewerManagerAxonTerminal.RegisterViewer(_viewerAxonTerminalGuidanceForces);

            ImageNetwork.Source = _viewerManager.WriteableBitmap;
            ImageAxonTerminalGuidanceForces.Source = _viewerManagerAxonTerminal.WriteableBitmap;
            CompositionTarget.Rendering += OnCompositionTargetRendering;
        }

        private void OnViewerManagerNeuronSelectedChanged(object sender, NeuronChangedEventArgs e)
        {
            SelectedNeuron = e.Neuron;
            _viewerAxonTerminalGuidanceForces.Neuron = e.Neuron;
        }

        private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _closeRequested = true;
        }

        private void NetworkProcessThreadStart()
        {
            while (_closeRequested == false)
            {
                Network.Process();
            }

            _closeRequested = false;
        }

        private void OnCompositionTargetRendering(object sender, EventArgs e)
        {
            var mousePosition = Mouse.GetPosition(ImageNetwork);

            // Wrap updates in a GetContext call, to prevent invalidation and nested locking/unlocking during this block
            // NOTE: This is not strictly necessary for the SL version as this is a WPF feature, however we include it here for completeness and to show
            // a similar API to WPF
            double elapsed = (DateTime.Now - _lastUpdate).TotalSeconds;
            _lastUpdate = DateTime.Now;

            using (_viewerManager.WriteableBitmap.GetBitmapContext())
            {
                _viewerManager.Update(elapsed, (int)mousePosition.X, (int)mousePosition.Y,
                    Mouse.LeftButton == MouseButtonState.Pressed);
            }

            using (_viewerManagerAxonTerminal.WriteableBitmap.GetBitmapContext())
            {
                _viewerManagerAxonTerminal.Update(elapsed, (int)mousePosition.X, (int)mousePosition.Y,
                    Mouse.LeftButton == MouseButtonState.Pressed);
            }

            double timeNow = _stopWatch.ElapsedMilliseconds;
            double elapsedMilliseconds = timeNow - _lastTime;
            _lowestFrameTime = Math.Min(_lowestFrameTime, elapsedMilliseconds);
            FpsCounter.Text = string.Format("FPS: {0:0.0} / Max: {1:0.0}", 1000.0 / elapsedMilliseconds, 1000.0 / _lowestFrameTime);
            _lastTime = timeNow;
        }

        private void OnButtonStartClick(object sender, RoutedEventArgs e)
        {
            OnStart();
        }

        private void OnButtonStopClick(object sender, RoutedEventArgs e)
        {
            OnStop();
        }

        private void OnButtonNextClick(object sender, RoutedEventArgs e)
        {
            OnNext();
        }

        private void OnButtonResetClick(object sender, RoutedEventArgs e)
        {
            OnReset();
        }

        private void OnStart()
        {
            ButtonStart.IsEnabled = false;
            ButtonStop.IsEnabled = true;
            ButtonNext.IsEnabled = false;
            ButtonReset.IsEnabled = false;

            _networkProcessThread = new Thread(NetworkProcessThreadStart)
            {
                IsBackground = true
            };
            _networkProcessThread.Start();
        }

        private void OnStop()
        {
            ButtonStart.IsEnabled = true;
            ButtonStop.IsEnabled = false;
            ButtonNext.IsEnabled = true;
            ButtonReset.IsEnabled = true;

            _closeRequested = true;
        }

        private void OnNext()
        {
            ButtonStart.IsEnabled = false;
            ButtonStop.IsEnabled = false;
            ButtonNext.IsEnabled = false;
            ButtonReset.IsEnabled = false;

            Network.Process();

            ButtonStart.IsEnabled = true;
            ButtonStop.IsEnabled = false;
            ButtonNext.IsEnabled = true;
            ButtonReset.IsEnabled = true;
        }

        private void OnReset()
        {
            Network.GenerateNetwork();
        }

        private void OnCboxNeuronDesirabilityMapCheckedChanged(object sender, RoutedEventArgs e)
        {
            if (_viewerDesirability == null)
            {
                return;
            }

            ChangeViewerVisibility(CboxNeuronDesirabilityMap.IsChecked, _viewerDesirability);
        }

        private void OnCboxNeuronUndesirabilityMapCheckedChanged(object sender, RoutedEventArgs e)
        {
            if (_viewerUndesirability == null)
            {
                return;
            }

            ChangeViewerVisibility(CboxNeuronUndesirabilityMap.IsChecked, _viewerUndesirability);
        }

        private void OnCboxNeuronBreakOnProcessCheckedChanged(object sender, RoutedEventArgs e)
        {

        }

        private void ChangeViewerVisibility(bool? display, ViewerBase viewer)
        {
            switch (display.HasValue && display.Value)
            {
                case true:
                    {
                        _viewerManager.DisplayedViewers.Add(viewer);
                        break;
                    }
                case false:
                    {
                        _viewerManager.DisplayedViewers.Remove(viewer);
                        break;
                    }
            }
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region Instance

        public MainWindow()
        {
            InitializeComponent();
        }

        #endregion
    }
}
