using cnnnet.Lib;

namespace cnnnet.ViewerWpf.ViewerManagers
{
    public class ViewerManagerAxonTerminal : ViewerManagerBase
    {
        #region Methods

        protected override void UpdateInternal(double elapsed, int mousePosX, int mousePosY, bool leftButtonPressed)
        {
        }

        #endregion

        #region Instance

        public ViewerManagerAxonTerminal()
            : base(Constants.AxonGuidanceForcesWidth, Constants.AxonGuidanceForcesHeight)
        {
        }

        #endregion
    }
}
