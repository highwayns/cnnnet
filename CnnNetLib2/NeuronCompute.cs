using System;
using System.Collections.Generic;

namespace CnnNetLib2
{
    public class NeuronCompute : NeuronBase
    {
        

        #region Methods

        public override void Process()
        {
            if (HasReachedFinalPosition)
            {
                #region Increase region desirability if neuron fires

                if (IsActive)
                {
                    AddDesirability();
                    _iterationsSinceLastActivation = 0;
                }

                #endregion

                #region Else increase region UN-desirability

                else
                {
                    _iterationsSinceLastActivation++;
                    AddUndesirability();
                }

                #endregion
            }
            else
            {
                #region Neuron searches for better position

                if (_movedDistance < _cnnNet.MaxNeuronMoveDistance)
                {
                    _neuronIterationsLeftBeforeFinalPosition =
                        ProcessMoveToHigherDesirability()
                        ? _cnnNet.NeuronIterationCountBeforeFinalPosition
                        : _neuronIterationsLeftBeforeFinalPosition - 1;

                    if (_neuronIterationsLeftBeforeFinalPosition == 0)
                    {
                        _hasReachedFinalPosition = true;
                    }
                }

                #endregion
            }
        }

        #endregion

        #region Instance

        public NeuronCompute(int id, CnnNet cnnNet)
            : base(id, cnnNet)
        {
            _neuronIterationsLeftBeforeFinalPosition = _cnnNet.NeuronIterationCountBeforeFinalPosition;
        }

        #endregion
    }
}
