import org.newdawn.slick.opengl.*;
import org.newdawn.slick.util.*;

import java.io.*;
import java.util.*;

public class NetworkDrawer {

    //region Fields

    private Network _network;
    private NeuronBase[][] _neuronPositionMap;
    private List<NeuronBase> _neurons;

    private Texture _neuronActive;
    private Texture _neuronIdle;
    private Texture _neuronInputActive;
    private Texture _neuronInputIdle;

    //endregion

    //region Methods

    public void Update()
    {
        for(Iterator<NeuronBase> iterator = this._neurons.iterator(); iterator.hasNext();) {
            NeuronBase currentNeuron = iterator.next();


        }
    }

    //endregion

    //region Instance

    public NetworkDrawer(Network network) throws IOException
    {
        _network = network;
        _neuronPositionMap = _network.get_neuronPositionMap();
        _neurons = _network.get_neurons();

        _neuronActive = TextureLoader.getTexture("PNG", ResourceLoader.getResourceAsStream("res/neuronActive.png"));
        _neuronIdle = TextureLoader.getTexture("PNG", ResourceLoader.getResourceAsStream("res/neuronIdle.png"));
        _neuronInputActive = TextureLoader.getTexture("PNG", ResourceLoader.getResourceAsStream("res/neuronInputActive.png"));
        _neuronInputIdle = TextureLoader.getTexture("PNG", ResourceLoader.getResourceAsStream("res/neuronInputIdle.png"));
    }

    //endregion
}
