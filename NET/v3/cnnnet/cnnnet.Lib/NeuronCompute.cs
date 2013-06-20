namespace cnnnet.Lib
{
    public class NeuronCompute : NeuronBase
    {
        #region Methods

        public override void Process()
        {
            if (HasSomaReachedFinalPosition)
            {
                ProcessHasReachedFinalPosition();
            }
            else
            {
                #region Neuron searches for better position

                if (_movedDistance < _cnnNet.MaxNeuronMoveDistance)
                {
                    ProcessMoveToHigherDesirability();
                    _iterationsSinceLastActivation++;
                    AddUndesirability();
                }

                #endregion
            }
        }

        /// <summary>
        /// Processes the HasReachedFinalPosition = true branch
        /// </summary>
        private void ProcessHasReachedFinalPosition()
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

        #endregion

        #region Instance

        public NeuronCompute(int id, CnnNet cnnNet)
            : base(id, cnnNet)
        {
        }

        #endregion
    }
}
