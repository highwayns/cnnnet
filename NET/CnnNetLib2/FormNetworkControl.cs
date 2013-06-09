using System;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace CnnNetLib2
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

        private readonly Timer _timer;
        // holds the number of Process calls handeled in the unit of time (1000 ms)
        private int _numberOfPerformedSteps;

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

                npDesirabilityDecayAmount.Value = (decimal)_cnnNet.DesirabilityDecayAmount;
                npInputNeuronCount.Value = _cnnNet.InputNeuronCount;
                npMaxNeuronInfluence.Value = (decimal)_cnnNet.NeuronDesirabilityMaxInfluence;
                npMaxNeuronMoveDistance.Value = _cnnNet.MaxNeuronMoveDistance;
                npMinDistanceBetweenNeurons.Value = _cnnNet.MinDistanceBetweenNeurons;
                npNeuronCount.Value = _cnnNet.NeuronCount;
                npNeuronDesirabilityPlainRange.Value = _cnnNet.NeuronHigherDesirabilitySearchPlainRange;
                npNeuronInfluenceRange.Value = _cnnNet.NeuronDesirabilityInfluenceRange;
            }
        }

        #endregion

        #region Methods

        public void SetNetworkParameters()
        {
            _cnnNet.DesirabilityDecayAmount = (double)npDesirabilityDecayAmount.Value;
            _cnnNet.InputNeuronCount = (int)npInputNeuronCount.Value;
            _cnnNet.NeuronDesirabilityMaxInfluence = (double)npMaxNeuronInfluence.Value;
            _cnnNet.MaxNeuronMoveDistance = (int)npMaxNeuronMoveDistance.Value;
            _cnnNet.MinDistanceBetweenNeurons = (int)npMinDistanceBetweenNeurons.Value;
            _cnnNet.NeuronCount = (int)npNeuronCount.Value;
            _cnnNet.NeuronHigherDesirabilitySearchPlainRange = (int)npNeuronDesirabilityPlainRange.Value;
            _cnnNet.NeuronDesirabilityInfluenceRange = (int)npNeuronInfluenceRange.Value;
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
                _numberOfPerformedSteps++;
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
            _timer.Start();
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
                _timer.Stop();
            }
        }

        private void OnButtonApplyParametersClick(object sender, EventArgs e)
        {
            SetNetworkParameters();
        }

        private void OnButtonResetClick(object sender, EventArgs e)
        {
            _stepNumber = 1;
            _numberOfPerformedSteps = 0;
            _cnnNet.GenerateNetwork();
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

        private void OnTimerTick(object sender, EventArgs e)
        {
            var numberOfPerformedSteps = _numberOfPerformedSteps;
            _numberOfPerformedSteps = 0;

            textBoxStepsPerSecond.Text = (numberOfPerformedSteps / ((float)_timer.Interval / 1000)).ToString(CultureInfo.InvariantCulture);
        }

        #endregion

        #region Instance

        public FormNetworkControl()
        {
            InitializeComponent();

            CreateThread();

            _threadSyncObject = new object();

            _timer = new Timer
            {
                Interval = 4000,
                Enabled = false
            };
            _timer.Tick += OnTimerTick;
        }

        #endregion
    }
}
