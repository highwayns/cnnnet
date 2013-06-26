using System;

namespace cnnnet.Lib.ActiveNeuronGenerator
{
    public class RandomActiveNeuronGenerator : IActiveNeuronGenerator
    {
        #region Fields

        private readonly int _neuronCount;
        private readonly double _percentActiveNourons;
        private readonly Random _random;

        #endregion Fields

        #region Methods

        public int[] GetActiveNeuronIds()
        {
            var retCount = (int)(_neuronCount * _percentActiveNourons);
            var ret = new int[retCount];
            for (int i = 0; i < retCount; i++)
            {
                ret[i] = _random.Next(_neuronCount);
            }
            return ret;
        }

        #endregion Methods

        #region Instance

        public RandomActiveNeuronGenerator(int neuronCount, double percentActiveNourons)
        {
            _neuronCount = neuronCount;
            _percentActiveNourons = percentActiveNourons;
            _random = new Random();
        }

        #endregion Instance
    }
}