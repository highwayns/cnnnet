using System.Windows.Forms;

namespace CnnNetLib
{
    public partial class FormNetworkControl : Form
    {
        #region Methods

        public NetworkParameters GetNetworkParameters()
        {
            return new NetworkParameters
            {
                DesirabilityDecayAmount = (double)npDesirabilityDecayAmount.Value,
                InputNeuronCount = (int)npInputNeuronCount.Value,
                InputNeuronsMoveToHigherDesirability = npInputNeuronsMoveToHigherDesirability.Checked,
                MaxNeuronInfluence = (double)npMaxNeuronInfluence.Value,
                MaxNeuronMoveDistance = (int)npMaxNeuronMoveDistance.Value,
                MinDistanceBetweenNeurons = (int)npMinDistanceBetweenNeurons.Value,
                NeuronDensity = (double)npNeuronDensity.Value,
                NeuronDesirabilityPlainRange = (int)npNeuronDesirabilityPlainRange.Value,
                NeuronInfluenceRange = (int)npNeuronInfluenceRange.Value,
                PercentActiveNeurons = (double)npPercentActiveNeurons.Value
            };
        }

        #endregion

        #region Instance

        public FormNetworkControl()
        {
            InitializeComponent();
        }

        #endregion
    }
}
