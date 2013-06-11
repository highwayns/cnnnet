import java.util.*;

public class Network {

    //region Fields

    private NetworkParameters _networkParameters;

    private NeuronBase[][] _neuronPositionMap;
    private ArrayList<NeuronBase> _neurons;
    private ArrayList<NeuronInput> _neuronsInput;

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
                for (Iterator<NeuronBase> iterator = _neurons.iterator(); iterator.hasNext();)
                {
                    NeuronBase currentNeuron = iterator.next();
                    continueLoop = currentNeuron.get_posX() == neuron.get_posX() && currentNeuron.get_posY() == neuron.get_posY();
                }
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

    public void Start()
    {

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
