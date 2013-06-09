import java.util.*;

public class Network {

    //region Fields

    private NetworkParameters _networkParameters;

    private NeuronBase[][] _neuronPositionMap;
    private ArrayList<NeuronBase> _neurons;

    //endregion

    //region Methods

    //region Get

    public NetworkParameters get_networkParameters() {
        return _networkParameters;
    }

    //endregion

    public void GenerateNetwork()
    {
        _neuronPositionMap = new NeuronBase[_networkParameters.Width][_networkParameters.Height];
        _neurons = new ArrayList<NeuronBase>();
    }

    public void Start()
    {

    }

    //endregion

    //region Instance

    public Network(NetworkParameters networkParameters)
    {
        _networkParameters = networkParameters;
    }

    //endregion

}
