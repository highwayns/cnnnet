using cnnnet.Lib.GuidanceForces;
using cnnnet.Lib.Neurons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cnnnet.ViewerWpf.Viewers
{
    public class ViewerAxonTerminalGuidanceForces : ViewerBase
    {
        #region Fields
        
        private Neuron _neuron;

        #endregion

        #region Properties

        public Neuron Neuron
        {
            get
            {
                return _neuron;
            }
            set
            {
                UnsubscribeFromGuidanceForces(_neuron);
                _neuron = value;
                SubscribeToGuidanceForces(_neuron);

                IsEnabled = _neuron != null;
            }
        }

        private void SubscribeToGuidanceForces(Neuron neuron)
        {
            if (neuron == null)
            {
                return;
            }

            neuron.AxonGuidanceForces.ToList().
                ForEach(guidanceForce => guidanceForce.ScoreAvailableEvent += OnGuidanceForceScoreAvailableEvent);
        }

        private void UnsubscribeFromGuidanceForces(Neuron neuron)
        {
            if (neuron == null)
            {
                return;
            }

            neuron.AxonGuidanceForces.ToList().
                ForEach(guidanceForce => guidanceForce.ScoreAvailableEvent -= OnGuidanceForceScoreAvailableEvent);
        }

        private void OnGuidanceForceScoreAvailableEvent(object sender, GuidanceForceScoreAvailableEventArgs e)
        {
            
        }

        #endregion

        #region Methods

        protected override void UpdateDataInternal(ref byte[,] data)
        {
            
        }

        #endregion

        #region Instance
        
        public ViewerAxonTerminalGuidanceForces()
            : base(51, 51, 3, false)
        {

        }

        #endregion
    }
}
