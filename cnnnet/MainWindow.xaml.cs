using System;
using System.Collections.Generic;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public double NeuronDensity { get; set; }

        public PlaceHolder[,] PlaceHolders { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            NeuronDensity = 0.1f;
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            var random = new Random();
            PlaceHolders = new PlaceHolder[grid.Rows, grid.Columns];

            for (int i = 0; i < grid.Rows; i++)
            {
                for (int j = 0; j < grid.Columns; j++)
                {
                    var placeHolder = new PlaceHolder();

                    if (random.NextDouble() <= NeuronDensity)
                    {
                        placeHolder.Neuron = new Neuron();
                    }

                    PlaceHolders[i, j] = placeHolder;
                    grid.Children.Add(placeHolder);
                }
            }
        }
    }
}
