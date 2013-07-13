#region Using Statements

using cnnnet.Lib;
using cnnnet.Lib.Neurons;
using cnnnet.Lib.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using System;
using System.Collections.Generic;
using System.Linq;

#endregion Using Statements

namespace cnnnet.Viewer
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class CnnNetGame : Game
    {
        #region Fields

        private readonly FormNetworkControl _formNetworkControl;

#pragma warning disable 169
        private const int ColorIndexRed = 0;
        private const int ColorIndexGreen = 1;
        private const int ColorIndexBlue = 2;
#pragma warning restore 169

        private const int Width = 400;
        private const int Height = 300;

        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Resources _resources;
        private Texture2D _textureBackground;
        private Texture2D _textureBlank;

        private byte[] _backgroundData;
        private readonly CnnNet _network;

        private Neuron _neuronSelected;

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

            var neuronDesirabilityMap = _network.NeuronDesirabilityMap ?? new double[0, 0];
            var neuronUndesirabilityMap = _network.NeuronUndesirabilityMap ?? new double[0, 0];

            var tableNeurons = _network.Neurons ?? new Neuron[0];

            _backgroundData = Enumerable.Repeat<byte>(0, _textureBackground.Width * _textureBackground.Height * 4).ToArray();

            // Drawing code here
            _spriteBatch.Begin();

            #region Background

            if (_formNetworkControl.dsNeuronDesirabilityMap.Checked)
            {
                UpdateBackground(neuronDesirabilityMap, ColorIndexGreen);
            }

            if (_formNetworkControl.dsNeuronUndesirabilityMap.Checked)
            {
                UpdateBackground(neuronUndesirabilityMap, ColorIndexRed);
            }

            #endregion Background

            _textureBackground.SetData(_backgroundData);
            _spriteBatch.Draw(_textureBackground, new Rectangle(0, 0, Width, Height), Color.White);

            #region Neurons

            UpdateNeurons(tableNeurons);

            #endregion Neurons

            UpdateHoverAndSelectedNeuron();

            _spriteBatch.End();

            base.Draw(gameTime);
            System.Windows.Forms.Application.DoEvents();
        }

        private void UpdateHoverAndSelectedNeuron()
        {
            var mouseState = Mouse.GetState();
            _formNetworkControl.Text = string.Format("X = {0} Y = {1}", mouseState.X, mouseState.Y);

            var neuron = Extensions.GetClosestNeuronsWithinRange(mouseState.X, mouseState.Y, _network, 10);

            if (neuron != null)
            {
                if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    _neuronSelected = neuron;
                    GameUtils.DrawTexture(_spriteBatch, _resources.NeuronSelected, neuron.PosX, neuron.PosY);
                }
                else
                {
                    GameUtils.DrawTexture(_spriteBatch, _resources.NeuronHover, neuron.PosX, neuron.PosY);
                }
            }

            if (_neuronSelected != null)
            {
                GameUtils.DrawTexture(_spriteBatch, _resources.NeuronHover, _neuronSelected.PosX, _neuronSelected.PosY);
                _formNetworkControl.InvokeEx(form => form.lbNeuronId.Text = _neuronSelected.Id.ToString());
                _formNetworkControl.InvokeEx(form => form.lbNeuronLocation.Text = string.Format("X = {0} Y = {1}", _neuronSelected.PosX, _neuronSelected.PosY));
                _formNetworkControl.InvokeEx(form => _neuronSelected.BreakOnProcessCall = form.cboxBreakOnceOnNeuronProcess.Checked);
            }
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

        private void UpdateNeurons(IEnumerable<Neuron> neurons)
        {
            foreach (Neuron neuron in neurons)
            {
                var neuronTexture = neuron.IsActive
                                      ? neuron.IsInputNeuron ? _resources.NeuronInputActive : _resources.NeuronActive
                                      : neuron.IsInputNeuron ? _resources.NeuronInputIdle : _resources.NeuronIdle;

                GameUtils.DrawTexture(_spriteBatch, neuronTexture, neuron.PosX, neuron.PosY);

                if (neuron.HasSomaReachedFinalPosition
                    && _formNetworkControl.dsDisplayNeuronDesirabilityRange.Checked)
                {
                    GameUtils.DrawTexture(_spriteBatch, GameUtils.CreateCircle(GraphicsDevice, _network.NeuronDesirabilityInfluenceRange), neuron.PosX, neuron.PosY);
                }

                #region Draw Axon

                for (int i = 1; i < neuron.AxonWaypoints.Count; i++)
                {
                    var startPos = new Vector2(neuron.AxonWaypoints[i - 1].X, neuron.AxonWaypoints[i - 1].Y);
                    var endPos = new Vector2(neuron.AxonWaypoints[i].X, neuron.AxonWaypoints[i].Y);

                    _textureBlank = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
                    _textureBlank.SetData(new[] { Color.White });
                    GameUtils.DrawLine(_spriteBatch, _textureBlank, 1, neuron == _neuronSelected ? Color.DarkBlue : Color.White, startPos, endPos);
                }

                #endregion

                var axonLastWayPoint = neuron.AxonWaypoints.LastOrDefault();

                if (neuron.IsInputNeuron)
                {
                    var axonLastWaypointCircle = GameUtils.CreateCircle(GraphicsDevice, _network.AxonGuidanceForceSearchPlainRange);

                    _spriteBatch.Draw(axonLastWaypointCircle, new Vector2(axonLastWayPoint.X - _network.AxonGuidanceForceSearchPlainRange, axonLastWayPoint.Y - _network.AxonGuidanceForceSearchPlainRange), Color.Red);
                }
            }
        }

        #endregion Methods

        #region Instance

        public CnnNetGame()
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

            _formNetworkControl = new FormNetworkControl();
            _network = new CnnNet(Width, Height);
            _formNetworkControl.CnnNet = _network;
        }

        #endregion Instance
    }
}