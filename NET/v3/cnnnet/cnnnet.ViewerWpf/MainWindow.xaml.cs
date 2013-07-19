﻿using cnnnet.Lib;
using cnnnet.ViewerWpf.Viewers;
using System;
using System.Diagnostics;
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
    public partial class MainWindow
    {
        #region Fields

        private const int NetworkWidth = 800;
        private const int NetworkHeight = 600;

        private Thread _networkProcessThread;
        private CnnNet _network;

        private ViewerManager _viewerManager;
        private ViewerDesirability _viewerDesirability;
        private ViewerUndesirability _viewerUndesirability;
        private ViewerAxonTerminalGuidanceForces _viewerAxonTerminalGuidanceForces;

        private bool _closeRequested;

        private DateTime _lastUpdate = DateTime.Now;
        private readonly Stopwatch _stopWatch = Stopwatch.StartNew();
        private double _lowestFrameTime;
        private double _lastTime;

        #endregion

        #region Methods

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            _network = new CnnNet(NetworkWidth, NetworkHeight);

            _viewerManager = new ViewerManager(_network);

            _viewerDesirability = new ViewerDesirability(_network);
            _viewerUndesirability = new ViewerUndesirability(_network);
            _viewerAxonTerminalGuidanceForces = new ViewerAxonTerminalGuidanceForces();

            _viewerManager.RegisterViewer(_viewerDesirability);
            _viewerManager.RegisterViewer(_viewerUndesirability);
            _viewerManager.NeuronSelectedChanged += OnViewerManagerNeuronSelectedChanged;

            ImageNetwork.Source = _viewerManager.WriteableBitmap;
            CompositionTarget.Rendering += OnCompositionTargetRendering;
        }

        private void OnViewerManagerNeuronSelectedChanged(object sender, NeuronChangedEventArgs e)
        {
            LabelId.Content = e.Neuron.Id;
            LabelLocation.Content = string.Format("X:{0} Y:{1}", e.Neuron.PosX, e.Neuron.PosY);
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

            _closeRequested = false;
        }

        private void OnCompositionTargetRendering(object sender, EventArgs e)
        {
            var mousePosition = Mouse.GetPosition(ImageNetwork);

            // Wrap updates in a GetContext call, to prevent invalidation and nested locking/unlocking during this block
            // NOTE: This is not strictly necessary for the SL version as this is a WPF feature, however we include it here for completeness and to show
            // a similar API to WPF
            using (_viewerManager.WriteableBitmap.GetBitmapContext())
            {
                _viewerManager.WriteableBitmap.Clear(Colors.Black);

                double elapsed = (DateTime.Now - _lastUpdate).TotalSeconds;
                _lastUpdate = DateTime.Now;
                _viewerManager.Update(elapsed, (int)mousePosition.X, (int)mousePosition.Y,
                    Mouse.LeftButton == MouseButtonState.Pressed);

                double timeNow = _stopWatch.ElapsedMilliseconds;
                double elapsedMilliseconds = timeNow - _lastTime;
                _lowestFrameTime = Math.Min(_lowestFrameTime, elapsedMilliseconds);
                FpsCounter.Text = string.Format("FPS: {0:0.0} / Max: {1:0.0}", 1000.0 / elapsedMilliseconds, 1000.0 / _lowestFrameTime);
                _lastTime = timeNow;
            }
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

            _network.Process();

            ButtonStart.IsEnabled = true;
            ButtonStop.IsEnabled = false;
            ButtonNext.IsEnabled = true;
            ButtonReset.IsEnabled = true;
        }

        private void OnReset()
        {
            _network.GenerateNetwork();
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

        #endregion

        #region Instance

        public MainWindow()
        {
            InitializeComponent();
        }

        #endregion
    }
}
