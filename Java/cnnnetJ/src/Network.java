import java.util.*;

public class Network {

    //region Fields

    private NetworkParameters _networkParameters;

    private NeuronBase[][] _neuronPositionMap;
    private ArrayList<NeuronBase> _neurons;

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
        _neuronPositionMap = new NeuronBase[_networkParameters.Width][_networkParameters.Height];
        _neurons = new ArrayList<NeuronBase>();

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
            _neuronPositionMap[neuron.get_posY()][neuron.get_posX()] = neuron;
        }
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
