using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using cnnnet.Lib;
using cnnnet.Lib.Neurons;
using cnnnet.Lib.Utils;
using cnnnet.ViewerWpf.Viewers;
using System.Diagnostics.Contracts;

namespace cnnnet.ViewerWpf.ViewerManagers
{
    public class ViewerManagerNetwork : ViewerManager
    {
        #region Fields

        private readonly CnnNet _network;

        private Neuron _neuronSelected;
        private Neuron _neuronHover;

        private Rect _neuronIconDestRect;
        private readonly Rect _neuronIconSourceRect;

        #endregion

        #region Properties
        
        public event EventHandler<NeuronChangedEventArgs> NeuronSelectedChanged;

        public event EventHandler<NeuronChangedEventArgs> NeuronHoverChanged;

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

        #endregion

        #region Methods
        
        protected override void UpdateInternal(double elapsed, int mousePosX, int mousePosY, bool leftButtonPressed)
        {
            using (Resources.NeuronActive.GetBitmapContext())
            using (Resources.NeuronHover.GetBitmapContext())
            using (Resources.NeuronIdle.GetBitmapContext())
            using (Resources.NeuronInputActive.GetBitmapContext())
            using (Resources.NeuronInputIdle.GetBitmapContext())
            using (Resources.NeuronSelected.GetBitmapContext())
            {
                UpdateHoverAndSelectedNeuron(mousePosX, mousePosY, leftButtonPressed);
                UpdateNeurons();
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

                WriteableBitmap.Blit(_neuronIconDestRect, neuronIcon, _neuronIconSourceRect);

                if (neuron == NeuronSelected)
                {
                    WriteableBitmap.Blit(_neuronIconDestRect, Resources.NeuronSelected, _neuronIconSourceRect);
                }
                else if (neuron == NeuronHover)
                {
                    WriteableBitmap.Blit(_neuronIconDestRect, Resources.NeuronHover, _neuronIconSourceRect);
                }


                #endregion

                #region Draw Axon

                var axonWaypoints = neuron.AxonWayPoints.ToArray();

                WriteableBitmap.DrawPolyline
                    (axonWaypoints.SelectMany(axonWaypoint => new[]
                    {
                        axonWaypoint.X,
                        axonWaypoint.Y
                    }).ToArray(),
                    neuron == NeuronSelected ? Colors.Blue : Colors.White);

                #endregion
            }
        }

        #endregion

        #region Instance

        public ViewerManagerNetwork(CnnNet network)
            : base(network.Width, network.Height)
        {
            Contract.Requires<ArgumentNullException>(network != null);

            _network = network;
            _neuronIconDestRect = new Rect(0, 0, Resources.NeuronIdle.PixelWidth, Resources.NeuronIdle.PixelHeight);
            _neuronIconSourceRect = new Rect(0, 0, Resources.NeuronIdle.PixelWidth, Resources.NeuronIdle.PixelHeight);
        }

        #endregion
    }
}
