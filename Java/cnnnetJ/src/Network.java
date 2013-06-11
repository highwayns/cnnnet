import java.util.*;

public class Network {

    //region Fields

    private NetworkParameters _networkParameters;

    private NeuronBase[][] _neuronPositionMap;

    private ArrayList<NeuronBase> _neurons;
    private ArrayList<NeuronInput> _neuronsInput;
    private NeuronBase[] _neuronsActive;

    public double[][] _neuronDesirabilityMap;
    public double[][] _neuronUndesirabilityMap;

    private IActiveNeuronGenerator _activeNeuronGenerator;
    private Random _random;

    //endregion

    //region Methods

    //region Get

    public NetworkParameters get_networkParameters() {
        return this._networkParameters;
    }

    public NeuronBase[][] get_neuronPositionMap() {
        return this._neuronPositionMap;
    }

    public List<NeuronBase> get_neurons() {
        return this._neurons;
    }

    //endregion

    public void GenerateNetwork()
    {
        _neuronDesirabilityMap = new double[_networkParameters.Height][_networkParameters.Width];
        _neuronUndesirabilityMap = new double[_networkParameters.Height][_networkParameters.Width];
        _neuronPositionMap = new NeuronBase[_networkParameters.Height][_networkParameters.Width];
        _neurons = new ArrayList<NeuronBase>();
        _neuronsInput = new ArrayList<NeuronInput>();

        for (int i = 0; i < _networkParameters.NeuronCount + _networkParameters.NeuronInputCount; i++)
        {
            NeuronBase neuron = i < _networkParameters.NeuronCount
                    ? (new NeuronCompute(i, this))
                    : (new NeuronInput(i, this));

            boolean continueLoop;
            do
            {
                neuron.MoveTo(_random.nextInt(_networkParameters.Height), _random.nextInt(_networkParameters.Width));

                // determine continuation
                continueLoop = false;
                for (NeuronBase currentNeuron : _neurons)
                    continueLoop = currentNeuron.get_posX() == neuron.get_posX() && currentNeuron.get_posY() == neuron.get_posY();
            }
            while (continueLoop);

            _neurons.add(neuron);
            if(neuron instanceof NeuronInput) {
                _neuronsInput.add((NeuronInput)neuron);
            }

            _neuronPositionMap[neuron.get_posY()][neuron.get_posX()] = neuron;
        }

        _activeNeuronGenerator =
                new SequentialActiveInputNeuronGenerator
                        (_neuronsInput.toArray(new NeuronInput[_neuronsInput.size()]), _neuronsInput.size() < 2 ? 2 : _neuronsInput.size());
    }

    public void Process() {
        ProcessDetermineActiveNeurons();

        for(NeuronBase neuron : _neurons) {
            neuron.Process();
        }

        ProcessDecayDesirabilityAndUndesirability();
    }

    private void ProcessDecayDesirabilityAndUndesirability() {
        for (int y = 0; y < _networkParameters.Height; y++)
        {
            for (int x = 0; x < _networkParameters.Width; x++)
            {
                _neuronDesirabilityMap[y][x] = Math.max(0, _neuronDesirabilityMap[y][x] - _networkParameters.DesirabilityDecayAmount);
                _neuronUndesirabilityMap[y][x] = Math.max(0, _neuronUndesirabilityMap[y][x] - _networkParameters.UndesirabilityDecayAmount);
            }
        }
    }

    private void ProcessDetermineActiveNeurons() {
        // inactivate previous active neurons
        if (_neuronsActive != null) {
            for (NeuronBase neuron : _neuronsActive) {
                neuron.set_isActive(false);
            }
        }

        // obtain new active neurons
        _neuronsActive = _activeNeuronGenerator.GetActiveNeurons();

        // set active neurons
        for (NeuronBase neuron : _neuronsActive) {
            neuron.set_isActive(true);
        }
    }

    //endregion

    //region Instance

    public Network(NetworkParameters networkParameters)
    {
        _networkParameters = networkParameters;

        _random = new Random();
    }

    //endregion

}
