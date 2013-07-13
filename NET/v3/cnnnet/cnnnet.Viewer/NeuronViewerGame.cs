using cnnnet.Lib.Neurons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cnnnet.Viewer
{
    public class NeuronViewerGame : Game
    {
        #region Fields

#pragma warning disable 169
        private const int ColorIndexRed = 0;
        private const int ColorIndexGreen = 1;
        private const int ColorIndexBlue = 2;
#pragma warning restore 169

        private const int Width = 300;
        private const int Height = 300;

        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Resources _resources;
        private Texture2D _textureBackground;
        private Texture2D _textureBlank;

        private byte[] _backgroundData;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();

            IsMouseVisible = true;
            IsFixedTimeStep = false;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // use this.Content to load your game content here
            _resources = new Resources(Content);

            _textureBackground = new Texture2D(GraphicsDevice, Width, Height);
            _backgroundData = Enumerable.Repeat<byte>(0, _textureBackground.Width * _textureBackground.Height * 4).ToArray();
            _textureBackground.SetData(_backgroundData);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            GraphicsDevice.Textures[0] = null;

            _backgroundData = Enumerable.Repeat<byte>(0, _textureBackground.Width * _textureBackground.Height * 4).ToArray();

            // Drawing code here
            _spriteBatch.Begin();

            _textureBackground.SetData(_backgroundData);
            _spriteBatch.Draw(_textureBackground, new Rectangle(0, 0, Width, Height), Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);
            System.Windows.Forms.Application.DoEvents();
        }

        private void UpdateBackground(double[,] values, int colorIndex)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    int index = y * Width * 4 + x * 4;

                    _backgroundData[index + colorIndex] = (byte)(values[y, x] * 255);
                }
            }
        }

        #endregion Methods

        #region Instance

        public NeuronViewerGame()
            : base()
        {
            _graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = Width,
                PreferredBackBufferHeight = Height
            };
            _graphics.ApplyChanges();
            Content.RootDirectory = "Content";

            _textureBlank = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            _textureBlank.SetData(new[] { Color.White });
        }

        #endregion Instance
    }
}
