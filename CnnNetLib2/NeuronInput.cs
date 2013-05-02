using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CnnNetLib2
{
    public class NeuronInput : NeuronBase
    {
        #region Methods

        public void SetIsActive(bool isActive)
        {
            _isActive = isActive;
        }

        public override void Process()
        {
            if (IsActive)
            {
                AddDesirability();
            }
        }

        #endregion

        #region Instance

        public NeuronInput(int id, CnnNet cnnNet)
            : base(id, cnnNet)
        {
            _hasReachedFinalPosition = true;
        }

        #endregion
    }
}
