namespace CnnNetLib
{
    public class SequentialActiveInputNeuronGenerator : IActiveNeuronGenerator
    {
        #region Fields
        
        private readonly int[] _inputNeuronIds;
        private readonly int _activeNeuronCount;

        private int _currentIndex;

        #endregion

        #region Methods

        public int[] GetActiveNeuronIds()
        {
            var ret = new int[_activeNeuronCount];

            for (int i = 0; i < _activeNeuronCount; i++)
            {
                ret[i] = _inputNeuronIds[(_currentIndex + i)%_inputNeuronIds.Length];
            }
            _currentIndex = (_currentIndex + _activeNeuronCount)%_inputNeuronIds.Length;

            return ret;
        }

        #endregion

        #region Instance

        public SequentialActiveInputNeuronGenerator(int[] inputNeuronIds, int activeNeuronCount)
        {
            _inputNeuronIds = inputNeuronIds;
            _activeNeuronCount = activeNeuronCount;
        }

        #endregion
    }
}
