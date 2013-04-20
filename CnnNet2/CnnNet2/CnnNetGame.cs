using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using CnnNetLib2;

namespace CnnNet2
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class CnnNetGame : Microsoft.Xna.Framework.Game
    {
        #region Fields

        private const int Width = 800;
        private const int Height = 600;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Texture2D _neuronIdle;
        private Texture2D _neuronActive;
        private Texture2D _neuronInputIdle;
        private Texture2D _neuronInputActive;
        private Texture2D _background;

        private byte[] _backgroundData;
        private readonly CnnNet _cnnNet;

        #endregion

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
            _neuronIdle = Content.Load<Texture2D>("neuronIdle");
            _neuronActive = Content.Load<Texture2D>("neuronActive");
            _neuronInputIdle = Content.Load<Texture2D>("neuronInputIdle");
            _neuronInputActive = Content.Load<Texture2D>("neuronInputActive");

            _background = new Texture2D(GraphicsDevice, Width, Height);
            _backgroundData = Enumerable.Repeat<byte>(255, _background.Width * _background.Height * 4).ToArray();
            _background.SetData(_backgroundData);
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
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            GraphicsDevice.Textures[0] = null;

            var neuronDesirabilityMap = _cnnNet.NeuronDesirabilityMap ?? new double[0, 0];
            var tableNeurons = _cnnNet.Neurons ?? new Neuron[0];
            var activeNeurons = _cnnNet.ActiveNeurons ?? new Neuron[0];
            var inputNeurons = _cnnNet.InputNeurons ?? new Neuron[0];

            // Drawing code here
            _spriteBatch.Begin();

            UpdateDesirability(neuronDesirabilityMap);
            UpdateNeurons(tableNeurons, activeNeurons, inputNeurons);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void UpdateDesirability(double[,] tableNeuronDesirability)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    int index = y * Width * 4 + x * 4;

                    _backgroundData[index + 0] = 0;
                    _backgroundData[index + 1] = (byte)(tableNeuronDesirability[y, x] * 255);
                    _backgroundData[index + 2] = 0;
                    _backgroundData[index + 3] = 255;
                }
            }

            _background.SetData(_backgroundData);
            _spriteBatch.Draw(_background, new Rectangle(0, 0, Width, Height), Color.White);
        }

        private void UpdateNeurons(Neuron[] tableNeurons, Neuron[] activeNeurons, Neuron[] inputNeurons)
        {
            for (int i = 0; i < tableNeurons.Length; i++)
            {
                _spriteBatch.Draw(activeNeurons.Contains(tableNeurons[i])
                                      ? inputNeurons.Contains(tableNeurons[i]) ? _neuronInputActive : _neuronActive
                                      : inputNeurons.Contains(tableNeurons[i]) ? _neuronInputIdle : _neuronIdle,
                                  new Vector2(tableNeurons[i].PosX, tableNeurons[i].PosY), Color.White);
            }
        }

        #endregion

        #region Instance

        public CnnNetGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            _cnnNet = new CnnNet(Width, Height, new NetworkParameters());
        }

        #endregion
    }
}
