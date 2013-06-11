/**
 * Created with IntelliJ IDEA.
 * User: smiron86
 * Date: 6/11/13
 * Time: 10:35 PM
 */
public class SequentialActiveInputNeuronGenerator implements IActiveNeuronGenerator {

    //region Fields

    private final NeuronInput[] _inputNeurons;
    private final int _activeNeuronCount;

    private int _currentIndex;
    //endregion

    //region Methods
    @Override
    public NeuronInput[] GetActiveNeuronIds() {
        NeuronInput[] ret = new NeuronInput[_activeNeuronCount];

        for (int i = 0; i < _activeNeuronCount; i++) {
            ret[i] = _inputNeurons[(_currentIndex + i) % _inputNeurons.length];
        }
        _currentIndex = (_currentIndex + _activeNeuronCount) % _inputNeurons.length;

        return ret;
    }
    //endregion

    //region Instance
    public SequentialActiveInputNeuronGenerator(NeuronInput[] inputNeurons, int activeNeuronCount) {
        _inputNeurons = inputNeurons;
        _activeNeuronCount = activeNeuronCount;
    }
    //endregion
}
