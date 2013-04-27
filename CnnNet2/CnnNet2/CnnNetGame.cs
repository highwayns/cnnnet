using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CnnNetLib2;
using System;

namespace CnnNet2
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class CnnNetGame : Game
    {
        #region Fields

        private readonly FormNetworkControl _formNetworkControl;

        private const int Width = 800;
        private const int Height = 600;

        private readonly GraphicsDeviceManager _graphics;
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
            // Add your initialization logic here
            base.Initialize();

            IsMouseVisible = true;
            IsFixedTimeStep = false;

            _formNetworkControl.Show();
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
                || Keyboard.GetState().GetPressedKeys().Contains(Keys.Escape))
            {
                Exit();
            }

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

        private void UpdateNeurons(IEnumerable<Neuron> neurons, Neuron[] activeNeurons, Neuron[] inputNeurons)
        {
            Texture2D circle = CreateCircle(_cnnNet.NeuronInfluenceRange);

            foreach (Neuron neuron in neurons)
            {
                _spriteBatch.Draw(activeNeurons.Contains(neuron)
                                      ? inputNeurons.Contains(neuron) ? _neuronInputActive : _neuronActive
                                      : inputNeurons.Contains(neuron) ? _neuronInputIdle : _neuronIdle,
                                  new Vector2(neuron.PosX, neuron.PosY), Color.White);

                if (neuron.HasReachedFinalPosition)
                {
                    _spriteBatch.Draw(circle, new Vector2(neuron.PosX - _cnnNet.NeuronInfluenceRange, neuron.PosY - _cnnNet.NeuronInfluenceRange), Color.Red);
                }

            }
        }

        public Texture2D CreateCircle(int radius)
        {
            int outerRadius = radius * 2 + 2; // So circle doesn't go out of bounds
            Texture2D texture = new Texture2D(GraphicsDevice, outerRadius, outerRadius);

            Color[] data = new Color[outerRadius * outerRadius];

            // Colour the entire texture transparent first.
            for (int i = 0; i < data.Length; i++)
                data[i] = Color.Transparent;

            // Work out the minimum step necessary using trigonometry + sine approximation.
            double angleStep = 1f / radius;

            for (double angle = 0; angle < Math.PI * 2; angle += angleStep)
            {
                // Use the parametric definition of a circle: http://en.wikipedia.org/wiki/Circle#Cartesian_coordinates
                int x = (int)Math.Round(radius + radius * Math.Cos(angle));
                int y = (int)Math.Round(radius + radius * Math.Sin(angle));

                data[y * outerRadius + x + 1] = Color.White;
            }

            texture.SetData(data);
            return texture;
        }

        #endregion

        #region Instance

        public CnnNetGame()
        {
            _graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = Width,
                PreferredBackBufferHeight = Height
            };
            _graphics.ApplyChanges();

            Content.RootDirectory = "Content";

            _formNetworkControl = new FormNetworkControl();

            _cnnNet = new CnnNet(Width, Height, _formNetworkControl.GetNetworkParameters());

            _formNetworkControl.CnnNet = _cnnNet;
        }

        #endregion
    }
}
