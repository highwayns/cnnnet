
public abstract class NeuronBase {
    //region Fields
    private final int _id;
    private final Network _network;
    private final NeuronBase[][] _neuronPositionMap;

    private int _posX;
    private int _posY;

    private boolean _isActive;
    private boolean _axonFinalPositionReached = true;
    //endregion

    public int get_posX() {
        return _posX;
    }

    public int get_posY() {
        return _posY;
    }

    public boolean is_isActive() {
        return _isActive;
    }

    public void set_isActive(boolean _isActive) {
        this._isActive = _isActive;
    }

    public void MoveTo(int newPosY, int newPosX) {
        _neuronPositionMap[_posY][_posX] = null;
        _neuronPositionMap[newPosY][newPosX] = this;

        _posX = newPosX;
        _posY = newPosY;
    }

    public void Process() {
        if (_axonFinalPositionReached) {
            if (_isActive) {
                AddDesirability();
            }
        } else {
            // navigate axon to higher undesirability
            //ProcessGuideAxon();
        }
    }

    private void AddDesirability() {
        AddProportionalRangedValue
                (_network.get_neuronDesirabilityMap(),
                _network.get_networkParameters().Width,
                _network.get_networkParameters().Height,
                _network.get_networkParameters().NeuronDesirabilityInfluenceRange,
                _network.get_networkParameters().NeuronDesirabilityMaxInfluence);
    }

    protected void AddProportionalRangedValue(double[][] map, int width, int height, int influencedRange, double maxValue) {
        int xMin = Math.max(_posX - influencedRange, 0);
        int xMax = Math.min(_posX + influencedRange, width);
        int yMin = Math.max(_posY - influencedRange, 0);
        int yMax = Math.min(_posY + influencedRange, height);

        for (int y = yMin; y < yMax; y++) {
            for (int x = xMin; x < xMax; x++) {
                double distance = Extensions.GetDistance(_posX, _posY, x, y);

                double influenceByRange = Math.max(influencedRange - distance, 0);

                map[y][x] = Math.min(1, map[y][x] + influenceByRange / influencedRange * maxValue);
            }
        }
    }

    //region Instance
    protected NeuronBase(int id, Network network) {
        _id = id;
        _network = network;

        _neuronPositionMap = _network.get_neuronPositionMap();
    }

    //endregion
}
