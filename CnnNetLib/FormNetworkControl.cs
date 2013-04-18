using System;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

namespace CnnNetLib
{
    public partial class FormNetworkControl : Form
    {
        #region Fields

        private readonly object _threadSyncObject;

        private CnnNet _cnnNet;
        private Thread _threadProcess;
        private bool _threadProcessStopInitiated;

        private int _stepNumber = 1;

        private int _lastStepNumber = int.MaxValue;

        #endregion

        #region Properties

        public CnnNet CnnNet
        {
            get
            {
                return _cnnNet;
            }
            set
            {
                _cnnNet = value;
            }
        }

        #endregion

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

        private void CreateThread()
        {
            _threadProcess = new Thread(ProcessNetwork)
            {
                IsBackground = true
            };
        }

        private void ProcessNetwork()
        {
            for (; _stepNumber < _lastStepNumber; _stepNumber++)
            {
                lock (_threadSyncObject)
                {
                    if (_threadProcessStopInitiated)
                    {
                        _threadProcessStopInitiated = false;
                        break;
                    }
                }

                _cnnNet.Process();

                this.InvokeEx(f => textBoxStepNumber.Text = _stepNumber.ToString(CultureInfo.InvariantCulture));
            }

            this.InvokeEx(f => OnProcessEnd());
        }

        private void OnProcessStart()
        {
            if (radioButtonRunInfinity.Checked)
            {
                _lastStepNumber = int.MaxValue;
            }
            else if(radioButtonSteps.Checked)
            {
                _lastStepNumber = _stepNumber + (int) nudSteps.Value;
            }

            buttonStart.Enabled = false;
            buttonStop.Enabled = true;
            buttonReset.Enabled = false;
            nudSteps.Enabled = false;
        }

        private void OnProcessEnding()
        {
            buttonStart.Enabled = false;
            buttonStop.Enabled = false;
            buttonReset.Enabled = false;
            nudSteps.Enabled = false;
        }

        private void OnProcessEnd()
        {
            buttonStart.Enabled = true;
            buttonStop.Enabled = false;
            buttonReset.Enabled = true;
            nudSteps.Enabled = radioButtonSteps.Checked;
        }

        private void OnButtonStartClick(object sender, EventArgs e)
        {
            OnProcessStart();
            CreateThread();
            _threadProcess.Start();
        }

        private void OnButtonStopClick(object sender, EventArgs e)
        {
            lock (_threadSyncObject)
            {
                OnProcessEnding();
                _threadProcessStopInitiated = true;
            }
        }

        private void OnButtonApplyParametersClick(object sender, EventArgs e)
        {
            _cnnNet.SetNetworkParameters(GetNetworkParameters());
            _cnnNet.ActiveNeuronGenerator = new SequentialActiveInputNeuronGenerator(_cnnNet.InputNeuronIds, Math.Min(_cnnNet.InputNeuronIds.Length, 2));
        }

        private void OnButtonResetClick(object sender, EventArgs e)
        {
            _stepNumber = 1;
            _cnnNet.GenerateNetwork();
            _cnnNet.ActiveNeuronGenerator = new SequentialActiveInputNeuronGenerator(_cnnNet.InputNeuronIds, Math.Min(_cnnNet.InputNeuronIds.Length, 2));
        }

        private void OnButtonNextStepByStepClick(object sender, EventArgs e)
        {
            _cnnNet.Process();
        }

        private void OnRadioButtonStepsCheckedChanged(object sender, EventArgs e)
        {
            nudSteps.Enabled = radioButtonSteps.Checked;
        }

        private void OnRadioButtonStepByStepCheckedChanged(object sender, EventArgs e)
        {
            buttonNextStepByStep.Enabled = radioButtonStepByStep.Checked;
        }

        #endregion

        #region Instance

        public FormNetworkControl()
        {
            InitializeComponent();

            CreateThread();

            _threadSyncObject = new object();
        }

        #endregion
    }
}
