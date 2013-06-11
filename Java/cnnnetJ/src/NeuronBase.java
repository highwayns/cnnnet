
public abstract class NeuronBase
{
    //region Fields
    private final int _id;
    private final Network _network;
    private final NeuronBase[][] _neuronPositionMap;

    private int _posX;
    private int _posY;

    private boolean _isActive;
    //endregion

    public int get_posX()
    {
        return _posX;
    }

    public int get_posY()
    {
        return _posY;
    }

    public boolean is_isActive() {
        return _isActive;
    }

    public void set_isActive(boolean _isActive) {
        this._isActive = _isActive;
    }

    public void MoveTo(int newPosY, int newPosX)
    {
        _neuronPositionMap[_posY][_posX] = null;
        _neuronPositionMap[newPosY][newPosX] = this;

        _posX = newPosX;
        _posY = newPosY;
    }

    public void Process() {

    }

    //region Instance
    protected NeuronBase(int id, Network network)
    {
        _id = id;
        _network = network;

        _neuronPositionMap = _network.get_neuronPositionMap();
    }

    //endregion
}
