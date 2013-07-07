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

        private Texture2D _textureNeuronIdle;
        private Texture2D _textureNeuronActive;

        private Texture2D _textureNeuronInputIdle;
        private Texture2D _textureNeuronInputActive;

        private Texture2D _textureNeuronHover;
        private Texture2D _textureNeuronSelected;

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
            _textureNeuronIdle = Content.Load<Texture2D>("neuronIdle");
            _textureNeuronActive = Content.Load<Texture2D>("neuronActive");
            _textureNeuronInputIdle = Content.Load<Texture2D>("neuronInputIdle");
            _textureNeuronInputActive = Content.Load<Texture2D>("neuronInputActive");
            _textureNeuronHover = Content.Load<Texture2D>("neuronHover");
            _textureNeuronSelected = Content.Load<Texture2D>("neuronSelected");

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
                    DrawTexture(_textureNeuronSelected, neuron.PosX, neuron.PosY);
                }
                else
                {
                    DrawTexture(_textureNeuronHover, neuron.PosX, neuron.PosY);
                }
            }

            if (_neuronSelected != null)
            {
                DrawTexture(_textureNeuronHover, _neuronSelected.PosX, _neuronSelected.PosY);
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
                                      ? neuron.IsInputNeuron ? _textureNeuronInputActive : _textureNeuronActive
                                      : neuron.IsInputNeuron ? _textureNeuronInputIdle : _textureNeuronIdle;

                DrawTexture(neuronTexture, neuron.PosX, neuron.PosY);

                if (neuron.HasSomaReachedFinalPosition
                    && _formNetworkControl.dsDisplayNeuronDesirabilityRange.Checked)
                {
                    DrawTexture(CreateCircle(_network.NeuronDesirabilityInfluenceRange), neuron.PosX, neuron.PosY);
                }

                #region Draw Axon

                for (int i = 1; i < neuron.AxonWaypoints.Count; i++)
                {
                    var startPos = new Vector2(neuron.AxonWaypoints[i - 1].X, neuron.AxonWaypoints[i - 1].Y);
                    var endPos = new Vector2(neuron.AxonWaypoints[i].X, neuron.AxonWaypoints[i].Y);

                    _textureBlank = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
                    _textureBlank.SetData(new[] { Color.White });
                    DrawLine(_spriteBatch, _textureBlank, 1, neuron == _neuronSelected ? Color.DarkBlue : Color.White, startPos, endPos);
                }

                #endregion

                var axonLastWayPoint = neuron.AxonWaypoints.LastOrDefault();

                if (neuron.IsInputNeuron)
                {
                    var axonLastWaypointCircle = CreateCircle(_network.AxonGuidanceForceSearchPlainRange);

                    _spriteBatch.Draw(axonLastWaypointCircle, new Vector2(axonLastWayPoint.X - _network.AxonGuidanceForceSearchPlainRange, axonLastWayPoint.Y - _network.AxonGuidanceForceSearchPlainRange), Color.Red);
                }
            }
        }

        private Texture2D CreateCircle(int radius)
        {
            int outerRadius = radius * 2 + 2; // So circle doesn't go out of bounds
            var texture = new Texture2D(GraphicsDevice, outerRadius, outerRadius);

            var data = new Color[outerRadius * outerRadius];

            // Colour the entire texture transparent first.
            for (int i = 0; i < data.Length; i++)
                data[i] = Color.Transparent;

            // Work out the minimum step necessary using trigonometry + sine approximation.
            double angleStep = 1f / radius;

            for (double angle = 0; angle < Math.PI * 2; angle += angleStep)
            {
                // Use the parametric definition of a circle: http://en.wikipedia.org/wiki/Circle#Cartesian_coordinates
                var x = (int)Math.Round(radius + radius * Math.Cos(angle));
                var y = (int)Math.Round(radius + radius * Math.Sin(angle));

                data[y * outerRadius + x + 1] = Color.White;
            }

            texture.SetData(data);
            return texture;
        }

        private void DrawLine(SpriteBatch batch, Texture2D blank,
              float width, Color color, Vector2 point1, Vector2 point2)
        {
            var angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            var length = Vector2.Distance(point1, point2);

            batch.Draw(blank, point1, null, color,
                       angle, Vector2.Zero, new Vector2(length, width),
                       SpriteEffects.None, 0);
        }

        private void DrawTexture(Texture2D texture, int posX, int posY)
        {
            _spriteBatch.Draw(texture,
                new Vector2(posX - _textureNeuronIdle.Width / 2, posY - _textureNeuronIdle.Height / 2),
                Color.White);
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