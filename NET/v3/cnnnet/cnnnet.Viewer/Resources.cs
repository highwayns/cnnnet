using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace cnnnet.Viewer
{
    public class Resources
    {
        #region Fields

        private readonly ContentManager _contentManager;

        public readonly Texture2D NeuronIdle;
        public readonly Texture2D NeuronActive;
        public readonly Texture2D NeuronInputIdle;
        public readonly Texture2D NeuronInputActive;
        public readonly Texture2D NeuronHover;
        public readonly Texture2D NeuronSelected;

        #endregion

        #region Instance

        public Resources(ContentManager contentManager)
        {
            _contentManager = contentManager;

            NeuronIdle = _contentManager.Load<Texture2D>("neuronIdle");
            NeuronActive = _contentManager.Load<Texture2D>("neuronActive");
            NeuronInputIdle = _contentManager.Load<Texture2D>("neuronInputIdle");
            NeuronInputActive = _contentManager.Load<Texture2D>("neuronInputActive");
            NeuronHover = _contentManager.Load<Texture2D>("neuronHover");
            NeuronSelected = _contentManager.Load<Texture2D>("neuronSelected");
        }

        #endregion
    }
}
