using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
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

namespace CnnSimple1
{
    /// <summary>
    /// Interaction logic for PlaceHolder.xaml
    /// </summary>
    public partial class PlaceHolder : UserControl, INotifyPropertyChanged
    {
        #region Fields

        private double _desirability;

        #endregion

        #region Properties

        public Neuron Neuron
        {
            get { return (Neuron)GetValue(NeuronProperty); }
            set { SetValue(NeuronProperty, value); }
        }

        public static readonly DependencyProperty NeuronProperty =
            DependencyProperty.Register("Neuron", typeof(Neuron),
            typeof(PlaceHolder), new PropertyMetadata(null, OnNeuronPropertyChangedCallback));

        public SolidColorBrush State
        {
            get { return (SolidColorBrush)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register("State", typeof(SolidColorBrush),
            typeof(PlaceHolder), new PropertyMetadata(new SolidColorBrush(Colors.White)));

        public double Desirability
        {
            get
            {
                return _desirability;
            }
            set
            {
                _desirability = value;
                NotifyPropertyChanged("Desirability");

                if (Neuron == null)
                {
                    State = new SolidColorBrush(new Color() { R = (byte)(255 * _desirability), A = 255, ScA = 1 });
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Methods

        private static void OnNeuronPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PlaceHolder)d).State = new SolidColorBrush(Colors.Red);
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            var propertyChangedHandler = PropertyChanged;
            if (propertyChangedHandler != null)
            {
                propertyChangedHandler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region Instance

        public PlaceHolder()
        {
            InitializeComponent();
        }

        #endregion
    }
}
